using Godot;
using System;
using System.Dynamic;
using System.Threading.Tasks;

public partial class LoadingScreen : Control
{
    [Signal]
    public delegate void SceneLoadedEventHandler();

    private CanvasLayer _screenOverlay;
    [Export] public ProgressBar LoadingBar { get; set; }
    private Godot.Collections.Array _progress;

    public override void _Ready()
    {
        _screenOverlay = GetNode<CanvasLayer>("CanvasLayer");
        _screenOverlay.Visible = false;
    }

    public void SetVisibility(bool isVisible)
    {
        _screenOverlay.Visible = isVisible;
    }
    /*
    public async Task StartLoading(PackedScene scene)
    {
        SetVisibility(true);
        await AwaitLoadScene(scene);
    }
    */
    public async Task<PackedScene> StartLoading(string scenePath)
    {
        SetVisibility(true);

        _progress = new Godot.Collections.Array();
        _progress.Resize(1);

        ResourceLoader.LoadThreadedRequest(scenePath);

        return await AwaitLoading(scenePath);
    }
    public async Task<PackedScene> AwaitLoadScene(string scenePath)
    {
        _progress = new Godot.Collections.Array();
        _progress.Resize(1);
        ResourceLoader.LoadThreadedRequest(scenePath);
        return await AwaitLoading(scenePath);
    }

    public async Task<PackedScene> AwaitLoading(string scenePath)
    {

        while (true)
        {
            var status = ResourceLoader.LoadThreadedGetStatus(scenePath, _progress);

            float currentProgress = (float)_progress[0];
            //await Task.Delay(1000);
            LoadingBar.Value = currentProgress * 100;

            if (status == ResourceLoader.ThreadLoadStatus.Loaded)
            {
                var resource = ResourceLoader.LoadThreadedGet(scenePath);
                return resource as PackedScene;
            }

            if (status == ResourceLoader.ThreadLoadStatus.Failed)
            {
                GD.PrintErr("Failed to load scene: " + scenePath);
                return null;
            }

            await ToSignal(GetTree(), "process_frame");

        }

        /*
        while (true)
        {
			await Task.Delay(1500);
            var status = ResourceLoader.LoadThreadedGetStatus(scene.ResourcePath, _progress);
            float currentProgress = (float)_progress[0];
            LoadingBar.Value = currentProgress * 100;

            if (status == ResourceLoader.ThreadLoadStatus.Loaded)
            {
				await Task.Delay(1000);
                EmitSignal(SignalName.SceneLoaded);
                break;
            }

            if (status == ResourceLoader.ThreadLoadStatus.Failed)
            {
                GD.PrintErr("Failed to load scene: " + scene.ResourcePath);
                return;
            }

            await Task.Delay(16); // Yield to avoid blocking the main thread
        }

		
		
		*/
    }
}
