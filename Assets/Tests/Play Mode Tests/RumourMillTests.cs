using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Conversation.Subengines.Rumours;
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
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RumourMillTests
    {
        private GameObject container;

        private IRumourMill target;
        
        private ScriptingEngine scriptingEngine;
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler relationshipHandler;

        private ObjectIconHandler objectIconHandler;

        private EntityBioSexHandler bioSexHandler;

        private EntitySexualityHandler sexualityHandler;

        private EntityRomanceHandler romanceHandler;

        private ParameterProcessorHandler parameterProcessorHandler;

        private EntitySkillHandler skillHandler;

        private EntityFactory entityFactory;

        private GameObject inventoryManager;
        
        private Entity left;
        private Entity right;

        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            templateHandler = container.AddComponent<EntityTemplateHandler>();
            cultureHandler = container.AddComponent<CultureHandler>();
            needHandler = container.AddComponent<NeedHandler>();
            relationshipHandler = container.AddComponent<EntityRelationshipHandler>();
            materialHandler = container.AddComponent<MaterialHandler>();
            jobHandler = container.AddComponent<JobHandler>();
            bioSexHandler = container.AddComponent<EntityBioSexHandler>();
            sexualityHandler = container.AddComponent<EntitySexualityHandler>();
            skillHandler = container.AddComponent<EntitySkillHandler>();
            parameterProcessorHandler = container.AddComponent<ParameterProcessorHandler>();
            romanceHandler = container.AddComponent<EntityRomanceHandler>();

            entityFactory = new EntityFactory();
            
            target = new ConcreteRumourMill();
            
            WorldInstance world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0],
                "TESTING");

            EntityTemplate template = templateHandler.GetRandom();
            IGrowingValue level = Mock.Of<IGrowingValue>();
            
            ICulture culture = Mock.Of<ICulture>(
                c => c.GetNameForChain(
                         It.IsAny<int>(), 
                         It.IsAny<string>(),
                         It.IsAny<int>()) == "NAME"
                     && c.NameData == new[] { new NameData("NAME", new []{ 0, 1 }, new [] { "female", "male" }, new int[0]) });

            List<ICulture> cultures = new List<ICulture> {culture};

            IJob job = Mock.Of<IJob>();
            IBioSex sex = Mock.Of<IBioSex>(s => s.Name == "female");
            IGender gender = Mock.Of<IGender>(
                g => g.Name == "female" 
                    && g.PersonalSubject == "her");
            ISexuality sexuality = Mock.Of<ISexuality>();
            IRomance romance = Mock.Of<IRomance>();

            Sprite[] sprites = objectIconHandler.GetDefaultSprites();

            left = new Entity(
                template,
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
                template,
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
        }

        [UnityTest]
        public IEnumerator RumourMill_ShouldHave_NonZeroRumourTypeCount()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.RumourTypes, Is.Not.Empty);

            return null;
        }

        [UnityTest]
        public IEnumerator RumourMill_ShouldMake_ValidRumours()
        {
            //given
            IRumour[] rumours = target.GenerateOneRumourOfEachType(new JoyObject[] {left, right});

            //then
            foreach (IRumour rumour in rumours)
            {
                Assert.That(rumour.Words, Does.Not.Contain("<"));
                Assert.That(rumour.Words, Does.Not.Contain("PARAMETER NUMBER MISMATCH"));
                Debug.Log(rumour.Words);
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container);
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}