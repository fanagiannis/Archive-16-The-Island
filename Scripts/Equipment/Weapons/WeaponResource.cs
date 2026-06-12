using Godot;
using System;
using System.Threading.Tasks;

[GlobalClass]
public partial class WeaponResource : Resource
{
    public enum WeaponType
	{
        Melee,Pistol,AssaultRifle,BoltActionRifle,Shotgun,Launcher
    }

    [Export]WeaponType weaponType;
    [Export]string Name;
    [Export]float Damage;
    [Export]float FireRate;
    [Export]float Range;
    [Export]int CurrentAmmo;
    [Export]int MaxAmmo;
    [Export]int Magazines;
    [Export]int MagazineSize;

    private bool CanFire = false;

    public void Fire()
    {
        if(CurrentAmmo>0 && CanFire)
        {
           CurrentAmmo-=1;
        }
       
    }

    public async Task Reload()
    {
        if(MaxAmmo>0)
        {
             CanFire = false;
            await ManageReload();
            CanFire = true;
        }
       
    }

    public async Task ManageReload()
    {
        if(MaxAmmo>=MagazineSize)
        {
            CurrentAmmo = MagazineSize;
            MaxAmmo-=MagazineSize;
        }
        else if(MaxAmmo>0 & MaxAmmo<MagazineSize)
        {
            CurrentAmmo = MaxAmmo;
            MaxAmmo-=MagazineSize;
            MaxAmmo = Mathf.Min(MaxAmmo,0);
        }
        else    
            return;
        //animation
    }

    public void SetCanFire(bool set)
    {
        CanFire = set;
    }

    public bool GetCanFire()
    {
        return CanFire;
    }

    public float GetFireRate()
    {
        return FireRate;
    }   
    public bool HasAmmo()
    {
        return CurrentAmmo>0;
    }

    public int GetCurrentAmmo()
    {
        return CurrentAmmo;
    } 

    public int GetMaxAmmo()
    {
        return MaxAmmo;
    } 

    public float GetDamage()
    {
        return Damage;
    }

    public int GetMagazineSize()
    {
        return MagazineSize;
    }
}
