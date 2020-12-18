using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class ConversationEngineTests
    {
        private ScriptingEngine scriptingEngine;

        private IConversationEngine target;

        private GameObject prefab;

        private IEntity instigator;
        private IEntity listener;

        private IWorldInstance world;

        [SetUp]
        public void SetUp()
        {
            prefab = Resources.Load<GameObject>("Prefabs/MonoBehaviourHandler");

            scriptingEngine = new ScriptingEngine();

            ILiveEntityHandler entityHandler = Mock.Of<ILiveEntityHandler>();

            JoyLib.Code.Entities.Relationships.Friendship friendship =
                new JoyLib.Code.Entities.Relationships.Friendship();
            
            IEntityRelationshipHandler relationshipHandler = Mock.Of<IEntityRelationshipHandler>(
                handler => handler.Get(It.IsAny<IJoyObject[]>(), It.IsAny<string[]>(), It.IsAny<bool>())
                           == new IRelationship[] {friendship});

            IEntity questObject = Mock.Of<IEntity>(
                entity => entity.JoyName == "NAME1" 
                    && entity.MyWorld == Mock.Of<IWorldInstance>(
                    w => w.GetRandomSentient() == Mock.Of<IEntity>(
                        e => e.JoyName == "NAME2")));

            world = Mock.Of<IWorldInstance>(
                w => w.GetRandomSentientWorldWide() == questObject
                     && w.GetRandomSentient() == questObject
                     && w.GetWorlds(It.IsAny<IWorldInstance>()) == new List<IWorldInstance>
                     {
                         Mock.Of<IWorldInstance>(mock => mock.Name == "TEST2")
                     }
                     && w.GetOverworld() == Mock.Of<IWorldInstance>(
                         mock => mock.Name == "EVERSE"
                         && mock.GetRandomSentient() == questObject
                         && mock.GetRandomSentientWorldWide() == questObject)
                     && w.Name == "TEST");

            INeed friendshipMock = Mock.Of<INeed>(
                need => need.Fulfill(It.IsAny<int>()) == 1
                        && need.Name == "friendship");
            IDictionary<string, INeed> needs = new Dictionary<string, INeed>();
            needs.Add("friendship", friendshipMock);
            
            IRollableValue<int> mockPersonality = Mock.Of<IRollableValue<int>>(
                value => value.Value == 4
                         && value.Name == "personality");
            IDictionary<string, IRollableValue<int>> stats = new Dictionary<string, IRollableValue<int>>();
            stats.Add(
                "personality", 
                new EntityStatistic(
                    "personality",
                    4,
                    GlobalConstants.DEFAULT_SUCCESS_THRESHOLD));

            instigator = Mock.Of<IEntity>(
                entity => entity.PlayerControlled == true
                          && entity.MyWorld == world
                          && entity.Needs == needs
                          && entity.Statistics == stats
                          && entity.Sentient == true
                          && entity.GUID == 1);

            listener = Mock.Of<IEntity>(entity => entity.MyWorld == world
                                                  && entity.Needs == needs
                                                  && entity.Statistics == stats
                                                  && entity.Sentient == true
                                                  && entity.GUID == 2);

            target = new ConversationEngine(relationshipHandler);

            TopicData.ConversationEngine = target;
            TopicData.RelationshipHandler = relationshipHandler;

            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.Player == instigator);
            
            friendship.AddParticipant(listener);
            friendship.AddParticipant(instigator);
        }

        [UnityTest]
        public IEnumerator LoadData_ShouldNotBeEmpty()
        {
            //given

            //when
            ITopic[] topics = target.AllTopics;

            //then
            Assert.That(topics, Is.Not.Empty);
            foreach (ITopic topic in topics)
            {
                Assert.That(topic.Words.Length, Is.GreaterThan(0));
                Assert.That(topic.ID.Length, Is.GreaterThan(0));
            }

            return null;
        }

        [UnityTest]
        public IEnumerator Converse_ShouldCompleteConversation()
        {
            int depth = 0;

            target.SetActors(instigator, listener);

            ITopic[] baseTopics = target.Converse();
            bool ended = false;
            foreach (ITopic topic in baseTopics)
            {
                ended = AdvanceToEnd(topic, baseTopics);
            }

            Assert.That(ended, Is.True);

            return null;
        }

        private bool AdvanceToEnd(ITopic topic, ITopic[] baseTopics)
        {
            ITopic[] nextTopics = target.Converse(topic.ID);
            if (nextTopics.Intersect(baseTopics).Count() == baseTopics.Length)
            {
                return true;
            }

            foreach (ITopic next in nextTopics)
            {
                AdvanceToEnd(next, baseTopics);
            }

            return true;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
        }
    }
}