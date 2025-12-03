using Newtonsoft.Json.Serialization;

namespace erpsolution.api
{
    public class JsonLowercaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToLower();
        }
    }
}