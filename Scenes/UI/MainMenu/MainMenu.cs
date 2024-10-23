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
		
		_settingsButton = GetNode<Button>("%SettingsButton");

		ConfigureContinueButton();
		ConfigureNewGameButton();
		ConfigureQuitButton();
		
		Log.Debug("Main Menu loaded");
	}

	public override void _Input(InputEvent @event)
	{
		if (Visible)
			return;
		
		if (@event is InputEventKey eventKey && eventKey.Keycode == Key.Escape)
		{
			Log.Debug("Escape menu triggered");
			
			Visible = true;
			GetTree().Paused = true;
			
			_continueButton.Disabled = false;
		}
	}

	private void ConfigureContinueButton()
	{
		_continueButton = GetNode<Button>("%ContinueButton");
		
		_continueButton.Pressed += (() =>
		{
			Log.Debug("Continue via Main Menu");
			
			GetTree().Paused = false;
			Visible = false;
		});
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
			Log.Debug("New Game via Main Menu");
			
			_gameController.LoadScene("res://Scenes/Systems/sunrise.tscn");
			Visible = false;
		};
	}
}
