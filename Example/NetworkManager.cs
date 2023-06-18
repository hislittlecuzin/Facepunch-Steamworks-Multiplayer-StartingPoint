using Godot;
using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;
using System;
using System.Diagnostics;//
using System.Drawing;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

public enum packetType : byte{ 
    ping = 0,
    location = 1,
    rotation = 2,
    nice = 69
}

public struct PingPacket {
    public packetType packet;
    public int time;
    public char myAlias;
}

public partial class NetworkManager : Node
{
    public static NetworkManager instance;
    
    public Lobby? currentLobby;
    public MySteamGameServer currentServer;
    SteamConnectionManager currentConnectionToServer;

    ushort serverPort = 21893;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        #region demonstration of intptr and bytes
        GD.Print("Size of int: " + sizeof(int));
        GD.Print("Size of byte: " + sizeof(byte));
        int i = 9999;
        byte value1 = (byte)i;
        GD.Print("Size of int byte: " + System.Runtime.InteropServices.Marshal.SizeOf(value1) + " // " + value1 + " // " + i);

        IntPtr value2 = (IntPtr)i;
        GD.Print("Size of int ptr: " + System.Runtime.InteropServices.Marshal.SizeOf(value2) + " // " + value2 + " // " + i);
        string helloMaybe = "Hello world";
        IntPtr value3 = System.Runtime.InteropServices.Marshal.StringToHGlobalAuto(helloMaybe);
        GD.Print("Size of int ptr: " + System.Runtime.InteropServices.Marshal.SizeOf(value3) + " // " + value3 + " // " + helloMaybe);
        GD.Print("int ptr to string marshal: " + Marshal.PtrToStringAuto(value3));
        GD.Print("size of string? " + System.Text.ASCIIEncoding.ASCII.GetByteCount(helloMaybe) + " OR.. val: " + Marshal.SizeOf(helloMaybe.Length)); // + sizeof(helloMaybe.Length)
        IntPtr myptr = new IntPtr();
        //myptr.
        GD.Print("Ptr iteration:");
        GD.Print(Marshal.PtrToStringAuto(value3));
        GD.Print(Marshal.PtrToStringAuto(value3 + 2));
        GD.Print(value3.GetType());
        byte[] managedArray = new byte[helloMaybe.Length];

