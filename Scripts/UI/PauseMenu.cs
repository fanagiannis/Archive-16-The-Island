using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public partial class PauseMenu : Control
{
    [Signal]
    public delegate void OnResumeEventHandler();
    [Signal]
    public delegate void OnExitEventHandler();
    private Panel _pauseMenuPanel;
    [Export] Theme PauseMenuTheme;
    private Button _resumeButton;
    private Button _optionsButton;
    private Button _exitButton;
    private Button _backButton;
    
    public override void _Ready()
    {
        // Get references to the panel and buttons
        _pauseMenuPanel= GetNode<Panel>("Panel");
        _resumeButton = GetNode<Button>("Panel/Label/ResumeButton");
        _optionsButton = GetNode<Button>("Panel/Label/OptionsButton");
        _exitButton = GetNode<Button>("Panel/Label/ExitButton");
        _backButton = GetNode<Button>("Dev_Levels/VBoxContainer/BackButton");

        // Connect button signals to methods
        _resumeButton.Pressed += OnResumeButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
        _backButton.Pressed +=LoadMainMenu;
        Hide();
        

		//GD.Print("MainMenu" +" " + GetPath());
    }

    private void OnResumeButtonPressed()
    {
        // Handle the Start button press (e.g., start the game)
        GD.Print("Resume button pressed!");
        Hide();
        GetTree().Paused = false;
        EmitSignal(SignalName.OnResume);
        //LoadMainGame();
        //LoadLevelList();
        // SceneManager.Instance.Start();
        // Hide the main menu panel if needed
        //_mainMenuPanel.Visible = false;
    }

    private void OnOptionsButtonPressed()
    {
        // Handle the Options button press (e.g., open options menu)
        GD.Print("Options button pressed!");
    }

    private void OnExitButtonPressed()
    {
        // Handle the Exit button press (e.g., exit the game)
        GD.Print("Exit button pressed!");
        GetTree().Paused = false;
        EmitSignal(SignalName.OnExit);
        //RELOAD GAME (RETURN TO MAIN MENU )
    }

    private void LoadMainMenu()
    {
        _pauseMenuPanel.Visible=true;
        //LoadLevelButtons(SceneManager.Instance.GetLevelsList());
    }

    private void OnLevelSelected(string path)
    {
        GD.Print($"Loading actual file at: {path}");
        SceneManager.Instance.StartLoading(path,null);
    }

}
