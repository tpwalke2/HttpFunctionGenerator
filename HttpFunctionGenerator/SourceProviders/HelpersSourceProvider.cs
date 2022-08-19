using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class HelpersSourceProvider
{
    public static SourceText TypeHelpersSource() => SourceText.From(
        $@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace {Constants.PackageBaseName}.Reflection;

public static class TypeHelpers
{{
    public static IEnumerable<Type> GetAllTypes(Assembly entryAssembly) => GetAssembliesInCurrentContext(entryAssembly)
        .SelectMany(assembly => assembly.GetTypes());

    public static IEnumerable<Type> GetAllInterfaces(IEnumerable<Type> allPublicInterfaceTypes) => allPublicInterfaceTypes
        .Where(t => t.IsInterface && t.IsPublic);

    private static IEnumerable<Assembly> GetAssembliesInCurrentContext(Assembly entryAssembly)
    {{
        var returnAssemblies = new List<Assembly>(new[] {{ entryAssembly }});
        var loadedAssemblies = new HashSet<string>();
        var assembliesToCheck = new Queue<Assembly>();

        assembliesToCheck.Enqueue(entryAssembly);

        var assemblyPrefix = entryAssembly.FullName?.Split('.')[0] ?? """";

        while (assembliesToCheck.Any())
        {{
            var assemblyToCheck = assembliesToCheck.Dequeue();

            foreach (var reference in assemblyToCheck.GetReferencedAssemblies())
            {{
                if (!reference.FullName.StartsWith(assemblyPrefix) ||
                    loadedAssemblies.Contains(reference.FullName)) continue;
                
                var assembly = Assembly.Load(reference);
                assembliesToCheck.Enqueue(assembly);
                loadedAssemblies.Add(reference.FullName);
                returnAssemblies.Add(assembly);
            }}
        }}

        return returnAssemblies;
    }}
}}",
        Encoding.UTF8);
}