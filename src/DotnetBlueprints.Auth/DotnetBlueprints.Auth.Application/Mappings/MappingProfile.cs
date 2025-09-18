using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DotnetBlueprints.Auth.Application.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly(Assembly.GetExecutingAssembly());
    }

    private void ApplyMappingsFromAssembly(Assembly assembly)
    {
        var mapFromType = typeof(IMapFrom<>);
        const string methodName = nameof(IMapFrom<object>.Mapping);

        bool HasInterface(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == mapFromType;

        var types = assembly.GetExportedTypes()
            .Where(t => t.GetInterfaces().Any(HasInterface))
            .ToList();

        var argTypes = new[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);
            var methodInfo = type.GetMethod(methodName);

            if (methodInfo is not null)
            {
                methodInfo.Invoke(instance, new object[] { this });
                continue;
            }

            foreach (var i in type.GetInterfaces().Where(HasInterface))
            {
                var interfaceMethod = i.GetMethod(methodName, argTypes);
                interfaceMethod?.Invoke(instance, new object[] { this });
            }
        }
    }
}
