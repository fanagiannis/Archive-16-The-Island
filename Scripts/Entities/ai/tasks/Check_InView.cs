using Godot;
using System;

[Tool]
public partial class Check_InView : BTCondition
{
    // Sets the display name of the node in the behavior tree graph
    public override string _GenerateName()
    {
        return "Check InView";
    }

    // Called once when the tree initialized
    public override void _Setup()
    {
        // e.g., get references or prepare variables
    }

    // Called when the task enters/starts executing
    public override void _Enter()
    {
    }

    // Called every frame/tick the behavior tree evaluates this node
    public override Status _Tick(double delta)
    {
        // Replace this with your line-of-sight / vision check logic.
        // You can access the agent running this tree using the 'Agent' property.
        bool isPlayerInView = CheckVisionLogic(); 

        if (isPlayerInView)
        {
            return Status.Success; // Condition met
        }
        else
        {
            return Status.Failure; // Condition not met
        }
    }

    private bool CheckVisionLogic()
    {
        // Your custom raycast or distance calculation goes here
        return true; 
    }
}
