using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
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
        private IQuestTracker questTracker;

        private IQuestProvider target;
        
        private IGameManager gameManager;
        
        private ScriptingEngine scriptingEngine;

        private IWorldInstance world;
        
        private IEntity left;
        private IEntity right;
        
        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();

            world = Mock.Of<IWorldInstance>(
                w => w.GetRandomSentientWorldWide() == right
                && w.GetWorlds(It.IsAny<IWorldInstance>()) == new List<IWorldInstance>
                {
                    Mock.Of<IWorldInstance>(mock => mock.Name == "TEST2")
                }
                && w.Name == "TEST");

            IItemInstance item = Mock.Of<IItemInstance>();
            
            left = Mock.Of<IEntity>(
                entity => entity.GUID == 1
                          && entity.PlayerControlled == true
                          && entity.MyWorld == world
                          && entity.HasDataKey(It.IsAny<string>()) == false);
            
            right = Mock.Of<IEntity>(
                entity => entity.GUID == 2
                && entity.MyWorld == world
                && entity.Backpack == new List<IItemInstance> { item });

            Friendship friendship = new Friendship();
            friendship.AddParticipant(left);
            friendship.AddParticipant(right);
            
            IEntityRelationshipHandler relationshipHandler = Mock.Of<IEntityRelationshipHandler>(
                handler => handler.Get(It.IsAny<IJoyObject[]>(), It.IsAny<string[]>(), It.IsAny<bool>())
                == new IRelationship[] { friendship });
            ILiveItemHandler itemHandler = Mock.Of<ILiveItemHandler>();
            IItemFactory itemFactory = Mock.Of<IItemFactory>(
                factory => factory.CreateRandomItemOfType(
                               It.IsAny<string[]>(), 
                               It.IsAny<bool>(),
                               It.IsAny<GameObject>()) == item
                && factory.CreateSpecificType(
                    It.IsAny<string>(), 
                    It.IsAny<string[]>(), 
                    It.IsAny<bool>(), 
                    It.IsAny<GameObject>())== item
                && factory.CreateCompletelyRandomItem(
                    It.IsAny<bool>(), 
                    It.IsAny<bool>(),
                    It.IsAny<GameObject>()) == item);

            gameManager = Mock.Of<IGameManager>(
                manager => manager.ItemFactory == itemFactory);

            GlobalConstants.GameManager = gameManager;

            target = new QuestProvider(
                relationshipHandler,
                itemHandler,
                itemFactory,
                new RNG());
            questTracker = new QuestTracker();
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
            GlobalConstants.GameManager = null;
        }
    }
}