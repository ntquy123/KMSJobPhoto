using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace erpsolution.api.Attribute
{
    public class IgnoreJsonAttributesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            // Serialize all the properties
            property.ShouldSerialize = _ => true;
            return property;
        }
    }
}
