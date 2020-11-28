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
using JoyLib.Code.Unity.GUI;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EntityRelationshipHandlerTests
    {
        private ScriptingEngine scriptingEngine;

        private EntityFactory entityFactory;

        private IGameManager container;

        private GameObject inventoryManager;

        private IEntityRelationshipHandler target;
        
        private Entity left;
        private Entity right;
        
        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            
            container = new GameObject("GameManager").AddComponent<GameManager>();

            target = container.RelationshipHandler;

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

            entityFactory = new EntityFactory();
        }

        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = container.EntityTemplateHandler.GetRandom();
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
            GameObject.DestroyImmediate(container.MyGameObject);
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}