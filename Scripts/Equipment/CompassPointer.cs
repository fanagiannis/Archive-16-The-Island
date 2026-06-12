using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class CompassPointer : MeshInstance3D
{
	[Export] Node3D NorthPoint;
	[Export] float TurnSpeed = 5.0f;
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
    {
        Node3D parent = GetParentNode3D();
        if (parent == null) return;
        Vector3 globalTarget = -SceneManager.Instance.GetCurrentLevel().GetNorthPoint().GlobalPosition;
        Vector3 localTargetPosition = parent.ToLocal(globalTarget);
        
        localTargetPosition.Y = Position.Y;

        if (Position.IsEqualApprox(localTargetPosition)) return;

        Transform3D targetTransform = Transform.LookingAt(localTargetPosition, Vector3.Up);

        Quaternion currentRotation = Transform.Basis.GetRotationQuaternion();
        Quaternion targetRotation = targetTransform.Basis.GetRotationQuaternion();

        Quaternion smoothRotation = currentRotation.Slerp(targetRotation, (float)delta * TurnSpeed);

        Transform3D newTransform = Transform;
        newTransform.Basis = new Basis(smoothRotation);
        Transform = newTransform;
    }
}
