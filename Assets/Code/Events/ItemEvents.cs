using JoyLib.Code.Entities.Items;

namespace JoyLib.Code.Events
{
    public delegate void ItemRemovedEventHandler(IItemContainer sender, ItemChangedEventArgs args);

    public delegate void ItemAddedEventHandler(IItemContainer sender, ItemChangedEventArgs args);
    
    public class ItemChangedEventArgs
    {
        public IItemInstance Item { get; set; }
    }
}