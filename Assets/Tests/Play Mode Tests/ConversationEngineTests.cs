using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using Castle.Core.Internal;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
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
using JoyLib.Code.Helpers;
using JoyLib.Code.Quests;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity;
using JoyLib.Code.Unity.GUI;
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

        private IGUIManager GUIManager;

        private GameObject conversationWindow;
        private GameObject inventoryWindow;
        private GameObject tradeWindow;

        private GameObject prefab;

        private IEntity instigator;
        private IEntity listener;

        private Canvas canvas;

        private IWorldInstance world;

        [SetUp]
        public void SetUp()
        {
            prefab = Resources.Load<GameObject>("Prefabs/MonoBehaviourHandler");

            scriptingEngine = new ScriptingEngine();

            canvas = new GameObject("Parent").AddComponent<Canvas>();

            conversationWindow =
                GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"),
                    canvas.transform,
                    true);
            conversationWindow.name = "Conversation Window";

            GUIManager = new GUIManager();

            inventoryWindow = GameObject.Instantiate(
                Resources.Load<GameObject>("Prefabs/GUI/Inventory/Inventory"),
                canvas.transform,
                true);
            inventoryWindow.name = "Inventory";

            tradeWindow = GameObject.Instantiate(
                Resources.Load<GameObject>("Prefabs/GUI/Trade"),
                canvas.transform,
                true);
            tradeWindow.name = "Trade";

            GUIManager.AddGUI(conversationWindow.GetComponent<GUIData>(), true, true);
            GUIManager.AddGUI(inventoryWindow.GetComponent<GUIData>());
            GUIManager.AddGUI(tradeWindow.GetComponent<GUIData>());
            GUIManager.OpenGUI(conversationWindow.name);

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

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed> { Mock.Of<INeed>(
                need => need.Fulfill(It.IsAny<int>()) == 1
                        && need.Name == "friendship") };
            
            BasicValueContainer<IRollableValue> stats = new BasicValueContainer<IRollableValue> { Mock.Of<IRollableValue>(
                value => value.Value == 4
                         && value.Name == "personality") };

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

            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.RelationshipHandler == relationshipHandler
                        && manager.ConversationEngine == target
                        && manager.Player == instigator);

            GlobalConstants.GameManager = gameManager;

            target = new ConversationEngine(relationshipHandler, GUIManager, conversationWindow);

            JoyItemSlot.ItemHolder = new GameObject("World Objects");
            JoyItemSlot.ConversationEngine = target;
            JoyItemSlot.GUIManager = GUIManager;
            TradeWindow.RelationshipHandler = relationshipHandler;
            
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
            GameObject.DestroyImmediate(canvas);
            GlobalConstants.GameManager = null;
        }
    }
}