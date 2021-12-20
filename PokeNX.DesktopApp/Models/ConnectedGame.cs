namespace PokeNX.DesktopApp.Models
{
    using Core.Models.Enums;

    public class ConnectedGame
    {
        public Game Game { get; set; } = Game.None;

        public uint TID { get; set; }

        public uint SID { get; set; }

        public bool HasShinyCharm { get; set; }

        public bool HasOvalCharm { get; set; }
    }
}
