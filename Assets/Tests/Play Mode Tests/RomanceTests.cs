using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RomanceTests
    {
        private ScriptingEngine scriptingEngine;

        private GameObject inventoryManager;

        private IEntityRomanceHandler target;

        private ICultureHandler cultureHandler;

        private ILiveEntityHandler EntityHandler;
        private IEntityRelationshipHandler RelationshipHandler;
        private IEntityTemplateHandler TemplateHandler;

        private INeedHandler NeedHandler;
        private IEntitySkillHandler SkillHandler;

        private WorldInstance world;
        
        private Entity heteroMaleHuman;
        private Entity heterofemaleHuman;

        private Entity homoMaleHumanLeft;
        private Entity homoMaleHumanRight;

        private Entity homofemaleHumanLeft;
        private Entity homofemaleHumanRight;

        private Entity biMaleHuman;
        private Entity bifemaleHuman;

        private Entity aroMaleHuman;

        private ISexuality asexual;

        private IRomance heteroromantic;
        private IRomance homoromantic;
        private IRomance biromantic;
        private IRomance aromantic;

        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            scriptingEngine = new ScriptingEngine();
            
            NeedHandler = new NeedHandler();
            SkillHandler = new EntitySkillHandler(NeedHandler);
            EntityHandler = new LiveEntityHandler();
            RelationshipHandler = new EntityRelationshipHandler();
            TemplateHandler = new EntityTemplateHandler(SkillHandler);

            cultureHandler = new CultureHandler();
            target = new EntityRomanceHandler(cultureHandler);
            
            world = new WorldInstance(
                new WorldTile[0,0],
                new string[0], 
                "TEST",
                EntityHandler,
                new RNG());
        }

        [SetUp]
        public void SetUpHumans()
        {
            IBioSex femaleSex = Mock.Of<IBioSex>(
                sex => sex.Name == "female"
                       && sex.CanBirth == true);
            IBioSex maleSex = Mock.Of<IBioSex>(
                sex => sex.Name == "male"
                       && sex.CanBirth == false);

            IGender femaleGender = Mock.Of<IGender>(gender => gender.Name == "female");
            IGender maleGender = Mock.Of<IGender>(gender => gender.Name == "male");

            JobType job = Mock.Of<JobType>();

            List<ICulture> cultures = cultureHandler.GetByCreatureType("human");

            asexual = Mock.Of<ISexuality>(sexuality => sexuality.Tags == new List<string>());

            heteroromantic = target.Get("heteroromantic");
            homoromantic = target.Get("homoromantic");
            biromantic = target.Get("biromantic");
            aromantic = target.Get("aromantic");

            IGrowingValue level = Mock.Of<IGrowingValue>();
            EntityTemplate humanTemplate = TemplateHandler.Get("human");

            heterofemaleHuman = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                femaleGender,
                femaleSex,
                asexual,
                heteroromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            heteroMaleHuman = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                maleGender,
                maleSex,
                asexual,
                heteroromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            homoMaleHumanLeft = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                maleGender,
                maleSex,
                asexual,
                homoromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            homoMaleHumanRight = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                maleGender,
                maleSex,
                asexual,
                homoromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            homofemaleHumanLeft = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                femaleGender,
                femaleSex,
                asexual,
                homoromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            homofemaleHumanRight = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                femaleGender,
                femaleSex,
                asexual,
                homoromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            biMaleHuman = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                maleGender,
                maleSex,
                asexual,
                biromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            bifemaleHuman = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                femaleGender,
                femaleSex,
                asexual,
                biromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            aroMaleHuman = new Entity(
                humanTemplate,
                new BasicValueContainer<INeed>(),
                cultures,
                level,
                job,
                maleGender,
                maleSex,
                asexual,
                aromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());


            Entity[] heteroCouple = new Entity[] { heterofemaleHuman, heteroMaleHuman };
            Entity[] homofemaleCouple = new Entity[] { homofemaleHumanLeft, homofemaleHumanRight };
            Entity[] homoMaleCouple = new Entity[] { homoMaleHumanLeft, homoMaleHumanRight };
            Entity[] biCoupleLeft = new Entity[] { bifemaleHuman, homofemaleHumanLeft };
            Entity[] biCoupleRight = new Entity[] { bifemaleHuman, biMaleHuman };
            Entity[] asexualCouple = new[] {aroMaleHuman, bifemaleHuman};

            RelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(asexualCouple, "monoamorous", 500);
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(heteroromantic.Compatible(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(heteroromantic.Compatible(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(homoromantic.Compatible(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(homoromantic.Compatible(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.Compatible(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.Compatible(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Aromantic_Compatible_WillRejectPartner()
        {
            IJoyObject[] participants = new[] { aroMaleHuman, bifemaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(aromantic.Compatible(aroMaleHuman, bifemaleHuman, relationships));

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}