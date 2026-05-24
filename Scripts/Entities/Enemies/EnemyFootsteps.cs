using Godot;
using System;
using System.Diagnostics.CodeAnalysis;

public partial class EnemyFootsteps : RayCast3D
{
	[Export]AudioStreamPlayer3D audioPlayer;
	[Export] Godot.Collections.Dictionary<string, AudioStream> MaterialSounds = new();
	[Export] Godot.Collections.Dictionary<int, string> TerrainTextureMapping = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//audioPlayer = GetNode<AudioStreamPlayer3D>("root/AudioPlayer");
		MaterialSounds["Wood"] = CreateRandomizerFromFolder("res://Imports/Assets/Audio/SFX/Footsteps/Wood-20260209T195312Z-3-001/Wood/");
		MaterialSounds["Dirt"] = CreateRandomizerFromFolder("res://Imports/Assets/Audio/SFX/Footsteps/Dirt-20260209T195307Z-3-001/Dirt/");
		MaterialSounds["Grass"] = CreateRandomizerFromFolder("res://Imports/Assets/Audio/SFX/Footsteps/Gravel-20260209T195309Z-3-001/Gravel");
	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}

	public void PlayAudio()
	{
		//GD.Print("FOOT");
		var collider = this.GetCollider();
        string detectedMaterial = "";

        // 1. CHECK FOR TERRAIN3D
        // We check the name or class to see if it's the terrain plugin
        
		if (this.IsColliding())
		{
			if (this.GetCollider() is Node nodeCollider)
			{
				if (nodeCollider.IsInGroup("Terrain"))
				{

					Vector3 hitPos = this.GetCollisionPoint();

					// Travel up the tree to find the main Terrain3D node automatically
					Node terrainNode = nodeCollider;
					while (terrainNode != null && terrainNode.Name != "Terrain3D")
					{
						terrainNode = terrainNode.GetParent();
					}

					if (terrainNode != null)
					{
						GodotObject terrainData = null;

						// Terrain3D v1.0+ uses "data", v0.9 and older use "storage"
						Variant dataVar = terrainNode.Get("data");
						Variant storageVar = terrainNode.Get("storage");

						if (dataVar.VariantType != Variant.Type.Nil)
							terrainData = dataVar.AsGodotObject();
						else if (storageVar.VariantType != Variant.Type.Nil)
							terrainData = storageVar.AsGodotObject();

						// Now ask the data object for the texture ID at our raycast hit position
						if (terrainData != null)
						{
							Variant texResult = terrainData.Call("get_texture_id", hitPos);
							int textureId = 0;

							// Depending on the Terrain3D version, get_texture_id returns a Vector3 
							// (Base Texture, Overlay Texture, Blend Value) or just an int.
							if (texResult.VariantType == Variant.Type.Vector3)
							{
								textureId = Mathf.RoundToInt(texResult.AsVector3().X);
							}
							else
							{
								textureId = texResult.AsInt32();
							}

							// Map the Texture ID to your material dictionary
							if (TerrainTextureMapping.ContainsKey(textureId))
							{
								detectedMaterial = TerrainTextureMapping[textureId];
								GD.Print(detectedMaterial);
								
							}
			
							//PlayFootstep(MaterialSounds[detectedMaterial.material.name] );
						}
					}
				}
				else if (nodeCollider.IsInGroup("Wood"))
				{
					detectedMaterial = "Wood";
				}

					//GD.Print("Wood surface");

				else if (nodeCollider.IsInGroup("stone"))
				{
					detectedMaterial = "Stone";
				}
					
					//GD.Print("Stone surface");

				if (!string.IsNullOrEmpty(detectedMaterial))
				{
					if (MaterialSounds.ContainsKey(detectedMaterial))
					{
						//GD.Print($"SUCCESS! Playing footstep for: {detectedMaterial}");
						AudioStream stream = MaterialSounds[detectedMaterial];
						audioPlayer.Stream = stream;
						audioPlayer.Play();
					}
					else
					{
						//GD.PrintErr($"No audio loaded in MaterialSounds for the key: '{detectedMaterial}'");
					}
				}
			}
		}
	}
	private AudioStreamRandomizer CreateRandomizerFromFolder(string folderPath)
	{
		AudioStreamRandomizer randomizer = new AudioStreamRandomizer();
		// Set your pitch variation here
		randomizer.RandomPitch = 1.15f; 

		using var dir = DirAccess.Open(folderPath);
		if (dir != null)
		{
			dir.ListDirBegin();
			string fileName = dir.GetNext();

			while (fileName != "")
			{
				// Only grab audio files (ignore folders and .import files)
				if (!dir.CurrentIsDir() && (fileName.EndsWith(".wav") || fileName.EndsWith(".ogg")))
				{
					// Load the audio file
					AudioStream stream = GD.Load<AudioStream>($"{folderPath}/{fileName}");
					
					// Add it to the randomizer (-1 means add to the end of the list)
					randomizer.AddStream(-1, stream);
				}
				fileName = dir.GetNext();
			}
		}
		else
		{
			//GD.PrintErr($"Failed to open footstep folder: {folderPath}");
		}

		return randomizer;
	}
}
