using JoyLib.Code.Entities.Items;
using MoonSharp.Interpreter;

namespace JoyLib.Code.Scripting
{
    public class MoonItem
    {
        protected ItemInstance m_Item;

        [MoonSharpHidden]
        public MoonItem(ItemInstance item)
        {
            m_Item = item;
        }

        [MoonSharpHidden]
        public ItemInstance ItemInstance
        {
            get
            {
                return m_Item;
            }
        }

        public int Value
        {
            get
            {
                return m_Item.ItemType.Value;
            }
        }
    }
}
