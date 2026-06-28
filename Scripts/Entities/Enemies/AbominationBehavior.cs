using Godot;
using PolarBears.PlayerControllerAddon;
using System;

public partial class AbominationBehavior : EnemyBehavior
{
    [Export] Enemy enemyBody;
    [Export] float attackDistance = 1.5f;
    [Export] float attackDamage = 10.0f;
    [Export] float attackCooldown = 1.5f;

    private enum AIState { Hunting, Fleeing, Hiding }
    private AIState currentState = AIState.Hunting;
    
    
    private bool isDamaged = false;
    private bool isBlinded = false;
    private double fleeTimer = 0.0;
    private double attackTimer = 0.0;
    private Vector3 fleeTargetPosition;
    private double pathUpdateTimer = 0.0;
    private const float PathUpdateInterval = 0.25f;
    public event Action OnBodyDamaged;

    public override void _Ready()
    {
        body = enemyBody as CharacterBody3D;

        if (body == null)
        {
            GD.PrintErr("FLEEING AGENT: enemyBody is not a CharacterBody3D!");
        }

        currentState = AIState.Hunting;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (body == null || enemyNavigation == null) return;
        
        // Decrease attack timer safely
        if (attackTimer > 0) attackTimer -= delta;

        if (enemyBody is Enemy_Abomination abomination)
        {
            bool bodyDamaged = abomination.GetIsDamaged();
            bool bodyBlinded = abomination.GetIsBlinded();

            if (!bodyDamaged && !bodyBlinded && (isDamaged || isBlinded))
            {
                isDamaged = false;
                isBlinded = false;
                currentState = AIState.Hunting;
            }
            else
            {
                if (bodyDamaged && !isDamaged)
                {
                    isDamaged = true;
                    OnBodyDamaged?.Invoke(); 
                    TriggerFleeState();
                }
                
                if (bodyBlinded && !isBlinded)
                {
                    isBlinded = true;
                    TriggerFleeState();
                }
            }
        }

        switch (currentState)
        {
            case AIState.Hunting:
                ProcessHunting(delta);
                break;
            case AIState.Fleeing:
                ProcessFleeing(delta);
                break;
            case AIState.Hiding:
                ProcessHiding(delta);
                break;
        }
    }

    private void ProcessHunting(double delta)
    {
        float distanceToPlayer = body.GlobalPosition.DistanceTo(SceneManager.Instance.GetPlayerPosition());
        if (distanceToPlayer <= attackDistance)
        {
            TryDamagePlayer();
            body.Velocity = Vector3.Zero;
            body.MoveAndSlide();
            return;
        }

        if(distanceToPlayer>=500 || enemyNavigation.IsTargetReachable()==false)
        {
            FailsafeRelocate(body,delta);
        }
        
        pathUpdateTimer += delta;
        if (pathUpdateTimer >= PathUpdateInterval)
        {
            if (enemyNavigation.TargetPosition.DistanceSquaredTo(SceneManager.Instance.GetPlayerPosition()) > 0.5f)
            {
                enemyNavigation.TargetPosition = SceneManager.Instance.GetPlayerPosition();
            }
            pathUpdateTimer = 0.0;
        }
        
        Enemy_Abomination abom = enemyBody as Enemy_Abomination;
        abom.Walk();
        abom.UpdateAnimator();
        MoveTowardsTarget(abom.GetCurentSpeed(), delta);
    }

    private void ProcessFleeing(double delta)
    {
        fleeTimer -= delta;
        
        // Reached target, got close enough, or failsafe timer ran out
        if (fleeTimer <= 0 || enemyNavigation.IsNavigationFinished() || body.GlobalPosition.DistanceSquaredTo(fleeTargetPosition) < 2.0f)
        {
            DisableAgent();
            return;
        }

        Enemy_Abomination abom = enemyBody as Enemy_Abomination;
        abom.Run();
        abom.UpdateAnimator();
        MoveTowardsTarget(abom.GetCurentSpeed(), delta);
    }

   
    
    private void ProcessHiding(double delta)
    {
        Vector3 vel = body.Velocity;
        vel.X = 0;
        vel.Z = 0;
        if (!body.IsOnFloor())
        {
            vel.Y -= 9.8f * (float)delta;
        }
        body.Velocity = vel;
        body.MoveAndSlide();

        float distanceToPlayer = body.GlobalPosition.DistanceTo(SceneManager.Instance.GetPlayerPosition());
        if (distanceToPlayer < 8.0f)
        {
            currentState = AIState.Hunting;
        }
    }

    private void MoveTowardsTarget(float speed, double delta)
    {
        if (enemyNavigation.IsNavigationFinished()) return;

        Vector3 nextPathPosition = enemyNavigation.GetNextPathPosition();
        Vector3 currentPosition = body.GlobalPosition;
        
        Vector3 direction = (nextPathPosition - currentPosition);
        direction.Y = 0;
        direction = direction.Normalized();
        
        Vector3 currentVelocity = body.Velocity;
        currentVelocity.X = direction.X * speed;
        currentVelocity.Z = direction.Z * speed;

        if (!body.IsOnFloor())
        {
            currentVelocity.Y -= 9.8f * (float)delta;
        }

        body.Velocity = currentVelocity;
        body.MoveAndSlide();

        if (direction.LengthSquared() > 0.01f)
        {
            Vector3 lookTarget = body.GlobalPosition + direction;
            lookTarget.Y = body.GlobalPosition.Y;
            body.LookAt(lookTarget, Vector3.Up);
        }
    }

    private void TriggerFleeState()
    {
        currentState = AIState.Fleeing;
        fleeTimer = 5.0f; 

        Vector3 dirAwayFromPlayer = (body.GlobalPosition - SceneManager.Instance.GetPlayerPosition()).Normalized();
        dirAwayFromPlayer.Y = 0;

        fleeTargetPosition = body.GlobalPosition + (dirAwayFromPlayer * 25.0f);
        
        enemyNavigation.TargetPosition = fleeTargetPosition;
    }

    private void DisableAgent()
    {
        // 1) Teleport out of map
        body.GlobalPosition = new Vector3(0, -1000f, 0);

        // 2) Make invisible
        enemyBody.Visible = false;

        // Reset state for when EnemyManager respawns it
        currentState = AIState.Hunting;
        
        // Disable physics processing so it rests quietly in the void
        enemyBody.ProcessMode = ProcessModeEnum.Disabled;

        // 3) Emit signal to EnemyManager to start the respawn timer
        if (enemyBody is Enemy_Abomination abomination)
        {
            abomination.EmitSignal(Enemy_Abomination.SignalName.Disabled);
        }
    }

    private void TryDamagePlayer()
    {
        if (attackTimer > 0) return;

        if (SceneManager.Instance.GetPlayer() is PlayerController playerCtrl)
        {
            GD.Print($"FLEEING AGENT: Damaged player for {attackDamage}!");
            // playerCtrl.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
        }
    }

    // --- Boilerplate Overrides ---
    public override void SetBlackboardValue(string key, Variant value) { }
    public override void SetTargetPosition(Vector3 targetPosition) { enemyNavigation.TargetPosition = targetPosition; }
    public override Vector3 MoveToTarget(Transform3D _agentTransform, float _agentspeed) { return Vector3.Zero; }
    public override void DebugAction(string test) { GD.Print(test); }
    public override void SetSpeed(float speed) { }
    public override void SetUpBehavior(Transform3D agentTransform, float speed) { }
    public override NavigationAgent3D GetNavAgent() { return enemyNavigation; }
    public override bool GetPlayerSpotted() { return false; }
}