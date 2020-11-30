using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        private IGameManager GameManager;
        
        private IConversationEngine target;

        private IGUIManager GUIManager;
        private IEntityRelationshipHandler RelationshipHandler;
        private IObjectIconHandler ObjectIconHandler;
        private INeedHandler NeedHandler;
        private ILiveEntityHandler EntityHandler;
        
        private GameObject conversationWindow;
        private GameObject inventoryWindow;
        private GameObject tradeWindow;

        private GameObject inventoryManager;

        private MonoBehaviourHandler instigatorObject;
        private MonoBehaviourHandler listenerObject;

        private GameObject prefab;
        
        private Entity instigator;
        private Entity listener;

        private Canvas canvas;

        private WorldInstance world;
        
        [SetUp]
        public void SetUp()
        {
            prefab = Resources.Load<GameObject>("Prefabs/MonoBehaviourHandler");
            
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            scriptingEngine = new ScriptingEngine();
            
            canvas = new GameObject("Parent").AddComponent<Canvas>();

            conversationWindow =
                GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"), 
                    canvas.transform, 
                    true);
            conversationWindow.name = "Conversation Window";
            
            GUIManager = new GUIManager();

            ObjectIconHandler = new ObjectIconHandler(new RNG());

            NeedHandler = new NeedHandler();
            IEntitySkillHandler skillHandler = new EntitySkillHandler(NeedHandler);

            EntityHandler = Mock.Of<ILiveEntityHandler>();
            
            IEntityTemplateHandler templateHandler = new EntityTemplateHandler(skillHandler);

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
            
            GUIManager.AddGUI(conversationWindow, true, true);
            GUIManager.AddGUI(inventoryWindow);
            GUIManager.AddGUI(tradeWindow);
            GUIManager.OpenGUI(conversationWindow.name);

            target = new ConversationEngine(RelationshipHandler, GUIManager, conversationWindow);

            GameManager = Mock.Of<IGameManager>(
                manager => manager.RelationshipHandler == RelationshipHandler
                           && manager.ConversationEngine == target
                           && manager.EntityTemplateHandler == templateHandler
                           && manager.ObjectIconHandler == ObjectIconHandler
                           && manager.NeedHandler == NeedHandler
                           && manager.SkillHandler == skillHandler);

            GlobalConstants.GameManager = GameManager;
            
            GUIData.GUIManager = GUIManager;
            JoyItemSlot.ItemHolder = new GameObject("World Objects");
            JoyItemSlot.ConversationEngine = target;
            JoyItemSlot.GUIManager = GUIManager;
            TradeWindow.RelationshipHandler = RelationshipHandler;
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING",
                EntityHandler,
                new RNG());

            IEntityTemplate template = templateHandler.Get("human");
            IGrowingValue level = new ConcreteGrowingValue(
                "level",
                1,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                0f,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(new RNG()),
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

            Sprite[] sprites = ObjectIconHandler.GetDefaultSprites();

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>(NeedHandler.GetManyRandomised(template.Needs));

            instigator = new Entity(
                template,
                needs, 
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

            listener = new Entity(
                template,
                needs, 
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

            instigator.PlayerControlled = true;

            world.AddEntity(instigator);
            world.AddEntity(listener);

            instigator.MyWorld = world;
            listener.MyWorld = world;
            
            instigatorObject = GameObject.Instantiate(prefab).GetComponent<MonoBehaviourHandler>();
            instigatorObject.AttachJoyObject(instigator);
            
            listenerObject = GameObject.Instantiate(prefab).GetComponent<MonoBehaviourHandler>();
            listenerObject.AttachJoyObject(listener);
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
            GameObject.DestroyImmediate(inventoryManager);
            GameObject.DestroyImmediate(canvas);
            GameObject.DestroyImmediate(listenerObject.gameObject);
            GameObject.DestroyImmediate(instigatorObject.gameObject);
        }
    }
}
