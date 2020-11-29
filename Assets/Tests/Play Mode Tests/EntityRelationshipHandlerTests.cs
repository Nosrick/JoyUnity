using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
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
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class EntityRelationshipHandlerTests
    {
        private ScriptingEngine scriptingEngine;

        private GameObject inventoryManager;

        private IEntityRelationshipHandler target;

        private IEntitySkillHandler SkillHandler;
        private IEntityTemplateHandler TemplateHandler;
        private INeedHandler NeedHandler;
        private ILiveEntityHandler EntityHandler;

        private WorldInstance world;
        
        private Entity left;
        private Entity right;
        
        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            scriptingEngine = new ScriptingEngine();

            target = new EntityRelationshipHandler();
            NeedHandler = new NeedHandler();
            SkillHandler = new EntitySkillHandler(NeedHandler);
            TemplateHandler = new EntityTemplateHandler(SkillHandler);
            EntityHandler = new LiveEntityHandler();
        }

        [SetUp]
        public void SetUpEntities()
        {
            EntityTemplate random = TemplateHandler.GetRandom();
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
            
            world = new WorldInstance(
                new WorldTile[0,0], 
                new string[0], 
                "TESTING",
                EntityHandler,
                new RNG());
            
            left = new Entity(
                random,
                new BasicValueContainer<INeed>(NeedHandler.GetManyRandomised(NeedHandler.NeedNames)),
                cultures,
                level,
                job,
                gender,
                sex,
                sexuality,
                romance,
                Vector2Int.down,
                new Sprite[0], 
                world,
                new StandardDriver());

            right = new Entity(
                random,
                new BasicValueContainer<INeed>(NeedHandler.GetManyRandomised(NeedHandler.NeedNames)),
                cultures,
                level,
                job,
                gender,
                sex,
                sexuality,
                romance,
                Vector2Int.down,
                new Sprite[0], 
                world,
                new StandardDriver());

            left.PlayerControlled = true;
            
            world.AddEntity(left);
            world.AddEntity(right);
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
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}