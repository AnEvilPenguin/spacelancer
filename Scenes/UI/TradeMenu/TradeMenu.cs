using Godot;
using System;
using System.Collections.Generic;
using System.Text;
using Spacelancer.Components.Commodities;

public partial class TradeMenu : CenterContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	private Station _selectedStation;
	private ItemList _sellerItemList;
	private Label _descriptionLabel;
	private TradeAction _tradeAction;
	
	private Dictionary<int, Tuple<Commodity, int>> _stationComodities = new ();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		_descriptionLabel = GetNode<Label>("%DescriptionLabel");
		var defaultText = _descriptionLabel.Text;
		
		_sellerItemList = GetNode<ItemList>("%StationPanel/VBoxContainer/ItemList");
		_sellerItemList.ItemSelected += (long value) => OnItemSelected(value, _stationComodities);
		
		_tradeAction = GetNode<TradeAction>("%TradeAction");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			
			_sellerItemList.Clear();
			_descriptionLabel.Text = defaultText;
			_tradeAction.Visible = false;
			_stationComodities.Clear();
			
			EmitSignal(SignalName.Closing);
		};
		
		// TODO get player inventory and add them to the list.
		// TODO get station inventory and add it to the list.
	}

	public void SetStation(Station station)
	{
		_selectedStation = station;
		
		var container = GetNode<VBoxContainer>("%StationPanel/VBoxContainer");
		container.GetNode<Label>("Label").Text = station.Name;

		var commodities = station.GetCommodityForSale();

		for (int i = 0; i < commodities.Count; i++)
		{
			var tuple = commodities[i];
			
			_stationComodities.Add(i, tuple);
			_sellerItemList.AddItem($"{tuple.Item1.Name}\n{tuple.Item2} credits", tuple.Item1.Texture);
		}
	}

	private void OnItemSelected(long index, Dictionary<int, Tuple<Commodity, int>> commodities)
	{
		var tuple = commodities[(int)index];
		var commodity = tuple.Item1;
		var amount = tuple.Item2;
		

		var label = new StringBuilder();
		label.AppendLine(commodity.Name);
		label.AppendLine($"Price: {amount} credits");
		label.AppendLine();
		label.AppendLine(commodity.Description);
		
		_descriptionLabel.Text = label.ToString();
		
		_tradeAction.SetBuyFromStation(commodity, amount);
	}
		
}
