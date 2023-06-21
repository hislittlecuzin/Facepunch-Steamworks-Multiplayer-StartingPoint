using Godot;
using System;

public partial class PlayerPawn : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	Vector2 inputDir = Vector2.Zero;


    public void ReceiveInput(ClientToServerControllerInput input) { 
		switch (input) {
			case ClientToServerControllerInput.MoveForward:
				inputDir.Y = 1;
				break;
			case ClientToServerControllerInput.MoveBackward:
                inputDir.Y = -1;
                break;
			case ClientToServerControllerInput.MoveLeft:
                inputDir.X = -1;
                break;
			case ClientToServerControllerInput.MoveRight:
                inputDir.X = 1;
                break;
			case ClientToServerControllerInput.Shoot:
				break;
			default:
				break;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        //Vector2 inputDir = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
