using Godot;
using Spacelancer.Economy;

namespace Spacelancer.Scenes.UI.StationMenu.TradeMenu.TradeDescription;

public partial class TradeDescription : PanelContainer
{
	private Label _componentNameLabel;
	private Label _tradeSummaryLabel;
	private Label _componentDescriptionLabel;
	
	public override void _Ready()
	{
		_componentNameLabel = GetNode<Label>("%ComponentName");
		_tradeSummaryLabel = GetNode<Label>("%TradeSummary");
		_componentDescriptionLabel = GetNode<Label>("%ComponentDescription");
	}

	public void SetTradeDetails(Commodity commodity, int price)
	{
		_componentNameLabel.Text = commodity.Name;
		_tradeSummaryLabel.Text = $"Price: {price} credits";
		_componentDescriptionLabel.Text = commodity.Description;
	}

	public void Reset()
	{
		_componentNameLabel.Text = "";
		_tradeSummaryLabel.Text = "";
		_componentDescriptionLabel.Text = "";
	}
}