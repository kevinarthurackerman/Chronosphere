using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Chronosphere
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddChronosphere(this IServiceCollection serviceCollection) => 
            AddChronosphere(serviceCollection, DateTimeOffset.UtcNow, Assembly.GetCallingAssembly());

        public static IServiceCollection AddChronosphere(this IServiceCollection serviceCollection, DateTimeOffset now) =>
            AddChronosphere(serviceCollection, now, Assembly.GetCallingAssembly());

        public static IServiceCollection AddChronosphere(this IServiceCollection serviceCollection, Assembly rootAssembly) =>
            AddChronosphere(serviceCollection, DateTimeOffset.UtcNow, rootAssembly);

        public static IServiceCollection AddChronosphere(this IServiceCollection serviceCollection, DateTimeOffset now, Assembly rootAssembly)
        {
            serviceCollection.AddScoped<IChronosphere>(x => new Chronosphere(now));
            serviceCollection.AddScoped<IReadOnlyChronosphere>(x => x.GetRequiredService<IChronosphere>());
            serviceCollection.AddScoped<ISystemClock, SystemClock>();

            foreach (var systemClock in BuildSystemClocks(rootAssembly))
                serviceCollection.AddScoped(systemClock.InterfaceType, systemClock.InstanceType);

            return serviceCollection;
        }

        private static IEnumerable<(Type InterfaceType, Type InstanceType)> BuildSystemClocks(Assembly rootAssembly)
        {
            var assemblies = new Stack<Assembly>(new[] { rootAssembly });
            
            var assembliesLookup = assemblies.ToHashSet();

            while(assemblies.TryPop(out var assembly))
            {
                foreach (var referencedAssemblyName in assembly.GetReferencedAssemblies())
                {
                    try
                    {
                        var referencedAssembly = Assembly.Load(referencedAssemblyName);

                        if (assembliesLookup.Contains(referencedAssembly)) continue;

                        assembliesLookup.Add(referencedAssembly);
                        assemblies.Push(referencedAssembly);
                    }
                    catch (FileNotFoundException) { }
                }
            }

            var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Chronosphere_Assembly_{Guid.NewGuid()}"), AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule($"Chronosphere_Module_{Guid.NewGuid()}");

            var baseCtor = typeof(SystemClock).GetConstructors().Single();

            var systemClockTypes = assembliesLookup.SelectMany(x => x.GetTypes())
                .Where(x =>
                {
                    if (x.Name != "ISystemClock") return false;
                    if (!x.IsInterface) return false;
                    if (x == typeof(ISystemClock)) return false;
                    var members = x.GetMembers();
                    if (!members.All(x =>
                        x is MethodInfo mx && mx.Name == "get_UtcNow" && mx.ReturnType == typeof(DateTimeOffset)
                        || x is PropertyInfo px && px.Name == "UtcNow" && px.PropertyType == typeof(DateTimeOffset)
                    )) return false;
                    return true;
                })
                .Select(x =>
                {
                    var instanceType = BuildSystemClockInstanceType(x);
                    return (InterfaceType: x, InstanceType: instanceType);
                })
                .ToArray();

            return systemClockTypes;

            Type BuildSystemClockInstanceType(Type interfaceType)
            {
                var typeBuilder = moduleBuilder.DefineType(
                    $"Chronosphere_SystemClock_{Guid.NewGuid()}",
                    TypeAttributes.Public,
                    typeof(SystemClock),
                    new Type[] { interfaceType, typeof(ISystemClock) }
                );

                var ctorBuilder = typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.HasThis,
                    new[] { typeof(IReadOnlyChronosphere) }
                );

                ILGenerator ctorIL = ctorBuilder.GetILGenerator();
                ctorIL.Emit(OpCodes.Ldarg_0);
                ctorIL.Emit(OpCodes.Ldarg_1);
                ctorIL.Emit(OpCodes.Call, baseCtor);
                ctorIL.Emit(OpCodes.Ret);

                return typeBuilder.CreateType();
            }
        }
    }
}
