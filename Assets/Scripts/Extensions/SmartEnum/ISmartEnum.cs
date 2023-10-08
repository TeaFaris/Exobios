public interface ISmartEnum<TID>
        where TID : struct
{
    public TID ID { get; }
    public string Name { get; }
}