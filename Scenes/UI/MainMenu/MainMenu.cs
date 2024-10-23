using Godot;
using Serilog;

public partial class MainMenu : CenterContainer
{
	private Button _continueButton;
	private Button _newGameButton;
	private Button _settingsButton;
	private Button _quitButton;
	
	private GameController _gameController;

	public override void _Ready()
	{
		_gameController = Global.Instance.GameController;
		
		_continueButton = GetNode<Button>("%ContinueButton");
		_settingsButton = GetNode<Button>("%SettingsButton");

		ConfigureQuitButton();
		ConfigureNewGameButton();
		
		Log.Debug("Main Menu loaded");
	}

	private void ConfigureQuitButton()
	{
        _quitButton = GetNode<Button>("%QuitButton");

		_quitButton.Pressed += () =>
		{
			Log.Debug("Quit via Main Menu");

			// We want to send the quit notification so that other components
			//     get the chance to shut down cleanly.
            GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
        };
    }

	private void ConfigureNewGameButton()
	{
		_newGameButton = GetNode<Button>("%NewGameButton");

		_newGameButton.Pressed += () =>
		{
			_gameController.LoadScene("res://Scenes/Systems/sunrise.tscn");
			Visible = false;
		};
	}
	
	// TODO Get 'Escape' keypress and make visible again
}
