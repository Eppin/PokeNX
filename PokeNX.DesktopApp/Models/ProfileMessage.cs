namespace PokeNX.DesktopApp.Models;

public class ProfileMessage
{
    public ushort? TID { get; }

    public ushort? SID { get; }

    public bool? ShinyCharm { get; }

    public bool? OvalCharm { get; }

    public ProfileMessage(ushort? tid = null, ushort? sid = null, bool? shinyCharm = null, bool? ovalCharm = null)
    {
        TID = tid;
        SID = sid;
        ShinyCharm = shinyCharm;
        OvalCharm = ovalCharm;
    }
}
