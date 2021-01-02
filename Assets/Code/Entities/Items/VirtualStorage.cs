﻿using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Events;

namespace JoyLib.Code.Entities.Items
{
    public class VirtualStorage : JoyObject, IItemContainer
    {
        protected List<IItemInstance> m_Contents;
        public IEnumerable<IItemInstance> Contents => this.m_Contents;

        public VirtualStorage()
        {
            this.m_Contents = new List<IItemInstance>();
        }
        
        public bool Contains(IItemInstance actor)
        {
            return this.Contents.Contains(actor);
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
            
            this.m_Contents.Add(actor);
                
            this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            return true;

        }

        public bool AddContents(IEnumerable<IItemInstance> actors)
        {
            this.m_Contents.AddRange(actors.Where(actor => this.Contents.Any(item => item.GUID == actor.GUID) == false));
            
            foreach (IItemInstance actor in actors)
            {
                this.ItemAdded?.Invoke(this, new ItemChangedEventArgs() { Item = actor });
            }

            return true;
        }

        public bool RemoveContents(IItemInstance actor)
        {
            if (!this.m_Contents.Remove(actor))
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