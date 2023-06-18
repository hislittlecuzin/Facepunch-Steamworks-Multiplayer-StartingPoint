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
