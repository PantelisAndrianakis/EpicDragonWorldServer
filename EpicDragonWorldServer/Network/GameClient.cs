using System;
using System.Net.Sockets;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class GameClient
{
    private readonly NetworkStream _networkStream;
    private readonly string _ip;
    private string _accountName = "";
    private Player _activeChar;

    public GameClient(NetworkStream stream, string address)
    {
        _networkStream = stream;
        // Clean IP address.
        _ip = address.Substring(0, address.LastIndexOf(']'));
        int start = _ip.LastIndexOf(':') + 1;
        _ip = _ip.Substring(start, _ip.Length - start);
    }

    public async void ChannelSend(SendablePacket packet)
    {
        try
        {
            await _networkStream.WriteAsync(packet.GetSendableBytes());
        }
        catch (Exception)
        {
            // Connection closed from client side.
        }
    }

    public string GetIp()
    {
        return _ip;
    }

    public string GetAccountName()
    {
        return _accountName;
    }

    public void SetAccountName(string name)
    {
        _accountName = name;
    }

    public Player GetActiveChar()
    {
        return _activeChar;
    }

    public void SetActiveChar(Player activeChar)
    {
        _activeChar = activeChar;
    }
}
