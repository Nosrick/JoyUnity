using DevionGames.UIWidgets;

namespace DevionGames.InventorySystem
{
    public class ItemTooltip : Tooltip
    {
        public void Show(Item item){
            Show(UnityTools.ColorString(item.DisplayName, item.Rarity.Color),item.Description,item.Icon,item.GetPropertyInfo());
        }
    }
}