using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using UnityEngine;

namespace JoyLib.Code.Quests
{
    public class DeliverQuestAction : IQuestAction
    {
        public string[] Tags { get; protected set; }

        public string Description { get; protected set; }
        
        public List<IItemInstance> Items { get; protected set; }
        public List<IJoyObject> Actors { get; protected set; }
        public List<WorldInstance> Areas { get; protected set; }

        protected ItemFactory ItemFactory { get; set; }

        public RNG Roller { get; protected set; }

        public DeliverQuestAction()
        {
        }
        
        public DeliverQuestAction(
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas,
             IEnumerable<string> tags,
            ItemFactory itemFactory = null,
            RNG roller = null)
        {
            List<string> tempTags = new List<string>();
            tempTags.Add("deliver");
            tempTags.AddRange(tags);
            this.Items = items;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = tempTags.ToArray();
            Description = AssembleDescription();

            Roller = roller is null ? new RNG() : roller; 
            ItemFactory = itemFactory is null ? GlobalConstants.GameManager.ItemFactory : itemFactory;
        }
        
        public IQuestStep Make(Entity questor, Entity provider, WorldInstance overworld, IEnumerable<string> tags)
        {
            IItemInstance deliveryItem = null;
            IItemInstance[] backpack = provider.Backpack;
            if (backpack.Length > 0)
            {
                int result = Roller.Roll(0, backpack.Length);

                deliveryItem = backpack[result];
            }
            Entity endPoint = overworld.GetRandomSentientWorldWide();
            if(deliveryItem == null)
            {
                deliveryItem = ItemFactory.CreateCompletelyRandomItem();
            }

            this.Items = new List<IItemInstance> {deliveryItem};
            this.Actors = new List<IJoyObject> {endPoint};
            this.Areas = new List<WorldInstance>();

            IQuestStep step = new ConcreteQuestStep(
                this, 
                this.Items, 
                this.Actors, 
                this.Areas,
                this.Tags);
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
            if (action.Name.Equals("giveitemaction", StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            if (action.LastParticipants.Intersect(Actors).Count() != Actors.Count)
            {
                return false;
            }

            if (action.LastArgs.Intersect(Items).Count() != Items.Count)
            {
                return false;
            }

            return action.Successful;
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

        public IQuestAction Create(IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas)
        {
            return new DeliverQuestAction(
                items,
                actors,
                areas,
                tags);
        }
    }
}