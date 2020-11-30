using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class QuestProviderTests
    {
        private IGameManager gameManager;
        
        private IQuestTracker questTracker;

        private IQuestProvider target;
        
        private ScriptingEngine scriptingEngine;
        private GameObject inventoryManager;

        private ILiveEntityHandler EntityHandler;
        private IEntityRelationshipHandler RelationshipHandler;
        private IEntityTemplateHandler TemplateHandler;

        private INeedHandler NeedHandler;
        private IEntitySkillHandler SkillHandler;

        private WorldInstance world;

        private Canvas canvas;
        private GameObject conversationWindow;
        
        private Entity left;
        private Entity right;
        
        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            
            canvas = new GameObject("Parent").AddComponent<Canvas>();

            conversationWindow =
                GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"), 
                    canvas.transform, 
                    true);
            conversationWindow.name = "Conversation Window";

            scriptingEngine = new ScriptingEngine();
            
            NeedHandler = new NeedHandler();
            SkillHandler = new EntitySkillHandler(NeedHandler);
            EntityHandler = new LiveEntityHandler();
            RelationshipHandler = new EntityRelationshipHandler();
            TemplateHandler = new EntityTemplateHandler(SkillHandler);

            IItemFactory itemFactory = Mock.Of<IItemFactory>(
                factory => factory.CreateSpecificType(It.IsAny<string>(), It.IsAny<string[]>(), It.IsAny<bool>())
                           == Mock.Of<IItemInstance>(
                               item => item.Copy(It.IsAny<IItemInstance>()) == Mock.Of<IItemInstance>()));

            ILiveItemHandler itemHandler = Mock.Of<ILiveItemHandler>();

            target = new QuestProvider(
                RelationshipHandler,
                itemHandler,
                itemFactory,
                new RNG());
            questTracker = new QuestTracker();

            gameManager = Mock.Of<IGameManager>(
                manager => manager.EntityHandler == EntityHandler
                && manager.NeedHandler == NeedHandler
                && manager.SkillHandler == SkillHandler
                && manager.RelationshipHandler == RelationshipHandler
                && manager.EntityTemplateHandler == TemplateHandler
                && manager.ItemHandler == itemHandler
                && manager.ItemFactory == itemFactory
                && manager.QuestProvider == target
                && manager.QuestTracker == questTracker);

            GlobalConstants.GameManager = gameManager;
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING",
                EntityHandler,
                new RNG());
        }
        
        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = TemplateHandler.Get("human");
            IGrowingValue level = new ConcreteGrowingValue(
                "level",
                1,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                0f,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(),
                new NonUniqueDictionary<INeed, float>());

            ICulture culture = Mock.Of<ICulture>( c => c.GetNameForChain(
                                                           It.IsAny<int>(), 
                                                           It.IsAny<string>(), 
                                                           It.IsAny<int>()) == "NAME"
                                                       && c.NameData == new NameData[] { new NameData("NAME", new []{0, 1}, new []{"all"}, new int[0]) });
            List<ICulture> cultures = new List<ICulture> {culture};

            IGender gender = Mock.Of<IGender>(g => g.Name == "female");
            IBioSex sex = Mock.Of<IBioSex>(s => s.Name == "female"
                                                && s.CanBirth == true);
            ISexuality sexuality = Mock.Of<ISexuality>(s => s.WillMateWith(
                                                                It.IsAny<Entity>(), It.IsAny<Entity>(), It.IsAny<IRelationship[]>()) == true
                                                            && s.Tags == new List<string>());
            IRomance romance = Mock.Of<IRomance>(r => r.Compatible(
                It.IsAny<Entity>(), It.IsAny<Entity>(), It.IsAny<IRelationship[]>()) == true);
            IJob job = Mock.Of<IJob>();

            Sprite[] sprites = new Sprite[0];
            
            left = new Entity(
                random,
                new BasicValueContainer<INeed>(), 
                cultures,
                level,
                job,
                gender,
                sex,
                sexuality,
                romance,
                Vector2Int.down, 
                sprites,
                world,
                new StandardDriver());

            right = new Entity(
                random,
                new BasicValueContainer<INeed>(), 
                cultures,
                level,
                job,
                gender,
                sex,
                sexuality,
                romance,
                Vector2Int.down, 
                sprites,
                world,
                new StandardDriver());

            left.PlayerControlled = true;

            world.AddEntity(left);
            world.AddEntity(right);
        }

        [UnityTest]
        public IEnumerator QuestProvider_ShouldHave_NonZeroQuests()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.Actions, Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator QuestProvider_ShouldGenerate_ValidQuest()
        {
            //given
            
            //when
            IQuest[] quests = target.MakeOneOfEachType(left, right, world).ToArray();

            //then
            foreach (IQuest quest in quests)
            {
                Assert.That(quest.Rewards, Is.Not.Empty);
                Assert.That(quest.Steps, Is.Not.Empty);
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}