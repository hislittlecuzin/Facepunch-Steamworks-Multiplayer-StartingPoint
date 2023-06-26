# Facepunch-Steamworks-Multiplayer-StartingPoint

I was using Godot cuz.... (Godot Mono/.NET 4.0.3 Stable)
This should be the bare minimum for a C# project.

If you don't use Godot, make sure you download the release (or compile it yourself IG)
https://github.com/Facepunch/Facepunch.Steamworks/releases

Then in Visual Studio right click the PROJECT, not the solution. "Add->Project Reference..." then find the 
Facepunch.Steamworks.2.3.2\net46\Facepunch.Steamworks.Win64.dll
AND
Facepunch.Steamworks.2.3.2\Unity\redistributable_bin\win64\steam_api64.dll

There's lobbies and there's servers.
Lobbies are just how ya like find each other on steam. 
Then Servers are the actual game's netcode.
Lobbies help you connect each other & you can send maybe some data. I'm not sure, I spend like too many days figuring this out. I was going to make a youtube video but ... fatigue.

Then you can make a server. You REALLY do NOT have to use Steamwork's socket... but you can. 
It relays through Steam's servers.
Jerry never updated the Facepunch Steamworks API documentation/wiki so if Steam made it so P2P doesn't share IP then maybe that'll work too. 

After making this poing in the project where you can talk to clients, I tried converting stuff, and for some reason I can't get packets to be consistently read... idk why. 

So make sure you organize your packets how I designed them. 
I tried to make as much stuff easy to do as possible for JUST MULTIPLAYER. 

I didn't do a lot of stuff like ... disconnecting & stuff. 
Many of the buttons are ticked to be "One Shot" which MIGHT make them only execute once... idk. 

Have... Fun? 




# more infos
Godot's gameobject hierarchy is a linked list or something so when I do like "add node" or something, that's how ya spawn shizz.
Also The NetworkManager.cs is loaded in the autoload cuz that's how Godot does singletons. 
In godot... go to the top left where "file, edit" etc would be dropdowns and:
Project->Project Settings->Autoload


Despite Jerry saying Facepunch Steamworks is a "C# reimagining" or whatever of Steamworks.NET, the sockets are VERY C level coded.
This means you're going to be dealing with data like "pointer arrays" which is some wizard shit they did in the 70s or something. 
I tried to make itt so you don't have to do that & stay in the .... I lost my train of though.
TL;DR:
Name your packets in the enumeration (enum can be up to 256 different packets... no need for numbas assignment)
And then make different packets. Then you perhaps won't have memory leaks 'n stuff
All packets HAVE to be structures... as far as I can tell. 
You can do byte arrays but that sounded hard... so I didn't do them. 

# If you have inconsistent data transmissions/etc
you have to malloc then point... this is very "C" level code so if you don't understand it, just like take this method and figure out wtf you can do IG....
I've been drunk for like 3 days so I'm not going to be too useful in editing stuff nicely. 

```cs
//ClientToServerControllerInput is an enum (byte)
//ClientToServerControllerInputPacket is a struct... guess the types, cuz they're enum(byte), enum(byte), float, bool in that order as you can see by the psuedo constructor. 
void MessageServerControllerInput(ClientToServerControllerInput input, bool pressed) {
        ClientToServerControllerInputPacket clientToServerControllerInputPacket = new ClientToServerControllerInputPacket();
        
        clientToServerControllerInputPacket.packet = PacketType.ClientToServerControllerInput;
        clientToServerControllerInputPacket.input = input;
        clientToServerControllerInputPacket.pointInTime = 0.0f;
        clientToServerControllerInputPacket.pressed = pressed;

//Malloc
        IntPtr ptrData = Marshal.AllocHGlobal(Marshal.SizeOf(clientToServerControllerInputPacket));
//Point to datum
        Marshal.StructureToPtr(clientToServerControllerInputPacket, ptrData, false);//

        int sizeOfMessage = Marshal.SizeOf<ClientToServerControllerInputPacket>(clientToServerControllerInputPacket);
        NetworkManager.instance.currentConnectionToServer.MessageServerData(ptrData, sizeOfMessage);
        //Marshal.write

        ClientToServerControllerInputPacket clientToServerControllerInputPacket1 = (ClientToServerControllerInputPacket)Marshal.PtrToStructure(ptrData, typeof(ClientToServerControllerInputPacket));
        GD.Print("Testing SendingDatum Packet not networked: Pressed?: " + clientToServerControllerInputPacket1.pressed + " Button: " + clientToServerControllerInputPacket1.input.ToString() + " Packet: " + clientToServerControllerInputPacket1.packet.ToString());
        GD.Print("Real Data: Pressed?: " + clientToServerControllerInputPacket.pressed + " Button: " + clientToServerControllerInputPacket.input.ToString() + " Packet: " + clientToServerControllerInputPacket.packet.ToString());
        //idk if you have to free it but I'm not going to delete this line of code now that it's here...
        Marshal.FreeHGlobal(ptrData);
    }
//end of code block
```

# I want to die
When you take in data in your receiver, you gotta convert data like this:

```
StartMatchPacket foo = new StartMatchPacket();
foo.packet = PacketType.StartMatch;
foo.gameMode = 1;
foo.teamAssignment = 69;
IntPtr ptrData = Marshal.AllocHGlobal(Marshal.SizeOf(foo));
Marshal.StructureToPtr(foo, ptrData, false);
int size = Marshal.SizeOf<StartMatchPacket>(foo);

StartMatchPacket bar;

bar = (StartMatchPacket)Marshal.PtrToStructure(ptrData, typeof(StartMatchPacket));

//Access sshtuff
GD.Print("FUCKING " + bar.teamAssignment.ToString());
```
It took me like 6 hours to figure out the 
```
bar = (StartMatchPacket)Marshal.PtrToStructure(ptrData, typeof(StartMatchPacket));
```
line
# example/rentalsgame
This folder has some more shizz I was working on for like fps. 

You can use that texture IG.... It's a picture of my desk with water damage or sumn. 
If you don;t use my drunk boiler plate code there, you can just delete that folda. 

If you appreciate me or love me or something , plz giv money... am poor. idk how you would but ig I have a game on steam that's meh called Holy Journey of Salvation. And a youtube channel so if I release other games without dying, you can maybe buy one of those. 
