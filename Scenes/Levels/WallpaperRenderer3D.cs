using Godot;
using System;

public partial class WallpaperRenderer3D : Node3D
{

    [Export] 
    private SubViewport _subViewport;

    public override async void _Ready()
    {
        if (_subViewport == null)
        {
            GD.PrintErr("Please assign the SubViewport in the Inspector!");
            return;
        }

        // 1. Wait for two engine ticks to let the 3D world and lighting load
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);

        // 2. Wait for the graphics card to finish drawing the current frame
        await ToSignal(RenderingServer.Singleton, RenderingServer.SignalName.FramePostDraw);

        // 3. Capture the image from the SubViewport
        Image img = _subViewport.GetTexture().GetImage();

        // 4. Save it directly to your project folder
        string savePath = "res://auto_wallpaper.png";
        img.SavePng(savePath);

        // 5. Print the exact computer file path to the console
        string exactPath = ProjectSettings.GlobalizePath(savePath);
        GD.Print($"Auto-screenshot saved to: {exactPath}");
    }

}
