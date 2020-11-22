using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
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
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class RomanceTests
    {
        private ScriptingEngine scriptingEngine;
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler entityRelationshipHandler;

        private ObjectIconHandler objectIconHandler;

        private EntityFactory entityFactory;

        private EntitySkillHandler skillHandler;

        private GameObject container;

        private GameObject inventoryManager;

        private EntityRomanceHandler target;



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
            container = new GameObject("GameManager");
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            templateHandler = container.AddComponent<EntityTemplateHandler>();
            cultureHandler = container.AddComponent<CultureHandler>();
            needHandler = container.AddComponent<NeedHandler>();
            entityRelationshipHandler = container.AddComponent<EntityRelationshipHandler>();
            materialHandler = container.AddComponent<MaterialHandler>();
            jobHandler = container.AddComponent<JobHandler>();
            skillHandler = container.AddComponent<EntitySkillHandler>();

            target = container.AddComponent<EntityRomanceHandler>();

            entityFactory = new EntityFactory();
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
            EntityTemplate humanTemplate = templateHandler.Get("human");

            heterofemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                asexual,
                heteroromantic,
                job);

            heteroMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                asexual,
                heteroromantic,
                job);

            homoMaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                asexual,
                homoromantic,
                job);

            homoMaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                asexual,
                homoromantic,
                job);

            homofemaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                asexual,
                homoromantic,
                job);

            homofemaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                asexual,
                homoromantic,
                job);

            biMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                asexual,
                biromantic,
                job);

            bifemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                asexual,
                biromantic,
                job);

            aroMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                asexual,
                aromantic,
                job);


            Entity[] heteroCouple = new Entity[] { heterofemaleHuman, heteroMaleHuman };
            Entity[] homofemaleCouple = new Entity[] { homofemaleHumanLeft, homofemaleHumanRight };
            Entity[] homoMaleCouple = new Entity[] { homoMaleHumanLeft, homoMaleHumanRight };
            Entity[] biCoupleLeft = new Entity[] { bifemaleHuman, homofemaleHumanLeft };
            Entity[] biCoupleRight = new Entity[] { bifemaleHuman, biMaleHuman };
            Entity[] asexualCouple = new[] {aroMaleHuman, bifemaleHuman};

            entityRelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(asexualCouple, "monoamorous", 500);
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(heteroromantic.Compatible(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(heteroromantic.Compatible(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(homoromantic.Compatible(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(homoromantic.Compatible(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.Compatible(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.Compatible(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Aromantic_Compatible_WillRejectPartner()
        {
            IJoyObject[] participants = new[] { aroMaleHuman, bifemaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(aromantic.Compatible(aroMaleHuman, bifemaleHuman, relationships));

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