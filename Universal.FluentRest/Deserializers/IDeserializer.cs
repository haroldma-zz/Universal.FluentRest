namespace Universal.FluentRest.Deserializers
{
    public interface IDeserializer
    {
        T Deserialize<T>(string text);
        string Serialize(object obj);
    }
}