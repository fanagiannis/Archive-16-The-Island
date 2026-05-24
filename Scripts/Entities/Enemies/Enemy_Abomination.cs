using Godot;
using GroveGames.BehaviourTree.Collections;
using System;

public partial class Enemy_Abomination : Enemy
{

    //EnemyAbominationData enemyData;

    AnimationTree animator;

    float walk_speed;

    float sprint_speed;
    
    public override void _Ready()

    {

        //base._Ready();
        SetupAI();
        animator = GetNode<AnimationTree>("Abomination/AnimationTree");
        
        Dead();

        if (enemyData != null)
        {
            walk_speed = enemyData.walk_speed;
            sprint_speed = enemyData.sprint_speed;
        }
        
        SetAgentSpeed();
        //UpdateSpeed();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateAnimator();
        //GD.Print(Velocity.Length());
    }


    public override void SetupAI()
    {
        base.SetupAI();
    }


    private void UpdateAnimator()
    {
        if (animator == null) return;
        if (animator == null) return;

        // Determine the target we WANT to reach
        float target_blend = (Velocity.Length() > 0.1f) ? current_speed : 0.0f;

        // Get the current blend position from the animator
        float actual_blend = (float)animator.Get("parameters/Movement/blend_position");

        // Smoothly interpolate from actual -> target
        // We multiply by delta * 10 to make it frame-rate independent
        float smoothed_blend = Mathf.Lerp(actual_blend, target_blend, (float)0.1f);

        // Apply it back
        animator.Set("parameters/Movement/blend_position", smoothed_blend);
        /*
        if(Velocity.Length()>0)
            animator.Set("parameters/Movement/blend_position",current_speed);
        else
            animator.Set("parameters/Movement/blend_position",0);*/
    }

    private void UpdateSpeed()
    {
        enemyBehavior.SetBlackboardValue("AgentSpeed",current_speed);
        UpdateAnimator();
    }

    public float GetCurentSpeed()
    {
        return current_speed;
    }
}

