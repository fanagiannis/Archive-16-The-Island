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
	private int currentAmmo;
	private int maxAmmo;
	public override void _Ready()
	{
		//animator = GetNode<Godot.AnimationPlayer>("Animator");
		WeaponData.SetCanFire(true);
		muzzleFlash.Visible=false;
		currentAmmo = WeaponData.GetCurrentAmmo();
		maxAmmo = WeaponData.GetMaxAmmo();
		ammoCount.Text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
	}

	public override void _Process(double delta)
	{
	}

    public override async void Use()
    {
        base.Use();
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
		await ReloadAmmo();
		ammoCount.Text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
	}
	
	public async Task Fire()
	{
		FireWeapon();
		muzzleanimator.Play("MuzzleFlash");
		WeaponData.SetCanFire(false);
		ammoCount.Text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
		animator.Play("Fire",customSpeed:WeaponData.GetFireRate());
		PlayEquipmentSound();
    	await ToSignal(animator, AnimationPlayer.SignalName.AnimationFinished);
		if(HasAmmo())
			WeaponData.SetCanFire(true);
		
	}

	public void FireWeapon()
	{
		if(currentAmmo>0 && WeaponData.GetCanFire())
        {
           currentAmmo-=1;
        }
	}

	public virtual async Task ReloadAmmo()
	{
		if(maxAmmo>0)
        {
            WeaponData.SetCanFire(false);
            if(maxAmmo>=WeaponData.GetMagazineSize())
			{
				currentAmmo = WeaponData.GetMagazineSize();
				maxAmmo-=WeaponData.GetMagazineSize();
			}
			else if(maxAmmo>0 & maxAmmo<WeaponData.GetMagazineSize())
			{
				currentAmmo = maxAmmo;
				maxAmmo-=WeaponData.GetMagazineSize();
				maxAmmo = Mathf.Min(maxAmmo,0);
			}
			else    
            	return;
            WeaponData.SetCanFire(true);
        }
	}

	public void AddMaxAmmo(int value)
	{
		maxAmmo+=value;
		ammoCount.Text = currentAmmo.ToString() + "/" + maxAmmo.ToString();
	}

	public bool HasAmmo()
    {
        return currentAmmo>0;
    }

	public WeaponResource GetWeaponData()
	{
		return WeaponData;
	}
}
