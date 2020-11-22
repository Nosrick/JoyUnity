using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevionGames.InventorySystem;
using DevionGames.InventorySystem.Configuration;
using JoyLib.Code;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Graphics;
using JoyLib.Code.Scripting;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Romance;
using NUnit.Framework;
using Moq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SexualityTest
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

        private GameObject container;

        private GameObject inventoryManager;

        private EntitySexualityHandler target;



        private Entity heteroMaleHuman;
        private Entity heterofemaleHuman;

        private Entity homoMaleHumanLeft;
        private Entity homoMaleHumanRight;

        private Entity homofemaleHumanLeft;
        private Entity homofemaleHumanRight;

        private Entity biMaleHuman;
        private Entity bifemaleHuman;

        private Entity asexualMaleHuman;

        private ISexuality heterosexual;
        private ISexuality homosexual;
        private ISexuality bisexual;
        private ISexuality asexual;

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
            target = container.AddComponent<EntitySexualityHandler>();

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

            IJob job = Mock.Of<IJob>();

            IRomance aromantic = Mock.Of<IRomance>();

            List<ICulture> cultures = cultureHandler.GetByCreatureType("human");

            heterosexual = target.Get("heterosexual");
            homosexual = target.Get("homosexual");
            bisexual = target.Get("bisexual");
            asexual = target.Get("asexual");

            IGrowingValue level = Mock.Of<IGrowingValue>();
            EntityTemplate humanTemplate = templateHandler.Get("human");

            heterofemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                heterosexual,
                aromantic,
                job);

            heteroMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                heterosexual,
                aromantic,
                job);

            homoMaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                homosexual,
                aromantic,
                job);

            homoMaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                homosexual,
                aromantic,
                job);

            homofemaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                homosexual,
                aromantic,
                job);

            homofemaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                homosexual,
                aromantic,
                job);

            biMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                maleGender,
                maleSex,
                bisexual,
                aromantic,
                job);

            bifemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level,
                Vector2Int.zero,
                cultures,
                femaleGender,
                femaleSex,
                bisexual,
                aromantic,
                job);

            asexualMaleHuman = entityFactory.CreateFromTemplate(
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
            Entity[] asexualCouple = new[] {asexualMaleHuman, bifemaleHuman};

            entityRelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monoamorous", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(asexualCouple, "monoamorous", 500);
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(heterosexual.WillMateWith(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(heterosexual.WillMateWith(heterofemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(homosexual.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(homosexual.WillMateWith(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Asexual_WillMateWith_RejectsPartner()
        {
            IJoyObject[] participants = new IJoyObject[] { asexualMaleHuman, bifemaleHuman };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(asexual.WillMateWith(asexualMaleHuman, bifemaleHuman, relationships));

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