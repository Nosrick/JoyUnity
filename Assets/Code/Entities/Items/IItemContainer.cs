using System.Collections.Generic;

namespace JoyLib.Code.Entities.Items
{
    public interface IItemContainer
    {
        List<IItemInstance> Contents { get; }

        bool AddContents(IItemInstance actor);

        bool AddContents(IEnumerable<IItemInstance> actors);

        bool RemoveContents(IItemInstance actor);

        void Clear();
        
        string ContentString { get; }
    }
}
