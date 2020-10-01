using System.Collections.Generic;
using DevionGames.InventorySystem;

namespace JoyLib.Code.Unity
{
    public class MutableItemContainer : ItemContainer
    {
        public List<Slot> Slots => this.m_Slots;
    }
}