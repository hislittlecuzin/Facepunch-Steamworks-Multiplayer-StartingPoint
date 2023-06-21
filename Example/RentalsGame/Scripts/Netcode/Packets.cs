using Godot;
using System;


public enum PacketType : byte {
    ClientToServerControllerInput,
    ServerToClientPlayerTransform,



    DebugPing
}

public struct DebugPingPacket {
    public PacketType packet;
    public float pointInTime;
    public char myAlias;
}

#region Client Control input
public enum ClientToServerControllerInput : byte { 
    MoveForward,
    MoveBackward,
    MoveLeft,
    MoveRight,
    LeanLeft,
    LeanRight,
    Shoot
}
public struct ClientToServerControllerInputPacket {
    public PacketType packet;
    public ClientToServerControllerInput input;
    public float pointInTime;
    /// <summary>
    /// Pressed = True when just pressed. 
    /// Pressed = False when just released.
    /// </summary>
    public bool pressed; 

}
#endregion

public struct ServerToClientPlayerTransformPacket {
    PacketType packet;

    public float positionX;
    public float positionY;
    public float rotationZ;

    public float rotationYaw;
    public float rotationPitch;
    public float rotationRoll;

    public float pointInTime;
}




public partial class Packets 
{
}
