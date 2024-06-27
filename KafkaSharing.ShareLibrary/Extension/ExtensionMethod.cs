using KafkaSharing.ShareLibrary.Dto;
using Newtonsoft.Json;

namespace KafkaSharing.ShareLibrary.Extension;

public static class ExtensionMethod
{
    public static string SerializeObject(this object input)
    {
        return JsonConvert.SerializeObject(input, Formatting.Indented);
    }

    public static TType DeserializeObject<TType>(this string input)
        where TType : class
    {
        return JsonConvert.DeserializeObject<TType>(input);
    }
}
