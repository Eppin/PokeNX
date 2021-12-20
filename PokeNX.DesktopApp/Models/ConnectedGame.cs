namespace PokeNX.DesktopApp.Models
{
    using Core.Models.Enums;
    using Utils;

    public class ConnectedGame
    {
        public Game Game { get; set; } = Game.None;

        public uint TID { get; set; }

        public uint SID { get; set; }

        private bool _hasShinyCharm;
        public bool HasShinyCharm
        {
            get => _hasShinyCharm;
            set
            {
                _hasShinyCharm = value;
                EventAggregator.PostMessage(new ProfileMessage(shinyCharm: value));
            }
        }

        private bool _hasOvalCharm;
        public bool HasOvalCharm
        {
            get => _hasOvalCharm;
            set
            {
                _hasOvalCharm = value;
                EventAggregator.PostMessage(new ProfileMessage(ovalCharm: value));
            }
        }
    }
}
