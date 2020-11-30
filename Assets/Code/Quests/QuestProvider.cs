using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using JoyLib.Code.World;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Scripting;
using UnityEngine;

namespace JoyLib.Code.Quests
{
    public class QuestProvider : IQuestProvider
    {
        protected IEntityRelationshipHandler EntityRelationshipHandler { get; set; }
        
        protected RNG Roller { get; set; }

        public List<IQuestAction> Actions { get; protected set; }
        
        protected static BagOfGoldHelper BagOfGoldHelper { get; set; }

        public QuestProvider(
            IEntityRelationshipHandler entityRelationshipHandler,
            ILiveItemHandler itemHandler,
            IItemFactory itemFactory,
            RNG roller)
        {
            BagOfGoldHelper = new BagOfGoldHelper(itemHandler, itemFactory);
            Roller = roller;
            EntityRelationshipHandler = entityRelationshipHandler;

            Actions = ScriptingEngine.instance.FetchAndInitialiseChildren<IQuestAction>().ToList();
        }

        protected void Initialise()
        {
            EntityRelationshipHandler = GlobalConstants.GameManager.RelationshipHandler;

            Actions = ScriptingEngine.instance.FetchAndInitialiseChildren<IQuestAction>().ToList();
        }

        public IQuest MakeRandomQuest(IEntity questor, IEntity provider, WorldInstance overworldRef)
        {
            List<IQuestStep> steps = new List<IQuestStep>();

            //int numberOfSteps = RNG.instance.Roll(1, 4);
            int numberOfSteps = 1;
            for (int i = 0; i < numberOfSteps; i++)
            {
                int result = Roller.Roll(0, Actions.Count);
                IQuestAction action = Actions[result].Create(
                    new string[0],
                    new List<IItemInstance>(),
                    new List<IJoyObject>(),
                    new List<WorldInstance>());
                steps.Add(action.Make(questor, provider, overworldRef, action.Tags));
            }

            IEnumerable<string> tagsForAllSteps = steps.SelectMany(step => step.Tags);
            
            return new Quest(steps, QuestMorality.Neutral, GetRewards(questor, provider, steps), provider, tagsForAllSteps);
        }

        public IEnumerable<IQuest> MakeOneOfEachType(IEntity questor, IEntity provider, WorldInstance overworldRef)
        {
            List<IQuest> quests = new List<IQuest>();

            foreach (IQuestAction action in Actions)
            {
                IQuestAction newAction = action.Create(
                    new string[0],
                    new List<IItemInstance>(),
                    new List<IJoyObject>(),
                    new List<WorldInstance>());
                List<IQuestStep> steps = new List<IQuestStep>{newAction.Make(questor, provider, overworldRef, new string[0])};
                quests.Add(new Quest(steps, QuestMorality.Neutral, GetRewards(questor, provider, steps), provider, new string[0]));
            }

            return quests;
        }

        public IQuest MakeQuestOfType(IEntity questor, IEntity provider, WorldInstance overworldRef, string[] tags)
        {
            throw new System.NotImplementedException();
        }

        private List<IItemInstance> GetRewards(IEntity questor, IEntity provider, List<IQuestStep> steps)
        {
            List<IItemInstance> rewards = new List<IItemInstance>();
            int reward = ((steps.Count * 100) + (EntityRelationshipHandler.GetHighestRelationshipValue(provider, questor)));
            rewards.Add(BagOfGoldHelper.GetBagOfGold(reward));
            foreach (IItemInstance item in rewards)
            {
                item.SetOwner(questor.GUID);
            }

            return rewards;
        }
    }
}
