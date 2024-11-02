using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spacelancer.Components.Commodities;

public partial class TradeMenu : CenterContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	private Station _selectedStation;
	private ItemList _sellerItemList;
	private ItemList _playerItemList;
	private Label _descriptionLabel;
	private TradeAction _tradeAction;
	
	private Dictionary<int, Tuple<Commodity, int>> _stationComodities = new ();
	private Dictionary<int, Tuple<CommodityStack, int>> _playerComodities = new ();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		_descriptionLabel = GetNode<Label>("%DescriptionLabel");
		var defaultText = _descriptionLabel.Text;
		
		_sellerItemList = GetNode<ItemList>("%StationPanel/VBoxContainer/ItemList");
		_sellerItemList.ItemSelected += OnItemSellSelected;
		
		_playerItemList = GetNode<ItemList>("%PlayerPanel/VBoxContainer/ItemList");
		_playerItemList.ItemSelected += OnItemBuySelected;
		
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
		
		// TODO extract this to a method and deal with this on load/byt/sell

		var hold = Global.Player.Hold;
		var holdContents = hold.GetCargoContents().ToList();

		for (int i = 0; i < holdContents.Count; i++)
		{
			var type = holdContents[i];

			var stack = hold.GetFromCargoHold(type);
			
			var price = station.GetCommodityToBuyPrice(stack.Commodity);
			
			_playerComodities.Add(i, new Tuple<CommodityStack, int>(stack, price));
			_playerItemList.AddItem($"{stack.Commodity.Name}\n{price} credits", stack.Commodity.Texture);
		}
	}

	private void OnItemSellSelected(long index)
	{
		var tuple = _stationComodities[(int)index];
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
	
	private void OnItemBuySelected(long index)
	{
		var tuple = _playerComodities[(int)index];
		var commodity = tuple.Item1.Commodity;
		var amount = tuple.Item2;
		

		var label = new StringBuilder();
		label.AppendLine(commodity.Name);
		label.AppendLine($"Price: {amount} credits");
		label.AppendLine();
		label.AppendLine(commodity.Description);
		
		_descriptionLabel.Text = label.ToString();
		
		_tradeAction.SetSellToStation(tuple.Item1, amount);
	}
		
}
