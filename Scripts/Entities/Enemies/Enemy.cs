using Godot;
using GroveGames.BehaviourTree;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] protected EnemyData enemyData;
	[Export] protected Label3D HPDisplay;
	[Export] protected DamageArea DamageArea;
	protected int difficultyIndex;
	
	protected EnemyBehavior enemyBehavior;
	//VITALITY
	protected float HP=1;
	protected float current_speed;
	protected bool isDead=false;
	protected bool isEnabled = true;
	public override void _Ready()
	{
		SetupAI();
		
		Dead();
		DamageArea.SetDamage(enemyData.GetDamage());
		if(enemyData!=null)
		{
			HP=enemyData.GetMaxHP();
			//SetAgentSpeed();
		}
		if(HPDisplay!=null)
			HPDisplay.Text = HP.ToString("0");
		
		
	}

	public override void _Process(double delta)
	{
		if (enemyBehavior.GetNavAgent() == null || enemyBehavior.GetNavAgent().IsNavigationFinished())
            return;  
        // Move the CharacterBody3D
       // Velocity = enemyBehavior.MoveToTarget();
        //MoveAndSlide();
	}

	public void TakeDamage(float value)
	{
		HP-=value;
		HP = Mathf.Max(HP,0);
		HPDisplay.Text = HP.ToString("0");
		Dead();
	}

	public void Heal(float value)
	{
		HP+=value;
		HP=Mathf.Min(HP,enemyData.GetMaxHP());
	}

	public void Dead()
	{
		if(HP<=0)
		{
			isDead=true;
			QueueFree();
		}
	}

	public virtual void SetupAI()
	{
		enemyBehavior = GetNode<EnemyBehavior>("AI");
		if(enemyBehavior!=null)
		{
			//current_speed  = enemyData.walk_speed;
			//enemyBehavior.SetUpBehavior(GlobalTransform,current_speed );
			//enemyBehavior.SetTargetPosition(new Vector3(1,1,0));
		}
        	
	}

	public virtual void SetDifficulty(int value)
	{
		difficultyIndex = value;
		enemyBehavior.SetBlackboardValue("Difficulty",difficultyIndex);
		SetAgentSpeed();
	}

	public virtual void SetAgentSpeed()
	{
		if(difficultyIndex==0)
			current_speed = enemyData.walk_speed;
		else if(difficultyIndex>0)
			current_speed = enemyData.sprint_speed;
		enemyBehavior.SetBlackboardValue("AgentSpeed",current_speed);
	}

	public EnemyBehavior GetBehaviorAgent()
	{
		if(enemyBehavior!=null)
			return enemyBehavior;
		return null;
	}

	public void SetEnabled(bool set)
	{
		isEnabled = set;
		Visible=set;
		ProcessMode = set ? ProcessModeEnum.Inherit : ProcessModeEnum.Disabled;
	}
}
