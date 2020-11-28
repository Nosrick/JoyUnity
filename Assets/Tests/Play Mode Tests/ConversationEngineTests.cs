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
        
        private ConversationEngine target;

        private GameObject gameManager;
        private GameObject conversationWindow;
        private GameObject inventoryWindow;
        private GameObject tradeWindow;
        
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler entityRelationshipHandler;

        private EntitySkillHandler skillHandler;

        private ObjectIconHandler objectIconHandler;

        private GUIManager guiManager;

        private GameObject inventoryManager;

        private QuestTracker questTracker;
        private QuestProvider questProvider;

        private ParameterProcessorHandler parameterProcessorHandler;

        private LiveEntityHandler entityHandler;
        private LiveItemHandler itemHandler;

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
            gameManager = new GameObject("GameManager");

            GlobalConstants.GameManager = gameManager;
            
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            
            canvas = new GameObject("Parent").AddComponent<Canvas>();

            conversationWindow =
                GameObject.Instantiate(
                    Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"), 
                    canvas.transform, 
                    true);
            conversationWindow.name = "Conversation Window";

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

            objectIconHandler = gameManager.AddComponent<ObjectIconHandler>();
            templateHandler = gameManager.AddComponent<EntityTemplateHandler>();
            needHandler = gameManager.AddComponent<NeedHandler>();
            skillHandler = gameManager.AddComponent<EntitySkillHandler>();
            entityRelationshipHandler = gameManager.AddComponent<EntityRelationshipHandler>();
            guiManager = gameManager.AddComponent<GUIManager>();
            materialHandler = gameManager.AddComponent<MaterialHandler>();

            parameterProcessorHandler = gameManager.AddComponent<ParameterProcessorHandler>();

            questProvider = gameManager.AddComponent<QuestProvider>();
            questTracker = gameManager.AddComponent<QuestTracker>();

            entityHandler = gameManager.AddComponent<LiveEntityHandler>();

            conversationWindow =
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"));
            conversationWindow.name = "Conversation Window";
            guiManager.AddGUI(conversationWindow, true, true);
            guiManager.AddGUI(inventoryWindow);
            guiManager.AddGUI(tradeWindow);
            guiManager.OpenGUI(conversationWindow.name);

            scriptingEngine = new ScriptingEngine();

            target = gameManager.AddComponent<ConversationEngine>();
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");

            EntityTemplate random = templateHandler.Get("human");
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

            Sprite[] sprites = objectIconHandler.GetDefaultSprites();

            BasicValueContainer<INeed> needs = new BasicValueContainer<INeed>(needHandler.GetManyRandomised(random.Needs));

            instigator = new Entity(
                random,
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
                random,
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

            entityHandler.AddEntity(instigator);
            entityHandler.AddEntity(listener);

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
            GameObject.DestroyImmediate(gameManager);
            GameObject.DestroyImmediate(inventoryManager);
            GameObject.DestroyImmediate(canvas);
            GameObject.DestroyImmediate(listenerObject.gameObject);
            GameObject.DestroyImmediate(instigatorObject.gameObject);
        }
    }
}