        PingPacket pingPacket = new PingPacket();
        pingPacket.packet = packetType.nice;
        pingPacket.time = 5;
        pingPacket.myAlias = 'h';
        IntPtr pingPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pingPacket));//  //Marshal.StructureToPtr pingPacket;
        byte[] pingArray = new byte[sizeof(byte)];
        Marshal.StructureToPtr(pingPacket, pingPtr, false);


        Marshal.Copy(pingPtr, pingArray, 0, 1);
        GD.Print("MyPingArray Check");
        GD.Print(pingArray[0]);

        Marshal.Copy(value3, managedArray, 0, helloMaybe.Length);
        GD.Print("Managed string");
        GD.Print(managedArray[0] == 72);
        GD.Print(Convert.ToChar(managedArray[0]));

        //Marshal.Copy(value3, managedArray, 2, 3);
        GD.Print("Managed string redo");
        GD.Print(Convert.ToChar(managedArray[1]));

        //Marshal.Copy(value3, managedArray, 4, 5);
        GD.Print("Managed string redo");
        GD.Print(Convert.ToChar(managedArray[2]));
        GD.Print(Convert.ToChar(managedArray[4]));
        GD.Print(Convert.ToChar(managedArray[6]));

        #endregion
        //Marshal.PtrToStructure

        instance = this;
        //NetworkedMultiplayerENet.New();
        try
        {
            Steamworks.SteamClient.Init(480, true); // this is my game's code... does it matter I leaked it? 2478900
            GD.Print(SteamClient.Name);
        }
        catch (Exception e)
        {
            GD.Print("Failed to initialize steamworks.\n" + e.Message);
        }


        SteamMatchmaking.OnLobbyCreated         += SteamMatchmaking_OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered         += SteamMatchmaking_OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined    += SteamMatchmaking_OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave     += SteamMatchmaking_OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite          += SteamMatchmaking_OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated     += SteamMatchmaking_OnLobbyGameCreated;
        SteamFriends.OnGameLobbyJoinRequested   += SteamFriends_OnGameLobbyJoinRequested;

    }

    #region test communication methods
    public void MessageServerBruv() {
        
        try
        {
            PingPacket pingPacket = new PingPacket();
            pingPacket.packet = packetType.nice;
            pingPacket.time = 5;
            pingPacket.myAlias = 'h';
            IntPtr pingPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pingPacket));
            Marshal.StructureToPtr(pingPacket, pingPtr, false);
            //currentConnectionToServer.SendMessageToSocketServer(myData);
            //IntPtr intPtrMessage = new IntPtr(1);
            //int sizeOfMessage = 0;
            int sizeOfMessage = Marshal.SizeOf<PingPacket>(pingPacket);
            currentConnectionToServer.Connection.SendMessage(pingPtr, sizeOfMessage, SendType.Reliable);
        }
        catch (Exception e)
        {

            GD.Print("Failed to message server... " + e.Message);
        }
    }

    public void MessageClientBruv()
    {
        
        try {
            PingPacket pingPacket = new PingPacket();
            pingPacket.packet = packetType.rotation;
            pingPacket.time = 5;
            pingPacket.myAlias = 'h';
            IntPtr pingPtr = Marshal.AllocHGlobal(Marshal.SizeOf(pingPacket));
            Marshal.StructureToPtr(pingPacket, pingPtr, false);
            //currentConnectionToServer.SendMessageToSocketServer(myData);
            //IntPtr intPtrMessage = new IntPtr(1);
            //int sizeOfMessage = 0;
            int sizeOfMessage = Marshal.SizeOf<PingPacket>(pingPacket);
            //currentConnectionToServer.Connection.SendMessage(pingPtr, sizeOfMessage, SendType.Reliable);



            //string helloWorld = "Hello world!!";
            //byte[] datum = Encoding.ASCII.GetBytes(helloWorld);
            //byte[] myData = new byte[0];
            //IntPtr myPtr = IntPtr.Zero;

            // Loop to only send messages to socket server members who are not the one that sent the message
            for (int i = 0; i < currentServer.Connected.Count; i++)
            {
                currentServer.Connected[i].SendMessage(pingPtr, sizeOfMessage);
            }
        }
        catch (Exception e)
        {
            GD.Print("Unable to relay socket server message... " + e.Message);
        }
    }
    #endregion

    #region On Lobby created and game created
    /// <summary>
    /// When lobby was created but first of 2
    /// </summary>
    /// <param name="_result"></param>
    /// <param name="_lobby"></param>
    private void SteamMatchmaking_OnLobbyCreated(Result _result, Lobby _lobby)
    {
        if (_result != Result.OK)
        {
            GD.Print("lobby was not created");
            return;
        }
        _lobby.SetPublic();
        _lobby.SetJoinable(true);
        _lobby.SetGameServer(_lobby.Owner.Id);
        GD.Print("lobby created: " + _lobby.Id);
        //NetworkTransmission.instance.AddMeToDictionaryServerRPC(SteamClient.SteamId, "FakeSteamName", NetworkManager.Singleton.LocalClientId);

    }

    /// <summary>
    /// When the lobby is created. second of 2
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    /// <param name="arg3"></param>
    /// <param name="arg4"></param>
    private void SteamMatchmaking_OnLobbyGameCreated(Lobby arg1, uint arg2, ushort arg3, SteamId arg4) {
        GD.Print("Lobby was created");
        //GameManager.instance.SendMessageToChat($"Lobby was created", NetworkManager.Singleton.LocalClientId, true);
    }
    #endregion

    #region joining lobbies
    /// <summary>
    /// When a player actually enters the lobby you are already in.
    /// </summary>
    /// <param name="_lobby"></param>
    /// <param name="friend"></param>
    private void SteamMatchmaking_OnLobbyMemberJoined(Lobby _lobby, Friend friend)
    {
        GetNode<Test2d>("/root/Node2D").AddPlayerToList(friend.Name);
        GD.Print("member join");
    }

    /// <summary>
    /// When YOU join a lobby. 
    /// </summary>
    /// <param name="_lobby"></param>
    private void SteamMatchmaking_OnLobbyEntered(Lobby _lobby)
    {
        //if (NetworkManager.Singleton.IsHost)
        //{
        //    return;
        //}
        StartClient(currentLobby.Value.Owner.Id);
        foreach (Friend friend in _lobby.Members)
        {
            GetNode<Test2d>("/root/Node2D").AddPlayerToList(friend.Name);
        }
        if (_lobby.IsOwnedBy(SteamClient.SteamId) == false)
        {
            currentConnectionToServer = SteamNetworkingSockets.ConnectRelay<SteamConnectionManager>(_lobby.Owner.Id, serverPort);

        }
    }
    #endregion

    #region Client/game methods
    /// <summary>
    /// Called by user/program. This is what I used to create the lobby AND the server.
    /// </summary>
    /// <param name="_maxMembers"></param>
    public async void StartHost(int _maxMembers)
    {
        //NetworkManager.Singleton.OnServerStarted += Singleton_OnServerStarted;
        //NetworkManager.Singleton.StartHost();
        //GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(_maxMembers);
        StartServer();
    }

    /// <summary>
    /// Used to create the steam server. Or alternate server...
    /// </summary>
    public void StartServer()
    {
        if (currentLobby != null)
        {
            currentLobby.Value.SetGameServer(currentLobby.Value.Owner.Id);
        }
        currentServer = SteamNetworkingSockets.CreateRelaySocket<MySteamGameServer>(serverPort);
        //currentServer.Receive();
        GD.Print("Started Game Server");
        GD.Print(currentServer.ToString());
    }

    /// <summary>
    /// Fekin nuthn
    /// </summary>
    /// <param name="_sId"></param>
    public void StartClient(SteamId _sId)
    {
        //NetworkManager.Singleton.OnClientConnectedCallback += Singleton_OnClientConnectedCallback;
        //NetworkManager.Singleton.OnClientDisconnectCallback += Singleton_OnClientDisconnectCallback;
        //transport.targetSteamId = _sId;
        //GameManager.instance.myClientId = NetworkManager.Singleton.LocalClientId;
        //if (NetworkManager.Singleton.StartClient())
        //{
        //    Debug.Log("Client has started");
        //}/
    }

    /// <summary>
    /// Would be called by the user/game/program. Use to disconnect from a lobby or sumn.
    /// </summary>
    public void Disconnected()
    {
        currentLobby?.Leave();
        //if (NetworkManager.Singleton == null)
        //{
        //    return;
        //}
        //if (NetworkManager.Singleton.IsHost)
        //{
        //    NetworkManager.Singleton.OnServerStarted -= Singleton_OnServerStarted;
        //}
        //else
        //{
        //    NetworkManager.Singleton.OnClientConnectedCallback -= Singleton_OnClientConnectedCallback;
        //}
        //NetworkManager.Singleton.Shutdown(true);
        //GameManager.instance.ClearChat();
        //GameManager.instance.Disconnected();
        GD.Print("disconnected");
    }
    #endregion

    #region idfk what to do with tis stuff
    private void SteamMatchmaking_OnLobbyInvite(Friend _steamId, Lobby arg2)
    {
        GD.Print($"Invite from {_steamId.Name}");
    }
    private async void SteamFriends_OnGameLobbyJoinRequested(Lobby _lobby, SteamId _steamId)
    {
        RoomEnter joinedLobby = await _lobby.Join();
        if (joinedLobby != RoomEnter.Success)
        {
            GD.Print("Failed to create lobby");
        }
        else
        {
            currentLobby = _lobby;
            //GameManager.instance.ConnectedAsClient();
            GD.Print("Joined Lobby");
        }
    }
    #endregion

    /// <summary>
    /// When someone else leaves the lobby
    /// </summary>
    /// <param name="_lobby"></param>
    /// <param name="_steamId"></param>
    private void SteamMatchmaking_OnLobbyMemberLeave(Lobby _lobby, Friend _steamId)
    {
        GD.Print("member leave");
        if (_lobby.IsOwnedBy(SteamClient.SteamId)) {
            GD.Print("You are now the host.");
        }
        GetNode<Test2d>("/root/Node2D").RemovePlayerToList(_steamId.Name);
        //GameManager.instance.SendMessageToChat($"{_steamId.Name} has left", _steamId.Id, true);
        //NetworkTransmission.instance.RemoveMeFromDictionaryServerRPC(_steamId.Id);
    }

    //Main loop. Runs in main thread iirc.
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
        try
        {
            currentServer.Receive();
        }
        catch { 
        }
        try
        {
            currentConnectionToServer.Receive();
        }
        catch { }
    }

}

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
            case (byte)packetType.ping:
                GD.Print("Ping communication from: " + connection.Id.ToString());
                break;
            case (byte)packetType.location:
                GD.Print("location communication from: " + connection.Id.ToString());
                break;
            case (byte)packetType.rotation:
                GD.Print("rotation communication from: " + connection.Id.ToString());
                break;
            case (byte)packetType.nice:
                GD.Print("nice communication from: " + connection.Id.ToString());
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

    public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel) {
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
            case (byte)packetType.ping:
                GD.Print("Ping communication from server");
                break;
            case (byte)packetType.location:
                GD.Print("location communication from server");
                break;
            case (byte)packetType.rotation:
                GD.Print("rotation communication from server");
                break;
            case (byte)packetType.nice:
                GD.Print("nice communication from server");
                break;
            default:
                GD.Print("Defaulted... Datum: " + pingArray[0] + " //From server");
                break;
        }


    }

    //Unused:
    void ProcessMessage(IntPtr messageIntPtr, int dataBlockSize) {
        try {
            byte[] message = new byte[dataBlockSize];
            System.Runtime.InteropServices.Marshal.Copy(messageIntPtr, message, 0, dataBlockSize);
            string messageString = System.Text.Encoding.UTF8.GetString(message);

            // Do something with received message
            SendMessageToSocketServer(message);
        }
        catch {
            GD.Print("Unable to process message from socket server");
        }
    }
    //Unused:
    public bool SendMessageToSocketServer(byte[] messageToSend) {
        try {
            // Convert string/byte[] message into IntPtr data type for efficient message send / garbage management
            int sizeOfMessage = messageToSend.Length;
            IntPtr intPtrMessage = System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeOfMessage);
            System.Runtime.InteropServices.Marshal.Copy(messageToSend, 0, intPtrMessage, sizeOfMessage);
            Result success = Connection.SendMessage(intPtrMessage, sizeOfMessage, SendType.Reliable);
            if (success == Result.OK) {
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage); // Free up memory at pointer
                return true;
            }
            else {
                // RETRY
                Result retry = Connection.SendMessage(intPtrMessage, sizeOfMessage, SendType.Reliable);
                System.Runtime.InteropServices.Marshal.FreeHGlobal(intPtrMessage); // Free up memory at pointer
                if (retry == Result.OK)
                {
                    return true;
                }
                return false;
            }
        }
        catch (Exception e) {
            GD.Print(e.Message);
            GD.Print("Unable to send message to socket server");
            return false;
        }
    }

}
