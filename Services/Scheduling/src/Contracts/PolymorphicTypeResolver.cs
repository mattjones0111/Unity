using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Contracts;

public class PolymorphicTypeResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        Type baseType = typeof(TemplateItem);
        if (jsonTypeInfo.Type == baseType)
        {
            jsonTypeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                IgnoreUnrecognizedTypeDiscriminators = true,
                UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FailSerialization,
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(CommentTemplateItem), nameof(CommentTemplateItem)),
                    new JsonDerivedType(typeof(AudioCriteriaTemplateItem), nameof(AudioCriteriaTemplateItem)),
                    new JsonDerivedType(typeof(AudioItemTemplateItem), nameof(AudioItemTemplateItem)),
                }
            };
        }

        return jsonTypeInfo;
    }
}