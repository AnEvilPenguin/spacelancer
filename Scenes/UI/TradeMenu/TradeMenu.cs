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
	private TradeList _sellerTradeList;
	private TradeList _playerTradeList;
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
		
		_sellerTradeList = GetNode<TradeList>("%StationTradeList");
		_sellerTradeList.ItemSelected += OnTradeSellSelected;
		
		_playerTradeList = GetNode<TradeList>("%PlayerTradeList");
		_playerTradeList.ItemSelected += OnTradeBuySelected;
		_playerTradeList.SetTitle("Player");
		
		_tradeAction = GetNode<TradeAction>("%TradeAction");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			
			_sellerTradeList.ClearItemList();
			_playerTradeList.ClearItemList();
			
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
		_sellerTradeList.SetTitle(station.Name);

		var commodities = station.GetCommodityForSale();

		foreach (var tuple in commodities)
		{
			var index = _sellerTradeList.AddItemToList(tuple.Item1, tuple.Item2);
			_stationComodities.Add(index, tuple);
		}
		
		// TODO extract this to a method and deal with this on load/byt/sell

		var hold = Global.Player.Hold;
		var holdContents = hold.GetCargoContents().ToList();

		foreach (var item in holdContents)
		{
			var stack = hold.GetFromCargoHold(item);
			
			var price = station.GetCommodityToBuyPrice(stack.Commodity);
			
			var index = _playerTradeList.AddItemToList(stack.Commodity, price);
			_playerComodities.Add(index, new Tuple<CommodityStack, int>(stack, price));
		}
	}

	private void OnTradeSellSelected(int index)
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
	
	private void OnTradeBuySelected(int index)
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
