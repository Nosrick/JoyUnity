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
using JoyLib.Code.Entities.AI.Drivers;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Rollers;
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using NUnit.Framework;
using Moq;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SexualityTest
    {
        private ScriptingEngine scriptingEngine;

        private ILiveEntityHandler EntityHandler;
        private IEntityRelationshipHandler RelationshipHandler;
        private IEntityTemplateHandler TemplateHandler;

        private INeedHandler NeedHandler;
        private IEntitySkillHandler SkillHandler;
        private ICultureHandler CultureHandler;

        private WorldInstance world;

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

            scriptingEngine = new ScriptingEngine();
            
            NeedHandler = new NeedHandler();
            SkillHandler = new EntitySkillHandler(NeedHandler);
            EntityHandler = new LiveEntityHandler();
            RelationshipHandler = new EntityRelationshipHandler();
            TemplateHandler = new EntityTemplateHandler(SkillHandler);

            CultureHandler = new CultureHandler();
            
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

            IJob job = Mock.Of<IJob>();

            IRomance aromantic = Mock.Of<IRomance>();

            List<ICulture> cultures = CultureHandler.GetByCreatureType("human");

            heterosexual = target.Get("heterosexual");
            homosexual = target.Get("homosexual");
            bisexual = target.Get("bisexual");
            asexual = target.Get("asexual");

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
                heterosexual,
                aromantic,
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
                heterosexual,
                aromantic,
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
                homosexual,
                aromantic,
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
                homosexual,
                aromantic,
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
                homosexual,
                aromantic,
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
                homosexual,
                aromantic,
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
                bisexual,
                aromantic,
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
                bisexual,
                aromantic,
                Vector2Int.zero,
                new Sprite[0], 
                world,
                new StandardDriver());

            asexualMaleHuman = new Entity(
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
            Entity[] asexualCouple = new[] {asexualMaleHuman, bifemaleHuman};

            RelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monoamorous", 500);
            RelationshipHandler.CreateRelationshipWithValue(asexualCouple, "monoamorous", 500);
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(heterosexual.WillMateWith(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(heterosexual.WillMateWith(heterofemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(homosexual.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(homosexual.WillMateWith(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Asexual_WillMateWith_RejectsPartner()
        {
            IJoyObject[] participants = new IJoyObject[] { asexualMaleHuman, bifemaleHuman };
            IRelationship[] relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(asexual.WillMateWith(asexualMaleHuman, bifemaleHuman, relationships));

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}