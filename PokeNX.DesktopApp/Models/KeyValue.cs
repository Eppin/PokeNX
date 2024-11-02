namespace PokeNX.DesktopApp.Models;

public class KeyValue<TKey, TValue>
{
    public TKey Key { get; set; }

    public TValue Value { get; set; }

    public KeyValue(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Value}";
    }
}
