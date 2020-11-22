using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
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
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EntityRelationshipHandlerTests
    {
        private ScriptingEngine scriptingEngine;
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler target;

        private ObjectIconHandler objectIconHandler;

        private EntityBioSexHandler bioSexHandler;

        private EntitySexualityHandler sexualityHandler;

        private EntityRomanceHandler romanceHandler;

        private EntitySkillHandler skillHandler;

        private EntityFactory entityFactory;

        private GameObject container;

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
            target = container.AddComponent<EntityRelationshipHandler>();
            materialHandler = container.AddComponent<MaterialHandler>();
            jobHandler = container.AddComponent<JobHandler>();
            bioSexHandler = container.AddComponent<EntityBioSexHandler>();
            sexualityHandler = container.AddComponent<EntitySexualityHandler>();
            romanceHandler = container.AddComponent<EntityRomanceHandler>();
            skillHandler = container.AddComponent<EntitySkillHandler>();

            entityFactory = new EntityFactory();
        }

        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = templateHandler.GetRandom();
            IGrowingValue level = new ConcreteGrowingValue(
                "level",
                1,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                0f,
                GlobalConstants.DEFAULT_SUCCESS_THRESHOLD,
                new StandardRoller(),
                new NonUniqueDictionary<INeed, float>());

            ICulture culture = Mock.Of<ICulture>( c => c.GetNameForChain(It.IsAny<int>(), It.IsAny<string>()) == "NAME"
            && c.NameData == new NameData[] { new NameData("NAME", new []{0, 1}, new []{"all"}) });
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
            
            left = entityFactory.CreateFromTemplate(
                random,
                level,  
                Vector2Int.down,
                cultures,
                gender,
                sex,
                sexuality,
                romance,
                job);

            right = entityFactory.CreateFromTemplate(
                random,
                level,
                Vector2Int.up,
                cultures,
                gender,
                sex,
                sexuality,
                romance,
                job);
        }

        [UnityTest]
        public IEnumerator CreateRelationship_ShouldHave_ZeroValue()
        {
            //given
            IRelationship relationship = target.CreateRelationship(new[] {left, right});
            
            //when

            //then
            Assert.That(relationship.GetRelationshipValue(left.GUID, right.GUID), Is.EqualTo(0));

            return null;
        }

        [UnityTest]
        public IEnumerator CreateRelationshipWithValue_ShouldHave_NonZeroValue()
        {
            //given
            IRelationship relationship = target.CreateRelationshipWithValue(
                new[] {left, right},
                "friendship",
                50);
            
            //when
            
            //then
            Assert.That(relationship.GetRelationshipValue(left.GUID, right.GUID), Is.EqualTo(50));

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