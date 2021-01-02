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
    public class ExploreQuestAction : IQuestAction
    {
        public string[] Tags { get; protected set; }
        public string Description { get; protected set; }
        public List<IItemInstance> Items { get; protected set; }
        public List<IJoyObject> Actors { get; protected set; }
        public List<IWorldInstance> Areas { get; protected set; }

        public RNG Roller { get; protected set; }

        public ExploreQuestAction()
        {}
        
        public ExploreQuestAction(
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
            IEnumerable<string> tags,
            RNG roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller; 
            List<string> tempTags = new List<string>();
            tempTags.Add("exploration");
            tempTags.AddRange(tags);
            this.Items = items;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = tempTags.ToArray();
            this.Description = this.AssembleDescription();
        }
        
        public IQuestStep Make(IEntity questor, IEntity provider, IWorldInstance overworld, IEnumerable<string> tags)
        {
            List<IWorldInstance> worlds = overworld.GetWorlds(overworld); 

            int result = this.Roller.Roll(0, worlds.Count);

            int breakout = 0;

            while (questor.HasDataKey(worlds[result].Name) && breakout < worlds.Count)
            {
                result++;
                result %= worlds.Count;

                breakout++;
            }

            if (breakout == worlds.Count)
            {
                throw new InvalidOperationException(questor.JoyName + " has explored the whole world!");
            }

            this.Items = new List<IItemInstance>();
            this.Actors = new List<IJoyObject>();
            this.Areas = new List<IWorldInstance> { worlds[result] };

            IQuestStep step = new ConcreteQuestStep(
                this, 
                this.Items, 
                this.Actors, 
                this.Areas,
                this.Tags);
            return step;
        }

        public bool ExecutedSuccessfully(IJoyAction action)
        {
            if (action.Name.Equals("enterworldaction", StringComparison.OrdinalIgnoreCase) == false)
            {
                return false;
            }

            if (action.LastArgs.Intersect(this.Areas).Count() != this.Areas.Count)
            {
                return false;
            }

            if (this.Areas.Any(world => !action.LastParticipants[0].HasDataKey(world.Name)))
            {
                return false;
            }

            return action.Successful;
        }

        public string AssembleDescription()
        {
            StringBuilder builder = new StringBuilder();

            for(int i = 0; i < this.Areas.Count; i++)
            {
                if (i > 0 && i < this.Items.Count - 1)
                {
                    builder.Append(", ");
                }
                if (this.Items.Count > 1 && i == this.Items.Count - 1)
                {
                    builder.Append("and ");
                }
                builder.Append(this.Areas[i].Name);
            }
            
            return "Go to " + builder.ToString() + ".";
        }

        public void ExecutePrerequisites(IEntity questor)
        {
        }

        public IQuestAction Create(IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<IWorldInstance> areas,
            IItemFactory itemFactory = null)
        {
            return new ExploreQuestAction(
                items,
                actors,
                areas,
                tags);
        }
    }
}