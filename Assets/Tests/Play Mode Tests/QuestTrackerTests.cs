using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
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
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class QuestTrackerTests
    {
        private IGameManager container;

        private IQuestTracker target;

        private IQuestProvider questProvider;
        
        private ScriptingEngine scriptingEngine;

        private GameObject inventoryManager;

        private WorldInstance world;
        
        private Entity left;
        private Entity right;
        
        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            container = new GameObject("GameManager").AddComponent<GameManager>();

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

            questProvider = container.QuestProvider;
            target = container.QuestTracker;
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");
        }
        
        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = container.EntityTemplateHandler.Get("human");
            IGrowingValue level = Mock.Of<IGrowingValue>();
            ICulture culture = Mock.Of<ICulture>(
                c => c.GetNameForChain(
                         It.IsAny<int>(), 
                         It.IsAny<string>(), 
                         It.IsAny<int>()) == "NAME"
                && c.NameData == new[] { new NameData("NAME", new []{ 0, 1 }, new [] { "all" }, new int[0]) });

            List<ICulture> cultures = new List<ICulture> {culture};

            IJob job = Mock.Of<IJob>();
            IBioSex sex = Mock.Of<IBioSex>(s => s.Name == "female");
            IGender gender = Mock.Of<IGender>(g => g.Name == "female");
            ISexuality sexuality = Mock.Of<ISexuality>();
            IRomance romance = Mock.Of<IRomance>();

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
                Vector2Int.zero,
                sprites,
                world,
                new StandardDriver());

            left.PlayerControlled = true;
            
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
                Vector2Int.zero,
                sprites,
                world,
                new StandardDriver());

            container.EntityHandler.AddEntity(left);
            container.EntityHandler.AddEntity(right);

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
            IJoyAction action = Mock.Of<IJoyAction>();
            IQuest quest = Mock.Of<IQuest>(
                q => q.AdvanceStep() == true
                && q.FulfilsRequirements(left, action) == true
                && q.CompleteQuest(left) == true
                && q.IsComplete == true);
            
            target.AddQuest(left.GUID, quest);
            quest.StartQuest(left);
            
            //when
            target.PerformQuestAction(left, quest, action);

            //then
            Assert.That(target.GetQuestsForEntity(left.GUID), Is.Empty);

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