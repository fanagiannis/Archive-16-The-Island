using Godot;
using PolarBears.PlayerControllerAddon;
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

[GlobalClass]
public partial class SceneManager : Node
{
	public ChunkLoader ChunkLoader;
	[Export] Godot.AnimationPlayer ManagerAnimator;
	[Export] PlayerController playerInstance;
	[Export] LoadingScreen loadingScreen;
	[Export] PackedScene PlayerScene;
	[Export] Node3D MainMenuBackground;
	AudioStreamPlayer3D AudioPlayer;
	Vector3 PlayerPosition;
	int selectedScene=0;
	[Export] string[] Scenes; //ARRAY
	string SceneToLoad; //ARRAY
	[Export]Level CurrentLevel;
	[Export] MainMenu MainMenu;
	Vector3 PlayerSpawn;
	bool _isLoading = false;
	bool _isPlayerLoaded=false;
	string EntranceTag = null;
	private static SceneManager _instance;

    public static SceneManager Instance
    {
        get
        {
            if (_instance == null)
            {
                GD.PrintErr("SceneManager instance is null! Make sure it is added to the scene tree.");
            }
            return _instance;
        }
    }
	public override void _Ready()
	{
		_instance=this;
		//loadingScreen.SceneLoaded += SceneLoadedEvent;
		loadingScreen.SetVisibility(false);
		ManagerAnimator.Play("FadeIn");
		if(AudioPlayer!=null)
		{
			GD.Print(AudioPlayer);
		}
		if(ManagerAnimator!=null)
		{	
			//ManagerAnimator.AnimationFinished += ChangeScene;
		}
		//StartMainMenu();
		//FindAllMainMenus();
		//ChunkLoader.Player = playerInstance;
	}

	public async void StartMainMenu()
	{
		if (ManagerAnimator != null )
		{
			MainMenu.Visible = false;

			// Play the animation
			//ManagerAnimator.Play("FadeOut");

			// Wait right here until it's done
			//await ToSignal(ManagerAnimator, AnimationMixer.SignalName.AnimationFinished);

			// NOW start loading
			//SetSelectedScene(0);
			
			StartLoading(Scenes[4],null);
		}
	}

	public async void Start()
	{
		
		if (ManagerAnimator != null )
		{
			MainMenu.Visible = false;

			// Play the animation
			//ManagerAnimator.Play("FadeOut");

			// Wait right here until it's done
			//await ToSignal(ManagerAnimator, AnimationMixer.SignalName.AnimationFinished);

			// NOW start loading
			//SetSelectedScene(0);
			
			StartLoading(SceneToLoad,null);
		}
		/*if (MainMenuBackground != null && IsInstanceValid(MainMenuBackground))
            {
                MainMenuBackground.QueueFree();
				//MainMenuBackground.GlobalPosition = new Vector3(0,1000,0);
                
                GD.Print("3D Main Menu Background unloaded successfully.");
            }*/
	}

	public override void _Process(double delta)
	{
		
	}

	public void StartLoading(string scenePath,string Tag)
	{
		EntranceTag = Tag;
		if (_isLoading)
			return;
		_isLoading = true;
		_ = LoadScene(scenePath,Tag);
		//UnloadScene();
		GD.Print("Started Loading");
		
	}

	public async Task LoadScene(string scenePath,string Tag)
	{
		
		MainMenu.Visible = false;

		PackedScene scene = await loadingScreen.StartLoading(scenePath);

		if (scene == null)
			return;
		GD.Print("Loaded");

		SceneLoadedEvent(scene,Tag);

	}

	public void SceneLoadedEvent(PackedScene scene, string Tag)
	{
		UnloadScene();
		loadingScreen.SetVisibility(false);

		var levelNode = scene.Instantiate();
		if (levelNode == null) return;

		// 1. Add to tree first so GlobalPositions exist
		AddChild(levelNode);

		CurrentLevel = levelNode as Level;
		if (CurrentLevel == null)
		{
			GD.PrintErr("Failed to cast scene to Level!");
			_isLoading = false;
			return;
		}

		// 2. CALCULATE the spawn point BEFORE activating player
		Vector3 targetSpawn;

		// Use the Tag passed into the function, NOT the global EntranceTag variable
		if (!string.IsNullOrEmpty(Tag))
		{
			var door = CurrentLevel.GetEntranceDoor(Tag);
			if (door != null)
			{
				// This ensures the door's global position is calculated
				door.ForceUpdateTransform();
				targetSpawn = door.GetSpawnPoint();
			}
			else
			{
				GD.PrintErr($"Door with Tag {Tag} not found! Using default spawn.");
				targetSpawn = CurrentLevel.GetPlayerSpawn().GlobalPosition;
			}
		}
		else
		{
			targetSpawn = CurrentLevel.GetPlayerSpawn().GlobalPosition;
		}

		// 3. Update the class variable for the player to use
		PlayerSpawn = targetSpawn;

		// 4. Activate and Teleport
		ActivatePlayer();

		// 5. Cleanup
		EntranceTag = null;
		_isLoading = false;
		GD.Print($"Scene loaded successfully. Player at: {PlayerSpawn}");
	}
	public void ActivatePlayer()
	{
		if (PlayerScene != null && !_isPlayerLoaded)
		{
			Node playerScene = PlayerScene.Instantiate();
			playerInstance = playerScene as PlayerController;
			AddChild(playerScene);
			_isPlayerLoaded = true;
		}

		// Ensure movement and UI are ready
		playerInstance.EnableUI(false);
		playerInstance.EnableController(true);

		// This now uses the PlayerSpawn we calculated in SceneLoadedEvent
		playerInstance.TeleportTo(PlayerSpawn);
		playerInstance.PlayerUI.FadeIn();
		ChunkLoader.Player = playerInstance;

		
	}

	public void ExitGame()
	{
		QueueFree();
		_ExitTree();
	}


	private void ChangeScene(StringName animName)
	{
		if(animName=="FadeOut")
			StartLoading(SceneToLoad,null);	
	}

	public void UnloadMainMenu()
	{
		MainMenu.Visible=false;
	}

	public void UnloadScene()
	{

		if (CurrentLevel != null)
        {
            CurrentLevel.QueueFree();
            CurrentLevel = null;
            //_isPlayerLoaded = false;
        }
//        await Task.Delay(500);

	}

	public void FindAllMainMenus()
	{
		var mainMenus = GetTree().GetNodesInGroup("MainMenu");
		if (mainMenus.Count == 0)
		{
			mainMenus = GetTree().Root.FindChildren("*", "MainMenu");
		}

		GD.Print("Found MainMenu instances: " + mainMenus.Count);
		foreach (Node node in mainMenus)
		{
			GD.Print("MainMenu found at path: " + node.GetPath());
		}
	}

	public bool Isloading()
	{
		return _isLoading;
	}

	public void SetSelectedScene(int index)
	{
		selectedScene = index;
		SceneToLoad = Scenes[selectedScene];
	}

	public Vector3 GetPlayerPosition()
	{
		if(playerInstance!=null)
		{
			return playerInstance.GlobalPosition;
			//GD.Print(PlayerPosition);
		}
		return Vector3.Zero;
		
	}

	public String[] GetLevelsList()
	{
		return Scenes;
	}

}
