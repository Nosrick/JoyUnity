using System.Collections;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class QuestTrackerTests
    {
        private GameObject container;

        private QuestTracker target;

        private QuestProvider questProvider;
        
        private ScriptingEngine scriptingEngine;
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler relationshipHandler;

        private ObjectIconHandler objectIconHandler;

        private EntityBioSexHandler bioSexHandler;

        private EntitySexualityHandler sexualityHandler;

        private EntitySkillHandler skillHandler;

        private EntityFactory entityFactory;

        private LiveItemHandler itemHandler;

        private GameObject inventoryManager;

        private WorldInstance world;
        
        private Entity left;
        private Entity right;
        
        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            scriptingEngine = new ScriptingEngine();

            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            templateHandler = container.AddComponent<EntityTemplateHandler>();
            cultureHandler = container.AddComponent<CultureHandler>();
            needHandler = container.AddComponent<NeedHandler>();
            relationshipHandler = container.AddComponent<EntityRelationshipHandler>();
            materialHandler = container.AddComponent<MaterialHandler>();
            jobHandler = container.AddComponent<JobHandler>();
            bioSexHandler = container.AddComponent<EntityBioSexHandler>();
            sexualityHandler = container.AddComponent<EntitySexualityHandler>();
            skillHandler = container.AddComponent<EntitySkillHandler>();
            itemHandler = container.AddComponent<LiveItemHandler>();

            questProvider = container.AddComponent<QuestProvider>();
            target = container.AddComponent<QuestTracker>();

            entityFactory = new EntityFactory();
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");
        }
        
        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = templateHandler.Get("human");
            IGrowingValue level = new ConcreteGrowingValue(
                "level",
                1,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                0f,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(),
                new NonUniqueDictionary<INeed, float>());
            
            left = entityFactory.CreateFromTemplate(
                random,
                level,  
                Vector2Int.down);

            right = entityFactory.CreateFromTemplate(
                random,
                level,
                Vector2Int.up);

            world.AddEntity(left);
            world.AddEntity(right);
        }

        [UnityTest]
        public IEnumerator QuestTracker_Should_AddQuest()
        {
            //given

            //when
            target.AddQuest(left.GUID, questProvider.MakeRandomQuest(left, right, world));
            
            //then
            Assert.That(target.GetQuestsForEntity(left.GUID), Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator QuestTracker_Should_AdvanceOrCompleteQuest()
        {
            //given
            IQuest quest = questProvider.MakeRandomQuest(left, right, world);
            target.AddQuest(left.GUID, quest);
            quest.StartQuest(left);
            
            //when
            IJoyAction action = left.FetchAction("giveitemaction");
            action.Execute(
                new IJoyObject[] {left, right},
                new string[0], 
                left.Backpack[0]);
            target.PerformQuestAction(left, quest, action);

            //then
            Assert.That(quest.IsComplete, Is.True);
            Assert.That(target.GetQuestsForEntity(left.GUID), Is.Empty);

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container);
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}