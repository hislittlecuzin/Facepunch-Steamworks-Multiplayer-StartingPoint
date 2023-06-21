using Godot;
using Steamworks.Data;
using Steamworks;
using System;
using System.Runtime.InteropServices;


/// <summary>
/// Used by Host
/// </summary>
public class MySteamGameServer : SocketManager {

    public readonly string serverName = "This has a name!";
    public override void OnConnecting(Connection connection, ConnectionInfo data)
    {
        base.OnConnecting(connection, data);
        connection.Accept();
        GD.Print($"{data.Identity.SteamId} is connecting");
    }

    public override void OnConnected(Connection connection, ConnectionInfo data)
    {
        base.OnConnected(connection, data);
        GD.Print($"{data.Identity.SteamId} has joined the game");
        //connection.SendMessage()
        //IntPtr ptr = new IntPtr()
        //connection.SendMessage("Here's some data", SendType.Reliable);
    }

    public override void OnDisconnected(Connection connection, ConnectionInfo data)
    {
        base.OnDisconnected(connection, data);
        GD.Print($"{data.Identity.SteamId} is out of here");
    }

    public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel) {
        base.OnMessage(connection, identity, data, size, messageNum, recvTime, channel);


        //PingPacket pingPacket = new PingPacket();
        //pingPacket.packet = packetType.nice;
        //pingPacket.time = 5;
        //pingPacket.myAlias = 'h';
        //IntPtr pingPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pingPacket));//  //Marshal.StructureToPtr pingPacket;
        //Marshal.StructureToPtr(pingPacket, pingPtr, false);

        byte[] pingArray = new byte[sizeof(byte)];
        Marshal.Copy(data, pingArray, 0, 1);
        //GD.Print("MyPingArray Check");
        //GD.Print(pingArray[0]);
        switch (pingArray[0]) {
            case (byte)PacketType.DebugPing:
                GD.Print("Ping communication from: " + connection.Id.ToString());
                break;
            case (byte)PacketType.ClientToServerControllerInput:
                GD.Print("Input communication from: " + connection.Id.ToString());
                break;
            default:
                GD.Print("Defaulted... Datum: " + pingArray[0] + " //From: " + connection.Id.ToString());
                break;
        }

        GD.Print($"We got a message from {identity}!");
        //NetworkManager.instance.MessageClientBruv();

        // Send it right back
        //connection.SendMessage(data, size, SendType.Reliable);
    }


    //Unused:
    public void RelaySocketMessageReceived(IntPtr message, int size, uint connectionSendingMessageId)
    {
        try
        {
            // Loop to only send messages to socket server members who are not the one that sent the message
            for (int i = 0; i < Connected.Count; i++)
            {
                if (Connected[i].Id != connectionSendingMessageId)
                {
                    Result success = Connected[i].SendMessage(message, size);
                    if (success != Result.OK)
                    {
                        Result retry = Connected[i].SendMessage(message, size);
                    }
                }
            }
        }
        catch
        {
            GD.Print("Unable to relay socket server message");
        }
    }
}
