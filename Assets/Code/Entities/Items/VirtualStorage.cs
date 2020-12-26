using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Events;
using JoyLib.Code.Managers;

namespace JoyLib.Code.Entities.Items
{
    public class VirtualStorage : JoyObject, IItemContainer
    {
        public List<IItemInstance> Contents { get; protected set; }

        public VirtualStorage()
        {
            this.Contents = new List<IItemInstance>();
            this.GUID = GUIDManager.Instance.AssignGUID();
        }
        
        public bool Contains(IItemInstance actor)
        {
            return Contents.Contains(actor);
        }

        public bool CanAddContents(IItemInstance actor)
        {
            return this.GUID != actor.GUID && !this.Contains(actor);
        }

        public bool AddContents(IItemInstance actor)
        {
            if (!this.CanAddContents(actor))
            {
                return false;
            }
            
            this.Contents.Add(actor);
                
            this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            return true;

        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            this.Contents.AddRange(actors.Where(actor => this.Contents.Any(item => item.GUID == actor.GUID) == false));
            
            foreach (IItemInstance actor in actors)
            {
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            }

            return true;
        }

        public bool RemoveContents(IItemInstance actor)
        {
            if (!this.Contents.Remove(actor))
            {
                return false;
            }
            this.ItemRemoved?.Invoke(this, new ItemChangedEventArgs() { Item = actor });

            return true;
        }

        public void Clear()
        {
            List<IItemInstance> copy = new List<IItemInstance>(this.Contents);
            foreach (IItemInstance item in copy)
            {
                this.RemoveContents(item);
            }
        }

        public string ContentString { get; }
        public event ItemRemovedEventHandler ItemRemoved;
        public event ItemAddedEventHandler ItemAdded;
    }
}