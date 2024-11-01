using System;
using Godot;
using Serilog;
using Spacelancer.Components.Commodities;

public partial class TradeAction : Control
{
	private Button _action;
	private TextureRect _texture;
	private Label _name;
	private HSlider _hSlider;
	private Label _quantity;
	private Label _price;
	private Label _total;
	private Label _profit;

	private Commodity _currentCommodity;
	private int _currentPrice;
	private bool _selling;
	
	public override void _Ready()
	{
		Visible = false;
		
		_action = GetNode<Button>("%Action");
		_hSlider = GetNode<HSlider>("%HSlider");
		_texture = GetNode<TextureRect>("%Texture");
		_name = GetNode<Label>("%Name");
		_quantity = GetNode<Label>("%Quantity");
		_price = GetNode<Label>("%Price");
		_total = GetNode<Label>("%Total");

		_action.Pressed += OnTradeActionButtonPressed;
		_hSlider.ValueChanged += OnSliderValueChanged;
	}

	public void SetBuyFromStation(Commodity commodity, int price)
	{
		Visible = true;
		_currentCommodity = commodity;
		_currentPrice = price;
		_selling = false;
		
		_action.Text = "<-- Buy";
		
		_hSlider.Value = 0;
		_hSlider.Editable = true;
		SetBuySliderMax();
		
		_quantity.Text = "Quantity: 0";
		_price.Text = $"Price: {price}";
		_total.Text = "Total: 0";
		
		_texture.Texture = commodity.Texture;
		_name.Text = commodity.Name;
	}
	
	// TODO consider changing this to a CommodityStack and putting profit label back in
	public void SetSellToStation(CommodityStack stack, int price)
	{
		Visible = true;
		_currentCommodity = stack.Commodity;
		_currentPrice = price;
		_selling = true;
		
		_action.Text = "Sell -->";
		_action.Disabled = false;
		
		_hSlider.Value = 0;
		_hSlider.MaxValue = stack.Count;
		_hSlider.Editable = true;
		
		_quantity.Text = "Quantity: 0";
		_price.Text = $"Price: {price}";
		_total.Text = "Total: 0";
		
		_texture.Texture = _currentCommodity.Texture;
		_name.Text = _currentCommodity.Name;
	}

	private void SetBuySliderMax()
	{
		var hold = Global.Player.Hold;

		var cash = Global.Player.Credits;
		var canAfford = cash / _currentPrice;

		var unusedCapacity = hold.GetUnusedCapacity();
		var maxQuantity = _currentCommodity.GetQuantityFromVolume(unusedCapacity);
		
		_hSlider.MaxValue = Math.Min(maxQuantity, canAfford);
		if (_hSlider.MaxValue == 0)
		{
			_action.Disabled = true;
			_hSlider.Editable = false;
		}
	}

	private void OnTradeActionButtonPressed()
	{
		if (_selling)
		{
			SellToStation();
			return;
		}

		BuyFromStation();
	}
		

	private void SellToStation()
	{
	}

	private void BuyFromStation()
	{
		// TODO check that we have enough cash?
		
		var hold = Global.Player.Hold;
		
		Log.Debug("Buying {Quantity} {Commodity} at {Price}", _hSlider.Value, _currentCommodity.Name, _currentPrice);
		
		var stack = hold.GetFromCargoHold(_currentCommodity.Name);
		
		var count = (int)_hSlider.Value;
		var totalPrice = _currentPrice * count;

		Global.Player.Credits -= totalPrice;
		
		var newStack = new CommodityStack(_currentCommodity, count, totalPrice);

		if (stack == null)
		{
			hold.SetToCargoHold(_currentCommodity.Name, newStack);
			SetBuySliderMax();
			return;
		}
		
		hold.RemoveFromCargoHold(_currentCommodity.Name);
		var combinedStack = stack.CombineStack(newStack);
		
		hold.SetToCargoHold(_currentCommodity.Name, combinedStack);
		SetBuySliderMax();
	}

	private void OnSliderValueChanged(double value)
	{
		_quantity.Text = $"Quantity: {(int)value}";
		_total.Text = $"Total: {(int)value * _currentPrice}";
	}
}
