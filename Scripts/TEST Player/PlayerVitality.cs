using Godot;
using System;

public partial class PlayerVitality : Node
{
	[Signal]
    public delegate void TakeDamageEventHandler(float value);
	[Signal]
    public delegate void DecreaseStaminaEventHandler(float value);
	[Export]public Vitality Health = new Vitality();
	[Export]public Vitality Stamina = new Vitality();
	public void Damage(float amount)
	{
		Health?.Decrease(amount);
		EmitSignal(SignalName.TakeDamage,amount);
	}
    public void UseStamina(float amount)
	{
		Stamina?.Decrease(amount);
		EmitSignal(SignalName.DecreaseStamina,amount);
	}

	public override void _Ready()
    {
        
    }

	public override void _Process(double delta)
	{
	}
}
