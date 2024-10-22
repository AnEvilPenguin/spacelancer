using Godot;
using Serilog;

public partial class MainMenu : CenterContainer
{
	private Button _continueButton;
	private Button _newGameButton;
	private Button _settingsButton;
	private Button _quitButton;

	public override void _Ready()
	{
		_continueButton = GetNode<Button>("%ContinueButton");
		_newGameButton = GetNode<Button>("%NewGameButton");
		_settingsButton = GetNode<Button>("%SettingsButton");

		ConfigureQuitButton();
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
}
