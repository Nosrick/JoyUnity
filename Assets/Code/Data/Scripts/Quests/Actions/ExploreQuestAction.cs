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
        public List<WorldInstance> Areas { get; protected set; }

        public RNG Roller { get; protected set; }

        public ExploreQuestAction()
        {}
        
        public ExploreQuestAction(
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas,
            IEnumerable<string> tags,
            RNG roller = null)
        {
            Roller = roller is null ? new RNG() : roller; 
            List<string> tempTags = new List<string>();
            tempTags.Add("exploration");
            tempTags.AddRange(tags);
            this.Items = items;
            this.Actors = actors;
            this.Areas = areas;
            this.Tags = tempTags.ToArray();
            Description = AssembleDescription();
        }
        
        public IQuestStep Make(Entity questor, Entity provider, WorldInstance overworld, IEnumerable<string> tags)
        {
            List<WorldInstance> worlds = overworld.GetWorlds(overworld); 

            int result = Roller.Roll(0, worlds.Count);

            int breakout = 0;

            WorldInstance chosenWorld = null;

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
            this.Areas = new List<WorldInstance> { worlds[result] };

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

            if (action.LastArgs.Intersect(Areas).Count() != Areas.Count)
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
                if (i > 0 && i < Items.Count - 1)
                {
                    builder.Append(", ");
                }
                if (Items.Count > 1 && i == Items.Count - 1)
                {
                    builder.Append("and ");
                }
                builder.Append(Areas[i].Name);
            }
            
            return "Go to " + builder.ToString() + ".";
        }

        public void ExecutePrerequisites(Entity questor)
        {
        }

        public IQuestAction Create(IEnumerable<string> tags,
            List<IItemInstance> items,
            List<IJoyObject> actors,
            List<WorldInstance> areas)
        {
            return new ExploreQuestAction(
                items,
                actors,
                areas,
                tags);
        }
    }
}