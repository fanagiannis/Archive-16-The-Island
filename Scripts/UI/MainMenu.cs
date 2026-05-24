using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;

public partial class MainMenu : Control
{

    private Panel _mainMenuPanel;
    [Export] Theme MainMenuTheme;
    private Button _startButton;
    private Button _optionsButton;
    private Button _exitButton;
    private Button _backButton;
    [Export] Panel _LevelSelectionScreen;
    [Export] VBoxContainer _LevelsLabel;
    private List<Button> LevelButtons = new List<Button>();
    [Export]private string MainGameLevelPath;
    
    public override void _Ready()
    {
        // Get references to the panel and buttons
        _mainMenuPanel = GetNode<Panel>("Panel");
        _startButton = GetNode<Button>("Panel/Label/StartButton");
        _optionsButton = GetNode<Button>("Panel/Label/OptionsButton");
        _exitButton = GetNode<Button>("Panel/Label/ExitButton");
        _backButton = GetNode<Button>("Dev_Levels/VBoxContainer/BackButton");

        // Connect button signals to methods
        _startButton.Pressed += OnStartButtonPressed;
        _optionsButton.Pressed += OnOptionsButtonPressed;
        _exitButton.Pressed += OnExitButtonPressed;
        _backButton.Pressed +=LoadMainMenu;

        

		//GD.Print("MainMenu" +" " + GetPath());
    }

    private void OnStartButtonPressed()
    {
        // Handle the Start button press (e.g., start the game)
        GD.Print("Start button pressed!");
        LoadMainGame();
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
        SceneManager.Instance.ExitGame();
    }

    private void LoadLevelList()
    {
        _mainMenuPanel.Visible=false;
        _LevelSelectionScreen.Visible=true;
        LoadLevelButtons(SceneManager.Instance.GetLevelsList());
    }

    private void LoadMainMenu()
    {
        _mainMenuPanel.Visible=true;
        _LevelSelectionScreen.Visible=false;
        //LoadLevelButtons(SceneManager.Instance.GetLevelsList());
    }

    public void LoadLevelButtons(String[] levelsList)
    {
        /*foreach(Button button in _LevelsLabel)
        {
            if(button==_backButton)
                break;
            button.QueueFree();
        }*/
        foreach (String level in levelsList)
        {
            Button button = new Button();
            string displayName = Path.GetFileNameWithoutExtension(level);
            button.Theme = MainMenuTheme;
            button.LayoutMode = 1;
            button.Position = new Vector2(0,_LevelsLabel.GetChildCount()*80);//GetChild.GetChildCount()*80);
            button.Text = displayName;
            button.CustomMinimumSize = new Vector2(200, 60);
            button.Pressed += () =>OnLevelSelected(level); 
            LevelButtons.Add(button);
            _LevelsLabel.AddChild(button);
        }
    }

    private void OnLevelSelected(string path)
    {
        GD.Print($"Loading actual file at: {path}");
        SceneManager.Instance.StartLoading(path,null);
    }

    private void LoadMainGame()
    {
        SceneManager.Instance.StartLoading(MainGameLevelPath,null);
    }

}
