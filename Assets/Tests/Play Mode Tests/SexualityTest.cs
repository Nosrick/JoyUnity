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
using JoyLib.Code.Unity.GUI;
using NUnit.Framework;
using Moq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SexualityTest
    {
        private ScriptingEngine scriptingEngine;
        private EntityFactory entityFactory;

        private IGameManager container;

        private GameObject inventoryManager;

        private IEntitySexualityHandler target;
        
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
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            container = new GameObject("GameManager").AddComponent<GameManager>();

            GlobalConstants.GameManager = container;

            scriptingEngine = new ScriptingEngine();

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

            List<ICulture> cultures = container.CultureHandler.GetByCreatureType("human");

            heterosexual = target.Get("heterosexual");
            homosexual = target.Get("homosexual");
            bisexual = target.Get("bisexual");
            asexual = target.Get("asexual");

            IGrowingValue level = Mock.Of<IGrowingValue>();
            EntityTemplate humanTemplate = container.EntityTemplateHandler.Get("human");

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

            container.RelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monoamorous", 500);
            container.RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monoamorous", 500);
            container.RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monoamorous", 500);
            container.RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monoamorous", 500);
            container.RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monoamorous", 500);
            container.RelationshipHandler.CreateRelationshipWithValue(asexualCouple, "monoamorous", 500);
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsTrue(heterosexual.WillMateWith(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsFalse(heterosexual.WillMateWith(heterofemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsTrue(homosexual.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsFalse(homosexual.WillMateWith(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Asexual_WillMateWith_RejectsPartner()
        {
            IJoyObject[] participants = new IJoyObject[] { asexualMaleHuman, bifemaleHuman };
            IRelationship[] relationships = container.RelationshipHandler.Get(participants);
            Assert.IsFalse(asexual.WillMateWith(asexualMaleHuman, bifemaleHuman, relationships));

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