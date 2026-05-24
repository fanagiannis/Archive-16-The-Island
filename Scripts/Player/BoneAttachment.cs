using Godot;
using System;

public partial class BoneAttachment : Node3D
{
	[Export] public Skeleton3D Skeleton;
	[Export] public Node3D NodeToAttach; // Assign the node you want to attach (e.g., Camera3D)

    [Export] public string BoneName = "head"; // Name of the bone to attach to

    public override void _Ready()
    {
        AttachNodeToBone();
    }

    private void AttachNodeToBone()
    {
        // Create a BoneAttachment3D node
        BoneAttachment3D boneAttachment = new BoneAttachment3D();

        // Set the bone name
        boneAttachment.BoneName = BoneName;

        // Add the BoneAttachment3D to the Skeleton3D
        Skeleton.AddChild(boneAttachment);

        // Attach the node to the BoneAttachment3D
        boneAttachment.AddChild(NodeToAttach);
    }
}
