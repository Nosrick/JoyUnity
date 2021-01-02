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
        public List<IWorldInstance> Areas { get; protected set; }

        protected IItemFactory ItemFactory { get; set; }

        public RNG Roller { get; protected set; }

        public DeliverQuestAction()
        {
        }
        
        public DeliverQuestAction(
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
             IEnumerable<string> tags,
            IItemFactory itemFactory = null,
            RNG roller = null)
        {
            List<string> tempTags = new List<string>();
            tempTags.Add("deliver");
            tempTags.AddRange(tags);
            this.Items = items;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = tempTags.ToArray();
            this.Description = this.AssembleDescription();

            this.Roller = roller is null ? new RNG() : roller;
            this.ItemFactory = itemFactory is null || GlobalConstants.GameManager is null == false ? GlobalConstants.GameManager.ItemFactory : itemFactory;
        }
        
        public IQuestStep Make(IEntity questor, IEntity provider, IWorldInstance overworld, IEnumerable<string> tags)
        {
            IItemInstance deliveryItem = null;
            List<IItemInstance> backpack = provider.Backpack;
            if (backpack.Count > 0)
            {
                int result = this.Roller.Roll(0, backpack.Count);

                deliveryItem = backpack[result];
            }
            IEntity endPoint = overworld.GetRandomSentientWorldWide();
            if(deliveryItem == null)
            {
                deliveryItem = this.ItemFactory.CreateCompletelyRandomItem();
            }

            this.Items = new List<IItemInstance> {deliveryItem};
            this.Actors = new List<IJoyObject> {endPoint};
            this.Areas = new List<IWorldInstance>();

            IQuestStep step = new ConcreteQuestStep(
                this, 
                this.Items, 
                this.Actors, 
                this.Areas,
                this.Tags);
            return step;
        }

        public void ExecutePrerequisites(IEntity questor)
        {
            foreach (IItemInstance item in this.Items)
            {
                questor.AddContents(item);
            }
        }

        public bool ExecutedSuccessfully(IJoyAction action)
        {
            if (action.LastTags.Any(tag => tag.Equals("item", StringComparison.OrdinalIgnoreCase)) == false)
            {
                Debug.Log("NO ITEM TAG");
                return false;
            }

            if (action.LastTags.Any(tag =>
                tag.Equals("trade", StringComparison.OrdinalIgnoreCase)
                || tag.Equals("give", StringComparison.OrdinalIgnoreCase)) == false)
            {
                Debug.Log("NO TRADE/GIVE TAG");
                return false;
            }

            if (action.LastParticipants.Intersect(this.Actors).Count() != this.Actors.Count)
            {
                Debug.Log("ACTORS DO NOT MATCH");
                Debug.Log("ACTORS IN QUEST");
                foreach (IJoyObject actor in this.Actors)
                {
                    Debug.Log(actor.ToString());
                }
                Debug.Log("ACTORS FOUND");
                foreach (IJoyObject actor in action.LastParticipants)
                {
                    Debug.Log(actor.ToString());
                }
                return false;
            }

            if (action.LastArgs.Length == 0)
            {
                return false;
            }

            List<IItemInstance> items = new List<IItemInstance>();
            foreach (object obj in action.LastArgs)
            {
                if (obj is IEnumerable<IItemInstance> toAdd)
                {
                    items.AddRange(toAdd);
                }
                else if (obj is IItemInstance item)
                {
                    items.Add(item);
                }
            }

            return items.Intersect(this.Items).Count() == this.Items.Count && action.Successful;
        }

        public string AssembleDescription()
        {
            StringBuilder itemBuilder = new StringBuilder();
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (i > 0 && i < this.Items.Count - 1)
                {
                    itemBuilder.Append(", ");
                }
                if (this.Items.Count > 1 && i == this.Items.Count - 1)
                {
                    itemBuilder.Append("and ");
                }
                itemBuilder.Append(this.Items[i].JoyName);
            }
            
            StringBuilder actorBuilder = new StringBuilder();
            for(int i = 0; i < this.Actors.Count; i++)
            {
                if (i > 0 && i < this.Actors.Count - 1)
                {
                    actorBuilder.Append(", ");
                }
                if (this.Actors.Count > 1 && i == this.Actors.Count - 1)
                {
                    actorBuilder.Append("or ");
                }
                actorBuilder.Append(this.Actors[i].JoyName);
                actorBuilder.Append(" in ");
                actorBuilder.Append(this.Actors[i].MyWorld.Name);
            }

            return "Deliver " + itemBuilder.ToString() + " to " + actorBuilder.ToString() + ".";
        }

        public IQuestAction Create(IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
            IItemFactory itemFactory = null)
        {
            return new DeliverQuestAction(
                items,
                actors,
                areas,
                tags, 
                itemFactory);
        }
    }
}