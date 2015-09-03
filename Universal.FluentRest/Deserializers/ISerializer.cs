namespace Universal.FluentRest.Deserializers
{
    public interface ISerializer
    {
        string Serialize(object obj);
    }
}