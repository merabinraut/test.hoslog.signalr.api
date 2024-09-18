using Newtonsoft.Json;

namespace hoslog.signalr.api.Utilities.Helper;
public static class Helper
{
    public static T MapObject<T>(this object item)
    {
        T sr = default(T);
        if (item != null)
        {
            var obj = JsonConvert.SerializeObject(item);
            sr = JsonConvert.DeserializeObject<T>(obj);
        }
        return sr;
    }

    public static List<T> MapObjects<T>(this object item)
    {
        List<T> sr = default(List<T>);
        if (item != null)
        {
            var obj = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            sr = JsonConvert.DeserializeObject<List<T>>(obj);
        }
        return sr;
    }
}