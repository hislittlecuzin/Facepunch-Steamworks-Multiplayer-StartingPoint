using Godot;
using Steamworks.Data;
using Steamworks;
using System;
using System.Runtime.InteropServices;

/// <summary>
/// Used by connections
/// </summary>
public class SteamConnectionManager : ConnectionManager
{
    public override void OnConnected(ConnectionInfo info)
    {
        base.OnConnected(info);
        GD.Print("ConnectionOnConnected");
    }

    public override void OnConnecting(ConnectionInfo info)
    {
        base.OnConnecting(info);
        GD.Print("ConnectionOnConnecting");
    }

    public override void OnDisconnected(ConnectionInfo info)
    {
        base.OnDisconnected(info);
        GD.Print("ConnectionOnDisconnected");
    }

    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
    {
        base.OnMessage(data, size, messageNum, recvTime, channel);
        // Message received from socket server, delegate to method for processing
        //SteamManager.Instance.ProcessMessageFromSocketServer(data, size);
        GD.Print("Connection Got A Message " + data);
        //ProcessMessage(data, size);


        byte[] pingArray = new byte[sizeof(byte)];
        Marshal.Copy(data, pingArray, 0, 1);
        //GD.Print("MyPingArray Check");
        //GD.Print(pingArray[0]);
        switch (pingArray[0])
        {
            case (byte)PacketType.DebugPing:
                GD.Print("Ping communication from server");
                break;
            case (byte)PacketType.ServerToClientPlayerTransform:
                GD.Print("Transform communication from server");
                break;
            default:
                GD.Print("Defaulted... Datum: " + pingArray[0] + " //From server");
                break;
        }


    }



    public void MessageServerData(IntPtr datum, int size) {
        try {
            //DebugPingPacket pingPacket = new DebugPingPacket();
            //pingPacket.packet = PacketType.DebugPing;
            //pingPacket.pointInTime = 5.0f;
            //pingPacket.myAlias = 'h';
            //IntPtr pingPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pingPacket));
            //Marshal.StructureToPtr(pingPacket, pingPtr, false);
            //currentConnectionToServer.SendMessageToSocketServer(myData);
            //IntPtr intPtrMessage = new IntPtr(1);
            //int sizeOfMessage = 0;
            //int sizeOfMessage = Marshal.SizeOf<DebugPingPacket>(pingPacket);
            Result r = NetworkManager.instance.currentConnectionToServer.Connection.SendMessage(datum, size, SendType.Reliable);
        }
        catch (Exception e) {

            GD.Print("Failed to message server... " + e.Message);
        }
    }
}
