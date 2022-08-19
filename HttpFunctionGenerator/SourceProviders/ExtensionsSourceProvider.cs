using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace HttpFunctionGenerator.SourceProviders;

public static class ExtensionsSourceProvider
{
    public static SourceText HostBuilderConfigurationSource() => SourceText.From(
        @"using Microsoft.Extensions.Hosting;

namespace HttpFunction.Extensions;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureGeneratedHttpFunctions(this IHostBuilder hostBuilder)
    {
        return hostBuilder
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureServices(services =>
            {
                services.AddRemainingDependencies<Program>();
            });
    }
}",
        Encoding.UTF8);
    
    public static SourceText ServiceCollectionExtensionsSource() => SourceText.From(
        @"using System;
using System.Collections.Generic;
using System.Linq;
using HttpFunction.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace HttpFunction.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRemainingDependencies<TProgram>(this IServiceCollection services)
    {
        var allTypes = TypeHelpers.GetAllTypes(typeof(TProgram).Assembly).ToList();
        var allInterfaces = TypeHelpers.GetAllInterfaces(allTypes).ToList();
        var allClasses = allTypes.Where(t => t.IsClass && !t.IsAbstract && t.IsPublic).ToList();
        var addedClasses = new HashSet<Type>();

        // add all non-generic types not already added as scoped
        allInterfaces
            .Where(t => !t.IsGenericType)
            .Select(i =>
            {
                var concreteType = allClasses.FirstOrDefault(i.IsAssignableFrom);
                return new
                {
                    Interface = i,
                    ConcreteType = concreteType
                };
            })
            .ForEach(x =>
            {
                if (x.ConcreteType != null) {
                    services.TryAddScoped(x.Interface, x.ConcreteType);
                    addedClasses.Add(x.ConcreteType);
                }
            });
                
        // add all generic types not already added
        allInterfaces
            .Where(t => t.IsGenericType)
            .ForEach(genericInterface =>
            {
                allClasses
                    .Where(x => x.GetInterfaces()
                        .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == genericInterface))
                    .Where(x => !x.IsGenericType)
                    .ForEach(implementationType =>
                    {
                        var firstInterface = implementationType.GetInterfaces().First();
                        services.TryAddScoped(firstInterface, implementationType);
                        addedClasses.Add(implementationType);
                    });
            });

        // add all classes not already added
        allClasses
            .Where(t => !addedClasses.Contains(t))
            .ForEach(services.TryAddScoped);
        
        return services;
    }
}",
        Encoding.UTF8);
    
    public static SourceText EnumerableExtensionsSource() => SourceText.From(
        @"using System;
using System.Collections.Generic;

namespace HttpFunction.Extensions;

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> itemAction)
    {
        foreach (var item in source)
        {
            itemAction(item);
        }
    }
}",
        Encoding.UTF8);
}