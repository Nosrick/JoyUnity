using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemContainer
    {
        List<ItemInstance> GetContents();

        bool AddContents(ItemInstance actor);

        bool RemoveContents(ItemInstance actor);
    }
}
