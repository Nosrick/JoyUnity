using JoyLib.Code.Entities.Items;
using JoyLib.Code.World;
using System.Collections.Generic;
using JoyLib.Code.Entities;

namespace JoyLib.Code.Quests
{
    public class ConcreteQuestStep : IQuestStep
    {
        protected string Description { get; set; }
        
        public ConcreteQuestStep(
            IQuestAction action, 
            List<ItemInstance> objects, 
            List<IJoyObject> actors,
            List<WorldInstance> areas)
        {
            this.Action = action;
            this.Items = objects;
            this.Actors = actors;
            this.Areas = areas;
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

        public List<ItemInstance> Items
        {
            get;
            protected set;
        }

        public List<IJoyObject> Actors
        {
            get;
            protected set;
        }

        public List<WorldInstance> Areas
        {
            get;
            protected set;
        }

        public void StartQuest(Entity questor)
        {
            Action.ExecutePrerequisites(questor);
        }
    }
}
