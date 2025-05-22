using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Flashcards.Common.Messages;

namespace Flashcards.Common;

internal class JsonMessageTypeResolver() : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Type == typeof(IMessage))
        {
            ConfigurePolymorphism(jsonTypeInfo, typeof(IEvent));
            ConfigurePolymorphism(jsonTypeInfo, typeof(ICommand));
        }
        else if (jsonTypeInfo.Type == typeof(IEvent))
        {
            ConfigurePolymorphism(jsonTypeInfo, typeof(IEvent));
        }
        else if (jsonTypeInfo.Type == typeof(ICommand))
        {
            ConfigurePolymorphism(jsonTypeInfo, typeof(ICommand));
        }

        return jsonTypeInfo;
    }
    
    private static void ConfigurePolymorphism(JsonTypeInfo jsonTypeInfo, Type interfaceType)
    {
        jsonTypeInfo.PolymorphismOptions ??= new JsonPolymorphismOptions
        {
            TypeDiscriminatorPropertyName = "$message",
            IgnoreUnrecognizedTypeDiscriminators = false,
            UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization
        };
        
        var derivedTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.GetInterfaces().Contains(interfaceType))
            .Where(t => !t.IsAbstract)
            .Select(t => new JsonDerivedType(t, t.Name));

        foreach (var derivedType in derivedTypes)
        {
            jsonTypeInfo.PolymorphismOptions!.DerivedTypes.Add(derivedType);
        }
    }
}