using Godot;
using System;
using System.Threading.Tasks;

public partial class Reticle : CenterContainer
{
	[Export] float lerpSpeed = 0;
	[Export] Godot.Collections.Array<Texture2D> Reticles;
	[Export] Sprite2D CurrentReticleContainer;
	//0 = idle
	//1 = weapon
	int CurrentReticleIndex = 0;

	[Export]float targetscalemin = 0.2f;
	[Export]float targetscalemax = 3.0f;
	
    public override void _Ready()
    {
        SetReticle(0);
		CurrentReticleContainer.Visible = true;
		//this.AddChild(CurrentReticleContainer);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
		//Test();
    }

	public async Task Test()
	{
        //await Task.Delay(1000);
		//LerpReticle(3.0f);
		//return;
	}



	public void LerpReticle(float lerpSpeed,float targetScale)
	{
		if(CurrentReticleIndex!=0)
			Scale = Scale.Lerp(new Vector2(targetScale, targetScale), lerpSpeed);
		else
			Scale = Scale.Lerp(new Vector2(1f, 1f), lerpSpeed);
	}

	public void SetReticle(int index)
	{
		CurrentReticleIndex = index;
		CurrentReticleContainer.Texture = Reticles[CurrentReticleIndex];
	} 

	


}
