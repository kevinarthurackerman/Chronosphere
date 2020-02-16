using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Chronosphere
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Assembly> LoadTree(this Assembly assembly)
        {
            var assemblies = new Stack<Assembly>(new[] { assembly });

            var assembliesLookup = assemblies.ToHashSet();

            while (assemblies.TryPop(out var checkingAssembly))
            {
                foreach (var referencedAssemblyName in checkingAssembly.GetReferencedAssemblies())
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

            return assembliesLookup.AsEnumerable();
        }
    }
}
