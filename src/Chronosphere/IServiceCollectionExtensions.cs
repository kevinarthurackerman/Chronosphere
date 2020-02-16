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

            var systemClockInterfaces = GetSystemClockInterfaces(rootAssembly);

            var systemClockInstanceType = BuildSystemClockInstanceType(systemClockInterfaces);

            foreach (var systemClockInterfaceType in systemClockInterfaces)
                serviceCollection.AddScoped(systemClockInterfaceType, systemClockInstanceType);

            return serviceCollection;

            static IEnumerable<Type> GetSystemClockInterfaces(Assembly rootAssembly)
            {
                var assemblies = new Stack<Assembly>(new[] { rootAssembly });

                var assembliesLookup = assemblies.ToHashSet();

                while (assemblies.TryPop(out var assembly))
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

                return assembliesLookup.SelectMany(x => x.GetTypes())
                    .Where(x => x.IsSystemClock())
                    .Append(typeof(ISystemClock))
                    .AsEnumerable();
            }

            static Type BuildSystemClockInstanceType(IEnumerable<Type> systemClockInterfaceTypes)
            {
                var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName($"Chronosphere_Assembly_{Guid.NewGuid()}"), AssemblyBuilderAccess.Run);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule($"Chronosphere_Module_{Guid.NewGuid()}");

                var baseCtor = typeof(SystemClock).GetConstructors().Single();

                var typeBuilder = moduleBuilder.DefineType(
                    $"Chronosphere_SystemClock_{Guid.NewGuid()}",
                    TypeAttributes.Public,
                    typeof(SystemClock),
                    systemClockInterfaceTypes.ToArray()
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
