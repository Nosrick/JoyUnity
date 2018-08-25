using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemContainer
    {
        List<ItemInstance> GetContents();
    }
}
