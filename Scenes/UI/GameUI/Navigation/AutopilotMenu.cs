using System.Collections.Generic;
using Godot;
using Spacelancer.Components.Navigation;
using Spacelancer.Scenes.UI.StationMenu.IconButton;

namespace Spacelancer.Scenes.UI.GameUI.Navigation;

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
		_freeTravel.Pressed += () => OnButtonPressed(_freeTravel, NavigationSoftwareType.Manual);
		
		_autopilot = GetNode<IconButton>("%AutoPilot");
		_autopilot.Pressed += () => OnButtonPressed(_autopilot, NavigationSoftwareType.Navigation);
		
		_docking = GetNode<IconButton>("%Dock");
		_docking.Pressed += () => OnButtonPressed(_docking, NavigationSoftwareType.Docking);
		
		// TODO make this not an otter and get a better icon.
		// Though it is a pretty sweet otter...
		_formation = GetNode<IconButton>("%Formation");
		_formation.Pressed += () => OnButtonPressed(_formation, NavigationSoftwareType.Formation);
		
		_active = _freeTravel;
		
		_buttons.Add(_freeTravel, true);
		_buttons.Add(_autopilot, false);
		_buttons.Add(_docking, false);
		_buttons.Add(_formation, false);
	}

	public void SetButtonAvailability(NavigationSoftwareType button, bool availability)
	{
		switch (button)
		{
			case NavigationSoftwareType.Manual:
				_buttons[_freeTravel] = availability;
				SetButtonColor(_freeTravel, availability ? _availableColor : _inactiveColor);
				return;
			
			case NavigationSoftwareType.Navigation:
				_buttons[_autopilot] = availability;
				SetButtonColor(_autopilot, availability ? _availableColor : _inactiveColor);
				return;
			
			case NavigationSoftwareType.Docking:
				_buttons[_docking] = availability;
				SetButtonColor(_docking, availability ? _availableColor : _inactiveColor);
				return;
			
			case NavigationSoftwareType.Formation:
				_buttons[_formation] = availability;
				SetButtonColor(_formation, availability ? _availableColor : _inactiveColor);
				return;
		}
	}

	public void SetActive(NavigationSoftwareType button)
	{
		var availability = _buttons[_active];
		SetButtonColor(_active, availability ? _availableColor : _inactiveColor);

		_active = button switch
		{
			NavigationSoftwareType.Manual => _freeTravel,
			NavigationSoftwareType.Navigation => _autopilot,
			NavigationSoftwareType.Docking => _docking,
			_ => _formation
		};
		
		SetButtonColor(_active, _activeColor);
	}

	private void SelectButton(NavigationSoftwareType button)
	{
		SetActive(button);
		
		var raiseEvent = AutopilotSelected;
		
		if (raiseEvent != null)
			raiseEvent(this, new AutopilotSelectedEventArgs(button));
	}

	private void OnButtonPressed(IconButton button, NavigationSoftwareType buttonType)
	{
		if (_active == button || !_buttons[button])
			return;
			
		SelectButton(buttonType);
	}

	private void SetButtonColor(IconButton button, Color color) =>
		button.SetColor(color);
}