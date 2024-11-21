using System.Collections.Generic;
using Godot;
using Spacelancer.Scenes.UI.StationMenu.IconButton;

namespace Spacelancer.Scenes.UI.GameUI.Navigation;

public enum AutopilotButtonType
{
	FreeTravel,
	Autopilot,
	Docking,
	Formation
}

public partial class AutopilotMenu : PanelContainer
{
	public event AutopilotSelectedEventHandler AutopilotSelected;
	public delegate void AutopilotSelectedEventHandler(object sender, AutopilotSelectedEventArgs e);
	
	[Export] 
	private Color _activeColor;

	[Export] 
	private Color _availableColor;
	
	[Export] 
	private Color _inactiveColor;
	
	private IconButton _freeTravel;
	private IconButton _autopilot;
	private IconButton _docking;
	private IconButton _formation;
	
	private readonly Dictionary<IconButton, bool> _buttons = new();
	
	private IconButton _active;
	
	public override void _Ready()
	{
		_freeTravel = GetNode<IconButton>("%FreeTravel");
		_freeTravel.Pressed += () => OnButtonPressed(_freeTravel, AutopilotButtonType.FreeTravel);
		
		_autopilot = GetNode<IconButton>("%AutoPilot");
		_autopilot.Pressed += () => OnButtonPressed(_autopilot, AutopilotButtonType.Autopilot);
		
		_docking = GetNode<IconButton>("%Dock");
		_docking.Pressed += () => OnButtonPressed(_docking, AutopilotButtonType.Docking);
		
		// TODO make this not an otter and get a better icon.
		// Though it is a pretty sweet otter...
		_formation = GetNode<IconButton>("%Formation");
		_formation.Pressed += () => OnButtonPressed(_formation, AutopilotButtonType.Docking);
		
		_active = _freeTravel;
		
		_buttons.Add(_freeTravel, true);
		_buttons.Add(_autopilot, false);
		_buttons.Add(_docking, false);
		_buttons.Add(_formation, false);
	}

	public void SetButtonAvailability(AutopilotButtonType button, bool availability)
	{
		switch (button)
		{
			case AutopilotButtonType.FreeTravel:
				_buttons[_freeTravel] = availability;
				SetButtonColor(_freeTravel, availability ? _availableColor : _inactiveColor);
				return;
			
			case AutopilotButtonType.Autopilot:
				_buttons[_autopilot] = availability;
				SetButtonColor(_autopilot, availability ? _availableColor : _inactiveColor);
				return;
			
			case AutopilotButtonType.Docking:
				_buttons[_docking] = availability;
				SetButtonColor(_docking, availability ? _availableColor : _inactiveColor);
				return;
			
			case AutopilotButtonType.Formation:
				_buttons[_formation] = availability;
				SetButtonColor(_formation, availability ? _availableColor : _inactiveColor);
				return;
		}
	}

	public void SetActive(AutopilotButtonType button)
	{
		var availability = _buttons[_active];
		SetButtonColor(_active, availability ? _availableColor : _inactiveColor);

		_active = button switch
		{
			AutopilotButtonType.FreeTravel => _freeTravel,
			AutopilotButtonType.Autopilot => _autopilot,
			AutopilotButtonType.Docking => _docking,
			_ => _formation
		};
		
		SetButtonColor(_active, _activeColor);
	}

	private void SelectButton(AutopilotButtonType button)
	{
		SetActive(button);
		
		var raiseEvent = AutopilotSelected;
		
		if (raiseEvent != null)
			raiseEvent(this, new AutopilotSelectedEventArgs(button));
	}

	private void OnButtonPressed(IconButton button, AutopilotButtonType buttonType)
	{
		if (_active == button || !_buttons[button])
			return;
			
		SelectButton(buttonType);
	}

	private void SetButtonColor(IconButton button, Color color) =>
		button.SetColor(color);
}