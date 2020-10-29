using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;

namespace JoyLib.Code.Quests
{
    public class DeliverQuestAction : IQuestAction
    {
        public string[] Tags { get; protected set; }

        public string Description { get; protected set; }
        
        public List<ItemInstance> Items { get; protected set; }
        public List<IJoyObject> Actors { get; protected set; }
        public List<WorldInstance> Areas { get; protected set; }

        protected static ItemFactory ItemFactory { get; set; }

        public DeliverQuestAction(
            List<ItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas,
             string[] tags)
        {
            this.Items = items;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = tags;
            Description = AssembleDescription();

            if (ItemFactory is null)
            {
                ItemFactory = new ItemFactory();
            }
        }
        
        public IQuestStep Make(Entity provider, WorldInstance overworld)
        {
            ItemInstance deliveryItem = null;
            ItemInstance[] backpack = provider.Backpack;
            if (backpack.Length > 0)
            {
                int result = RNG.instance.Roll(0, backpack.Length);

                deliveryItem = backpack[result];
            }
            Entity endPoint = overworld.GetRandomSentientWorldWide();
            if(deliveryItem == null)
            {
                deliveryItem = ItemFactory.CreateCompletelyRandomItem();
            }

            IQuestStep step = new ConcreteQuestStep(
                this, 
                new List<ItemInstance> { deliveryItem }, 
                new List<IJoyObject> { endPoint }, 
                new List<WorldInstance>());
            return step;
        }

        public void ExecutePrerequisites(Entity questor)
        {
            foreach (ItemInstance item in Items)
            {
                questor.AddContents(item);
            }
        }

        public bool ExecutedSuccessfully(IJoyAction action)
        {
            if (action.Name.Equals("giveitemaction") == false)
            {
                return false;
            }

            return action.LastParticipants.Intersect(this.Actors).Count() == this.Actors.Count
                   && action.LastTags.Intersect(this.Tags).Count() == this.Tags.Length
                   && action.LastArgs.Intersect(this.Items).Count() == this.Items.Count;
        }

        public string AssembleDescription()
        {
            StringBuilder itemBuilder = new StringBuilder();
            for (int i = 0; i < Items.Count; i++)
            {
                if (i > 0 && i < Items.Count - 1)
                {
                    itemBuilder.Append(", ");
                }
                if (Items.Count > 1 && i == Items.Count - 1)
                {
                    itemBuilder.Append("and ");
                }
                itemBuilder.Append(Items[i].JoyName);
            }
            
            StringBuilder actorBuilder = new StringBuilder();
            for(int i = 0; i < Actors.Count; i++)
            {
                if (i > 0 && i < Actors.Count - 1)
                {
                    actorBuilder.Append(", ");
                }
                if (Actors.Count > 1 && i == Actors.Count - 1)
                {
                    actorBuilder.Append("or ");
                }
                actorBuilder.Append(Actors[i].JoyName);
                actorBuilder.Append(" in ");
                actorBuilder.Append(Actors[i].MyWorld.Name);
            }

            return "Deliver " + itemBuilder.ToString() + " to " + actorBuilder.ToString() + ".";
        }
    }
}