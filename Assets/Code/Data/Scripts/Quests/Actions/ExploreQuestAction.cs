using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Sirenix.OdinSerializer;

namespace JoyLib.Code.Quests
{
    [Serializable]
    public class ExploreQuestAction : IQuestAction
    {
        [OdinSerialize]
        public string[] Tags { get; protected set; }
        [OdinSerialize]
        public string Description { get; protected set; }
        
        [OdinSerialize]
        public List<long> Items { get; protected set; }
        
        [OdinSerialize]
        public List<long> Actors { get; protected set; }
        
        [OdinSerialize]
        public List<long> Areas { get; protected set; }

        [OdinSerialize]
        public RNG Roller { get; protected set; }

        public ExploreQuestAction()
        {}
        
        public ExploreQuestAction(
            IEnumerable<IItemInstance> items,
            IEnumerable<IJoyObject> actors,
            IEnumerable<IWorldInstance> areas,
            IEnumerable<string> tags,
            RNG roller = null)
        {
            this.Roller = roller is null ? new RNG() : roller; 
            List<string> tempTags = new List<string>();
            tempTags.Add("exploration");
            tempTags.AddRange(tags);
            this.Items = items.Select(instance => instance.GUID).ToList();
            this.Actors = actors.Select(instance => instance.GUID).ToList();
            this.Areas = areas.Select(instance => instance.GUID).ToList();
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

            this.Items = new List<long>();
            this.Actors = new List<long>();
            this.Areas = new List<long> { worlds[result].GUID };

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

            foreach (object obj in action.LastArgs)
            {
                if (obj is IWorldInstance world)
                {
                    if (this.Areas.Contains(world.GUID) == false)
                    {
                        return false;
                    }
                }
            }

            IWorldInstance overworld = GlobalConstants.GameManager.Player.MyWorld.GetOverworld();
            List<IWorldInstance> worlds = overworld.GetWorlds(overworld)
                .Where(instance => this.Areas.Contains(instance.GUID))
                .ToList();
            return worlds.All(world => action.LastParticipants[0].HasDataKey(world.Name)) && action.Successful;
        }

        public string AssembleDescription()
        {
            StringBuilder builder = new StringBuilder();

            IWorldInstance overworld = GlobalConstants.GameManager.Player.MyWorld.GetOverworld();
            List<IWorldInstance> worlds = overworld.GetWorlds(overworld)
                .Where(instance => this.Areas.Contains(instance.GUID))
                .ToList();

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
                builder.Append(worlds[i].Name);
            }
            
            return "Go to " + builder + ".";
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