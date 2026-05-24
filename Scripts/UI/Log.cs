using System;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class Log : Label
{
    private static Log _instance;
    private Tween _activeTween; 

    public static Log Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("Log instance is null!");
            }
            return _instance;
        }
    }

    public override void _Ready()
    {
        _instance = this;
        Visible = false;
        Modulate = new Color(1, 1, 1, 0);
    }

    public void SetLog(string text,float duration)
    {
        Text = text;
        Visible = true;
        if (_activeTween != null && _activeTween.IsValid())
        {
            _activeTween.Kill(); 
        }
        Modulate = new Color(1, 1, 1, 1);
        _activeTween = GetTree().CreateTween();
        _activeTween.TweenInterval(duration); 
        _activeTween.TweenProperty(this, "modulate:a", 0.0f, 1.0f);
        _activeTween.Finished += () => Visible = false;
    }
}