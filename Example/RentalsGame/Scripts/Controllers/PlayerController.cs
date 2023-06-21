using System;
using System.Runtime.InteropServices;
using Godot;

public partial class PlayerController : Node {
    public override void _Process(double delta) { 
        //base._Process(delta);

    }


    void MessageServerControllerInput(ClientToServerControllerInput input, bool pressed) {
        ClientToServerControllerInputPacket clientToServerControllerInputPacket = new ClientToServerControllerInputPacket();
        
        clientToServerControllerInputPacket.packet = PacketType.ClientToServerControllerInput;
        clientToServerControllerInputPacket.input = input;
        clientToServerControllerInputPacket.pointInTime = 0.0f;
        clientToServerControllerInputPacket.pressed = pressed;

        IntPtr ptrData = Marshal.AllocHGlobal(Marshal.SizeOf(clientToServerControllerInputPacket));
        Marshal.StructureToPtr(clientToServerControllerInputPacket, ptrData, false);//

        int sizeOfMessage = Marshal.SizeOf<ClientToServerControllerInputPacket>(clientToServerControllerInputPacket);
        NetworkManager.instance.currentConnectionToServer.MessageServerData(ptrData, sizeOfMessage);

        ClientToServerControllerInputPacket clientToServerControllerInputPacket1 = (ClientToServerControllerInputPacket)Marshal.PtrToStructure(ptrData, typeof(ClientToServerControllerInputPacket));
        Marshal.FreeHGlobal(ptrData);
    }
    

    public override void _Input(InputEvent @event) {
        if (@event.IsActionPressed("MoveForward")) {
            GD.Print("MoveForward");
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                
                MessageServerControllerInput(ClientToServerControllerInput.MoveForward, true);
            }
        }
        else if (@event.IsActionReleased("MoveForward")) {
            GD.Print("Not MoveForward");
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveForward, false);
            }
        }
        if (@event.IsActionPressed("MoveBackward")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveBackward, true);
            }
        }
        else if (@event.IsActionReleased("MoveBackward")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveBackward, false);
            }
        }
        if (@event.IsActionPressed("MoveLeft")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveLeft, true);
            }
        }
        else if (@event.IsActionReleased("MoveLeft")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveLeft, false);
            }
        }
        if (@event.IsActionPressed("MoveRight")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveRight, true);
            }
        }
        else if (@event.IsActionReleased("MoveRight")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.MoveRight, false);
            }
        }
        if (@event.IsActionPressed("Shoot")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.Shoot, true);
            }
        }
        else if (@event.IsActionReleased("Shoot")) {
            if (NetworkManager.instance.serverStatus == ServerStatus.Client) {
                MessageServerControllerInput(ClientToServerControllerInput.Shoot, false);
            }
        }
    }
}
