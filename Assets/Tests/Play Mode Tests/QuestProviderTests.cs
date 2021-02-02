using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class QuestProviderTests
    {
        private IQuestTracker questTracker;

        private IQuestProvider target;

        private IGameManager gameManager;

        private ScriptingEngine scriptingEngine;

        private IWorldInstance overworld;
        private IWorldInstance world;

        private IEntity left;
        private IEntity right;

        [SetUp]
        public void SetUp()
        {
            ActionLog actionLog = new ActionLog();
            GlobalConstants.ActionLog = actionLog;
            this.scriptingEngine = new ScriptingEngine();

            IItemInstance item = Mock.Of<IItemInstance>();

            this.overworld = Mock.Of<IWorldInstance>(
                w => w.Name == "overworld"
                     && w.GetWorlds(It.IsAny<IWorldInstance>()) == new List<IWorldInstance>
                     {
                         Mock.Of<IWorldInstance>(
                             instance => instance.GUID == 200)
                     }
                     && w.GetRandomSentientWorldWide() == Mock.Of<IEntity>(
                         entity => entity.GUID == 2
                                   && entity.MyWorld == this.world
                                   && entity.Contents == new List<IItemInstance> {item}));

            this.world = Mock.Of<IWorldInstance>(
                w => w.GetOverworld() == this.overworld
                     && w.Name == "TEST"
                     && w.GUID == 200);

            this.left = Mock.Of<IEntity>(
                entity => entity.GUID == 1
                          && entity.PlayerControlled == true
                          && entity.MyWorld == this.world
                          && entity.HasDataKey(It.IsAny<string>()) == false);

            this.right = Mock.Of<IEntity>(
                entity => entity.GUID == 2
                          && entity.MyWorld == this.world
                          && entity.Contents == new List<IItemInstance> {item});

            IRelationship friendship = Mock.Of<IRelationship>(
                relationship => relationship.GetRelationshipValue(It.IsAny<long>(), It.IsAny<long>()) == 0);

            IEntityRelationshipHandler relationshipHandler = Mock.Of<IEntityRelationshipHandler>(
                handler => handler.Get(It.IsAny<IJoyObject[]>(), It.IsAny<string[]>(), It.IsAny<bool>())
                           == new[] {friendship});
            ILiveItemHandler itemHandler = Mock.Of<ILiveItemHandler>();
            IItemFactory itemFactory = Mock.Of<IItemFactory>(
                factory => factory.CreateRandomItemOfType(
                               It.IsAny<string[]>(),
                               It.IsAny<bool>()) == item
                           && factory.CreateSpecificType(
                               It.IsAny<string>(),
                               It.IsAny<string[]>(),
                               It.IsAny<bool>()) == item
                           && factory.CreateCompletelyRandomItem(
                               It.IsAny<bool>(),
                               It.IsAny<bool>()) == item);

            this.gameManager = Mock.Of<IGameManager>(
                manager => manager.ItemFactory == itemFactory
                           && manager.Player == this.left);

            GlobalConstants.GameManager = this.gameManager;

            this.target = new QuestProvider(
                relationshipHandler,
                itemHandler,
                itemFactory,
                new RNG());
            this.questTracker = new QuestTracker();
        }

        [UnityTest]
        public IEnumerator QuestProvider_ShouldHave_NonZeroQuests()
        {
            //given

            //when

            //then
            Assert.That(this.target.Actions, Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator QuestProvider_ShouldGenerate_ValidQuest()
        {
            //given

            //when
            IQuest[] quests = this.target.MakeOneOfEachType(this.left, this.right, this.overworld).ToArray();

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
            GlobalConstants.ActionLog.Dispose();
        }
    }
}