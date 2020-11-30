using System;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities;

namespace JoyLib.Code.Quests
{
    public class ConcreteQuestStep : IQuestStep
    {
        protected string Description { get; set; }
        
        public List<string> Tags { get; protected set; }
        
        public ConcreteQuestStep(
            IQuestAction action, 
            List<IItemInstance> objects, 
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
            IEnumerable<string> tags)
        {
            this.Action = action;
            this.Items = objects;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = new List<string>(tags);
        }

        public override string ToString()
        { 
            return Description ?? (Description = Action.AssembleDescription());
        }

        public IQuestAction Action
        {
            get;
            protected set;
        }

        public List<IItemInstance> Items
        {
            get;
            protected set;
        }

        public List<IJoyObject> Actors
        {
            get;
            protected set;
        }

        public List<IWorldInstance> Areas
        {
            get;
            protected set;
        }

        public void StartQuest(IEntity questor)
        {
            Action.ExecutePrerequisites(questor);
        }
        
        public bool AddTag(string tag)
        {
            if (Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)) != false)
            {
                return false;
            }
            
            Tags.Add(tag);
            return true;
        }

        public bool RemoveTag(string tag)
        {
            if (!Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
            
            Tags.Remove(tag);
            return true;
        }

        public bool HasTag(string tag)
        {
            return Tags.Any(t => t.Equals(tag, StringComparison.OrdinalIgnoreCase));
        }
    }
}
