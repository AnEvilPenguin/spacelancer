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
	
	public void SetSliderMax(int max) => 
		_hSlider.MaxValue = max;

	private void OnTradeButtonButtonPressed()
	{
		if (_action == null)
		{
			Log.Debug("No action assigned to TradeAction");
			return;
		}
		
		_action((int)_hSlider.Value);
	}
	
	private void OnSliderValueChanged(double value)
	{
		_quantity.Text = $"Quantity: {(int)value}";
		_total.Text = $"Total: {(int)value * _currentPrice}";
	}
}
