namespace Universal.FluentRest.Deserializers
{
    public interface IDeserializer
    {
        T Deserialize<T>(string text);
    }
}