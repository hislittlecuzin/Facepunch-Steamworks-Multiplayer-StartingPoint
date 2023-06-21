using Godot;
using System;
using Steamworks;
using Steamworks.Data;

public partial class Test2d : Node2D
{
	[Export]
	SpinBox playerCount;
	[Export]
	VBoxContainer playersContainer;

	[Export]
	PackedScene playerInLobbyNameThing;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {

	}

	public void _on_button_button_down() {
		int amountOfPlayers = (int)playerCount.Value;
		

        NetworkManager.instance.StartHost(amountOfPlayers);
	}

	public void _on_button_2_button_down() {
		GD.Print("Pinging server.... ? ");
		NetworkManager.instance.MessageServerBruv();

    }

	public void _on_button_3_button_down() {
		try
		{
			GD.Print("Pinging client.... ? ");
			NetworkManager.instance.MessageClientBruv();
		} catch (Exception ex)
		{
			GD.Print("Can't ping client UI: " + ex.ToString());
		}
    }

	public void _on_button_4_button_down() {
		GD.Print("Connections to server...");
		foreach (Connection c in NetworkManager.instance.currentServer.Connected) {
			GD.Print("Connected person:Name: " + c.ConnectionName + " UD: " + c.UserData + " ID: " + c.Id);
		}
	}

    public void AddPlayerToList(string username) {
		Label newPlayer = playerInLobbyNameThing.Instantiate<Label>();
		newPlayer.Text = username; // Steamworks.SteamClient.Name;
		playersContainer.AddChild(newPlayer);
		playersContainer.QueueSort();
	}

	public void RemovePlayerToList(string username) {
        foreach (Label label in playersContainer.GetChildren()) {
            if (label.Text == username) {
                playersContainer.RemoveChild(label);
				return;
            }
			
        }
    }

	public void ClearPlayerList() {
		foreach (Label label in playersContainer.GetChildren()) { 
			playersContainer.RemoveChild(label);
		}
	}
}
