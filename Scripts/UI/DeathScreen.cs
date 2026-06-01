using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public partial class DeathScreen : Control
{
    [Signal]
    public delegate void OnReturnEventHandler();
    [Signal]
    public delegate void OnExitEventHandler();
    
    private Panel _deathMenuPanel;
    [Export] Theme PauseMenuTheme;
    private Button _returnButton;
    private Button _exitButton;
    private Godot.AnimationPlayer _screenAnimator;
    
    public override void _Ready()
    {
        // Get references to the panel and buttons
        _deathMenuPanel= GetNode<Panel>("Panel");
        _returnButton= GetNode<Button>("Panel/Label/ResumeButton");
        _exitButton = GetNode<Button>("Panel/Label/ExitButton");
        _screenAnimator = GetNode<Godot.AnimationPlayer>("MenuAnimator");

        // Connect button signals to methods
        _returnButton.Pressed += OnMenuButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
        Hide();
        

		//GD.Print("MainMenu" +" " + GetPath());
    }

    private void OnMenuButtonPressed()
    {
        // Handle the Start button press (e.g., start the game)
        GD.Print("Resume button pressed!");
        Hide();
        GetTree().Paused = false;
        SceneManager.Instance.ReturnToMainMenu();
        //LoadMainGame();
        //LoadLevelList();
        // SceneManager.Instance.Start();
        // Hide the main menu panel if needed
        //_mainMenuPanel.Visible = false;
    }

    private void OnExitButtonPressed()
    {
        // Handle the Exit button press (e.g., exit the game)
        GD.Print("Exit button pressed!");
        SceneManager.Instance.ExitGame();
        //RELOAD GAME (RETURN TO MAIN MENU )
    }

    private void LoadMainMenu()
    {
        _deathMenuPanel.Visible=true;
        //LoadLevelButtons(SceneManager.Instance.GetLevelsList());
    }

    public void PlayFadeAnimation()
    {
        _screenAnimator.Play("Opening");
    }
}
