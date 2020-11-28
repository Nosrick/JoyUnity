using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
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
        private IGameManager container;

        private IQuestTracker questTracker;

        private IQuestProvider target;
        
        private ScriptingEngine scriptingEngine;
        private GameObject inventoryManager;

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
            
            container = new GameObject("GameManager").AddComponent<GameManager>();

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

            target = container.QuestProvider;
            questTracker = container.QuestTracker;
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");
        }
        
        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = container.EntityTemplateHandler.Get("human");
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

            Sprite[] sprites = container.ObjectIconHandler.GetDefaultSprites();
            
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

            container.EntityHandler.AddEntity(left);
            container.EntityHandler.AddEntity(right);

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
            IQuest quest = target.MakeRandomQuest(left, right, world);

            //then
            Assert.That(quest.Rewards, Is.Not.Empty);
            Assert.That(quest.Steps, Is.Not.Empty);

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container.MyGameObject);
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}