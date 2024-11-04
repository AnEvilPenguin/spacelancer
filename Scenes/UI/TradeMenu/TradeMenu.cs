using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Serilog;
using Spacelancer.Components.Commodities;
using Spacelancer.Components.Storage;

public partial class TradeMenu : CenterContainer
{
	[Signal]
	public delegate void ClosingEventHandler();
	
	private Station _selectedStation;
	private TradeList _sellerTradeList;
	private TradeList _playerTradeList;
	private TradeAction _tradeAction;
	private TradeDescription _tradeDescription;
	
	private Dictionary<int, Tuple<Commodity, int>> _stationComodities = new ();
	private Dictionary<int, Tuple<CommodityStack, int>> _playerComodities = new ();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var leaveButton = GetNode<Button>("%LeaveButton");
		
		_sellerTradeList = GetNode<TradeList>("%StationTradeList");
		_sellerTradeList.ItemSelected += OnTradeSellSelected;
		
		_playerTradeList = GetNode<TradeList>("%PlayerTradeList");
		_playerTradeList.ItemSelected += OnTradeBuySelected;
		_playerTradeList.SetTitle("Player");
		
		_tradeAction = GetNode<TradeAction>("%TradeAction");
		_tradeDescription = GetNode<TradeDescription>("%TradeDescription");
		
		leaveButton.Pressed += () =>
		{
			Visible = false;
			
			_sellerTradeList.ClearItemList();
			_playerTradeList.ClearItemList();
			
			_tradeAction.Reset();
			_stationComodities.Clear();
			
			_tradeDescription.Reset();
			
			EmitSignal(SignalName.Closing);
		};
	}

	public void LoadMenu(Station station)
	{
		SetStationMenu(station);
		SetPlayerMenu(Global.Player.Hold);
	}

	private void SetStationMenu(Station station)
	{
		_sellerTradeList.ClearItemList();
		_stationComodities.Clear();
		
		_selectedStation = station;
		_sellerTradeList.SetTitle(station.Name);

		var commodities = station.GetCommodityForSale();

		foreach (var tuple in commodities)
		{
			var index = _sellerTradeList.AddItemToList(tuple.Item1, tuple.Item2);
			_stationComodities.Add(index, tuple);
		}
	}

	private void SetPlayerMenu(CargoHold hold)
	{
		_playerTradeList.ClearItemList();
		_playerComodities.Clear();
		
		var holdContents = hold.GetCargoContents().ToList();

		foreach (var item in holdContents)
		{
			var stack = hold.GetFromCargoHold(item);
			
			var price = _selectedStation.GetCommodityToBuyPrice(stack.Commodity);
			
			var index = _playerTradeList.AddItemToList(stack.Commodity, price);
			_playerComodities.Add(index, new Tuple<CommodityStack, int>(stack, price));
		}
	}

	private int GetMaxCommodityPurchase(Commodity commodity, int price)
	{
		var canAfford = Global.Player.Credits / price;
		var canStore = Global.Player.Hold.CheckCapacity(commodity);
		
		return Math.Min(canStore, canAfford);
	}
	
	// TODO Unify these functions, let the action component do the heavy lifting
	// Also consider what way round we want these named, probably want the player to be the focus
	private void OnTradeSellSelected(int index)
	{
		var tuple = _stationComodities[(int)index];
		var commodity = tuple.Item1;
		var price = tuple.Item2;
		
		var maxPurchaseCount = GetMaxCommodityPurchase(commodity, price);
		
		_playerTradeList.DeselectItems();
		
		_tradeDescription.SetTradeDetails(commodity, price);
		
		_tradeAction.SetAction(commodity, price, (quantity) =>
		{
			Log.Debug("Buying {Quantity} {Commodity} at {Price}", quantity, commodity.Name, price);
			
			// TODO check enough money
			
			var player = Global.Player;
			var hold = player.Hold;

			var totalPrice = price * quantity;

			if (player.Credits < totalPrice)
			{
				Log.Error("Player cant afford purchase of {Commodity} - {price} at {TotalPrice}", commodity.Name, price, totalPrice, totalPrice);
				throw new Exception("Player cant afford purchase");
			}
				
			player.Credits -= totalPrice;
			
			var stack = new CommodityStack(commodity, quantity, totalPrice);
			hold.SetToCargoHold(commodity.Name, stack);
			
			_tradeAction.SetSliderMax(GetMaxCommodityPurchase(commodity, price));
			SetPlayerMenu(hold);
		}, "<== Buy", maxPurchaseCount);
	}
	
	private void OnTradeBuySelected(int index)
	{
		var tuple = _playerComodities[(int)index];
		var stack = tuple.Item1;
		var commodity = stack.Commodity;
		var price = tuple.Item2;
		
		_sellerTradeList.DeselectItems();
		
		_tradeDescription.SetTradeDetails(commodity, price);
		// FIXME proper action implementation
		_tradeAction.SetAction(commodity, price, (quantity) =>
		{
			Log.Debug("Selling {Quantity} {Commodity} at {Price}", quantity, commodity.Name, price);
			
			// TODO remove inventory
			// TODO give player money
			// TODO trigger ui re-evaluation
		}, "Sell ==>", stack.Count);
	}
}
