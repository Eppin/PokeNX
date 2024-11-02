namespace PokeNX.Core.Models
{
    public class Event8Request
    {
        public bool SetIVs => true;

        public Filter Filter { get; set; }
    }
}
