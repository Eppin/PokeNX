namespace PokeNX.DesktopApp.Models;

public class ConnectionMessage
{
    public bool IsConnected { get; set; }

    public ConnectionMessage(bool isConnected)
    {
        IsConnected = isConnected;
    }
}
