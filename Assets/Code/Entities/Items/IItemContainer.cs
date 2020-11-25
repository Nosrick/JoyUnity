using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemContainer
    {
        List<ItemInstance> GetContents();

        bool AddContents(ItemInstance actor);

        bool AddContents(IEnumerable<ItemInstance> actors);

        bool RemoveContents(ItemInstance actor);

        void Clear();
    }
}
