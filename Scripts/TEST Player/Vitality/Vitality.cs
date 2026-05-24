using Godot;
using System;
using System.Numerics;

[Tool] //EDIT WHILE RUNNING
public partial class Vitality : Resource
{
	[Export]public string name;
	[Export]public float currentValue;
	[Export]public float maxValue;

	public Vitality(string setName, float setCurrentValue, float setMaxValue)
    {
        name = setName;
		currentValue = setCurrentValue;
		maxValue = setMaxValue;
    }

	public Vitality() { }

	public void Increase(float value)
    {
        currentValue = Mathf.Clamp(currentValue + value, 0, maxValue);
    }

	public void Decrease(float value)
    {
        currentValue = Mathf.Clamp(currentValue - value, 0, maxValue);
    }
    

	public string VitalityName(){ return name;}

	public float GetMaxValue() {return maxValue;}

	public bool isDead() => currentValue<=0;

}
