using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public enum GameState : byte { 
	menu,
	lobby,
	pregame,
	game,
	postgame
}

public enum GameTeam : byte { 
	red,
	blue
}

public class PlayerProxyControl { 
	public Steamworks.SteamId steamid;
	PlayerPawn? playerPawn;
	GameTeam team;
}

public partial class GameMaster : Node {

	public static readonly int maxPlayerCount = 10;


	List<Steamworks.Friend> lobbyMembers = new List<Steamworks.Friend>(maxPlayerCount);
	GameState gameState = GameState.menu;
    List<PlayerProxyControl> playerProxyControl = new List<PlayerProxyControl>(maxPlayerCount);

    public void UpdateGameState(GameState newGameState) { 
		gameState = newGameState;

		switch (gameState) {
			case GameState.menu:
				break;
			case GameState.lobby:
                lobbyMembers = (List<Steamworks.Friend>)NetworkManager.instance.currentLobby.Value.Members.ToList();
				Steamworks.SteamId steamid;

                for (int i = 0; i < NetworkManager.instance.currentLobby.Value.Members.ToList().Count; i++) {
                    steamid = NetworkManager.instance.currentLobby.Value.Members.ToList()[i].Id;
                    playerProxyControl[i].steamid = steamid;
                }
				
                break;
			case GameState.pregame:
				break;
			case GameState.game:
				break;
			case GameState.postgame:
				break;
			default:
				break;
		}

	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		for (int i = 0; i < maxPlayerCount; i++) {
			playerProxyControl.Add(new PlayerProxyControl());
			// Spawn in nodes for each of these as pawns.
			// Set pawns hidden.
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
