using Godot;
using System;
using System.Threading.Tasks;

public partial class Weapon : Equipment
{
	[Export] WeaponResource WeaponData;
	[Export] Godot.AnimationPlayer animator;
	[Export] Godot.AnimationPlayer muzzleanimator;
	[Export] RayCast3D pistolFireCast;
	[Export] SpotLight3D muzzleFlash;
	[Export] Label3D ammoCount;
	public override void _Ready()
	{
		//animator = GetNode<Godot.AnimationPlayer>("Animator");
		WeaponData.SetCanFire(true);
		muzzleFlash.Visible=false;
		ammoCount.Text = WeaponData.GetCurrentAmmo().ToString() + "/" + WeaponData.GetMaxAmmo().ToString();
	}

	public override void _Process(double delta)
	{
	}

    public override async void Use()
    {
        //base.Use();
		if(WeaponData.GetCanFire())
		{
			await Fire();
			if(pistolFireCast.GetCollider()!=null)
			{
				//GD.Print(pistolFireCast.GetCollider());
				Vector3 collisionPosition = pistolFireCast.GetCollisionPoint();
				GD.Print(collisionPosition);
				if(pistolFireCast.GetCollider() is Enemy enemy)
				{
					enemy.TakeDamage(WeaponData.GetDamage());
				}
			}
		}
	
			
    }

	public virtual async Task Reload()
	{
		await WeaponData.Reload();
		ammoCount.Text = WeaponData.GetCurrentAmmo().ToString() + "/" + WeaponData.GetMaxAmmo().ToString();
	}
	
	public async Task Fire()
	{
		WeaponData.Fire();
		muzzleanimator.Play("MuzzleFlash");
		WeaponData.SetCanFire(false);
		WeaponData.Fire();
		ammoCount.Text = WeaponData.GetCurrentAmmo().ToString() + "/" + WeaponData.GetMaxAmmo().ToString();
		animator.Play("Fire",customSpeed:WeaponData.GetFireRate());
    	await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
		if(WeaponData.HasAmmo())
			WeaponData.SetCanFire(true);
		
	}

	public WeaponResource GetWeaponData()
	{
		return WeaponData;
	}
}
