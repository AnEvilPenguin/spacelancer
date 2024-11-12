using Godot;
using Spacelancer.Components.Economy.Commodities;

namespace Spacelancer.Scenes.UI.StationMenu.TradeMenu.TradeList;

public partial class TradeList : Control
{
	[Signal]
	public delegate void ItemSelectedEventHandler(int index);
	
	private Label _title;
	private ItemList _itemList;

	public override void _Ready()
	{
		_title = GetNode<Label>("%Title");
		_itemList = GetNode<ItemList>("%ItemList");
		
		_itemList.ItemSelected += OnItemSelected;
	}

	public void SetTitle(string title) =>
		_title.Text = title;

	public int AddItemToList(Commodity item, int price)
	{
		var message = $"{item.Name} - {price} credits";
		
		return _itemList.AddItem(message, item.Texture);
	}
	
	public void RemoveItemFromList(int index) =>
		_itemList.RemoveItem(index);
	
	public void ClearItemList() =>
		_itemList.Clear();

	public void DeselectItems() =>
		_itemList.DeselectAll();

	private void OnItemSelected(long value) =>
		EmitSignal(SignalName.ItemSelected, (int)value);
}