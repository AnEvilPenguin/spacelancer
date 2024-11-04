using System;
using Godot;
using Serilog;
using Spacelancer.Components.Commodities;

public partial class TradeAction : Control
{
	private Button _button;
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

	private Action<int> _action;
	
	public override void _Ready()
	{
		Visible = false;
		
		_button = GetNode<Button>("%Action");
		_hSlider = GetNode<HSlider>("%HSlider");
		_texture = GetNode<TextureRect>("%Texture");
		_name = GetNode<Label>("%Name");
		_quantity = GetNode<Label>("%Quantity");
		_price = GetNode<Label>("%Price");
		_total = GetNode<Label>("%Total");

		_button.Pressed += OnTradeButtonButtonPressed;
		_hSlider.ValueChanged += OnSliderValueChanged;
	}

	public void Reset()
	{
		Visible = false;
		_button.Disabled = true;
		
		_currentCommodity = null;
	}

	/// <summary>
	/// Sets the current behaviour of the component
	/// </summary>
	/// <param name="commodity">The commodity being bought/sold</param>
	/// <param name="price">The price of the commodity</param>
	/// <param name="action">
	///		An action to be executed when the button is pressed
	///		<param name="action arg1">Quantity</param>
	/// </param>
	/// <param name="buttonText">The label applied to the button</param>
	/// <param name="sliderMax">The maximum value of the slider</param>
	public void SetAction(Commodity commodity, int price, Action<int> action, string buttonText, int sliderMax)
	{
		Visible = true;
		
		_button.Disabled = false;
		_action = action;
		
		_currentCommodity = commodity;
		_currentPrice = price;
		
		_button.Text = buttonText;
		
		_hSlider.Value = 0;
		_hSlider.MaxValue = sliderMax;
		_hSlider.Editable = true;
		
		_quantity.Text = "Quantity: 0";
		_price.Text = $"Price: {price}";
		_total.Text = "Total: 0";
	}
	
	private void OnTradeButtonButtonPressed() =>
		_action((int)_hSlider.Value);
	
	private void OnSliderValueChanged(double value)
	{
		_quantity.Text = $"Quantity: {(int)value}";
		_total.Text = $"Total: {(int)value * _currentPrice}";
	}
	
	// TODO clean this up on re-implementation
	/*
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
			_button.Disabled = true;
			_hSlider.Editable = false;
		}
	}
	*/
	
	/*

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
	*/


}
