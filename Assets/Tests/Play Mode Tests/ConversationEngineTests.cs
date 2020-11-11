using System.Collections;
using System.Collections.Generic;
using Castle.Core.Internal;
using DevionGames.InventorySystem;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours.Parameters;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
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

        private WorldInstance world;
        
        [SetUp]
        public void SetUp()
        {
            prefab = Resources.Load<GameObject>("Prefabs/MonoBehaviourHandler");
            gameManager = new GameObject("GameManager");
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            conversationWindow =
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"));
            conversationWindow.name = "Conversation Window";

            inventoryWindow = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Inventory/GUIInventory"));
            inventoryWindow.name = "Inventory";

            objectIconHandler = gameManager.AddComponent<ObjectIconHandler>();
            templateHandler = gameManager.AddComponent<EntityTemplateHandler>();
            cultureHandler = gameManager.AddComponent<CultureHandler>();
            needHandler = gameManager.AddComponent<NeedHandler>();
            skillHandler = gameManager.AddComponent<EntitySkillHandler>();
            entityRelationshipHandler = gameManager.AddComponent<EntityRelationshipHandler>();
            materialHandler = gameManager.AddComponent<MaterialHandler>();
            jobHandler = gameManager.AddComponent<JobHandler>();
            guiManager = gameManager.AddComponent<GUIManager>();
            itemHandler = gameManager.AddComponent<LiveItemHandler>();

            parameterProcessorHandler = gameManager.AddComponent<ParameterProcessorHandler>();

            questProvider = gameManager.AddComponent<QuestProvider>();
            questTracker = gameManager.AddComponent<QuestTracker>();

            entityHandler = gameManager.AddComponent<LiveEntityHandler>();

            GameObject conversationGUI =
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"));
            conversationGUI.name = "Conversation Window";
            guiManager.AddGUI(conversationGUI, true, true);
            guiManager.OpenGUI(conversationGUI.name);
            guiManager.AddGUI(inventoryWindow);

            scriptingEngine = new ScriptingEngine();

            target = gameManager.AddComponent<ConversationEngine>();
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");
            
            EntityFactory factory = new EntityFactory();

            IBioSex female = Mock.Of<IBioSex>(sex => sex.Name == "female");

            NameData[] namedata = new[]
            {
                new NameData("NAME",
                    new[] {0, 1},
                    new[] {"male", "female"})
            };

            List<ICulture> cultures = new List<ICulture>()
            {
                Mock.Of<ICulture>(
                    c => c.GetNameForChain(It.IsAny<int>(), It.IsAny<string>()) == "NAME"
                    && c.NameData == namedata)
            };

            IRomance romance = Mock.Of<IRomance>();

            ISexuality sexuality = Mock.Of<ISexuality>(s => s.Tags == new List<string>());

            IGrowingValue level = Mock.Of<IGrowingValue>();
            EntityTemplate humanTemplate = templateHandler.Get("human");
            
            instigator = factory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                female,
                sexuality,
                romance);
            
            listener = factory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                female,
                sexuality,
                romance);
            
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
            
            ITopic[] topics = target.Converse();
            while (topics.IsNullOrEmpty() == false)
            {
                int result = RNG.instance.Roll(0, topics.Length);
                topics = target.Converse(result);

                depth += 1;
            }

            Assert.That(depth, Is.Not.Zero);

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(gameManager);
            GameObject.DestroyImmediate(inventoryManager);
            GameObject.DestroyImmediate(listenerObject.gameObject);
            GameObject.DestroyImmediate(instigatorObject.gameObject);
        }
    }
}
