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

        private IEntityRomanceHandler target;
        private IEntityRelationshipHandler RelationshipHandler;
        
        private IEntity heteroMaleHuman;
        private IEntity heterofemaleHuman;

        private IEntity homoMaleHumanLeft;
        private IEntity homoMaleHumanRight;

        private IEntity homofemaleHumanLeft;
        private IEntity homofemaleHumanRight;

        private IEntity biMaleHuman;
        private IEntity bifemaleHuman;

        private IEntity aroMaleHuman;

        private ISexuality asexual;

        private IRomance heteroromantic;
        private IRomance homoromantic;
        private IRomance biromantic;
        private IRomance aromantic;

        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();

            RelationshipHandler = new EntityRelationshipHandler();

            ICultureHandler cultureHandler = Mock.Of<ICultureHandler>(
                handler => handler.Cultures == new ICulture[]
                {
                    Mock.Of<ICulture>(culture => culture.RomanceTypes ==
                                                 new string[]
                                                 {
                                                     "heteroromantic",
                                                     "homoromantic",
                                                     "biromantic",
                                                     "aromantic"
                                                 })
                });
            target = new EntityRomanceHandler();

            heteroromantic = target.Get("heteroromantic");
            homoromantic = target.Get("homoromantic");
            biromantic = target.Get("biromantic");
            aromantic = target.Get("aromantic");
        }

        [SetUp]
        public void SetUpHumans()
        {
            IGender femaleGender = Mock.Of<IGender>(gender => gender.Name == "female");
            IGender maleGender = Mock.Of<IGender>(gender => gender.Name == "male");

            heterofemaleHuman = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                && human.Romance == heteroromantic);

            heteroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == heteroromantic);

            homoMaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == homoromantic);

            homoMaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == homoromantic);

            homofemaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == homoromantic);

            homofemaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == homoromantic);

            biMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == biromantic);

            bifemaleHuman = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == biromantic);

            aroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == aromantic);


            IEntity[] heteroCouple = new IEntity[] { heterofemaleHuman, heteroMaleHuman };
            IEntity[] homofemaleCouple = new IEntity[] { homofemaleHumanLeft, homofemaleHumanRight };
            IEntity[] homoMaleCouple = new IEntity[] { homoMaleHumanLeft, homoMaleHumanRight };
            IEntity[] biCoupleLeft = new IEntity[] { bifemaleHuman, homofemaleHumanLeft };
            IEntity[] biCoupleRight = new IEntity[] { bifemaleHuman, biMaleHuman };
            IEntity[] asexualCouple = new IEntity[] {aroMaleHuman, bifemaleHuman};

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
            Assert.IsFalse(heteroromantic.Compatible(heterofemaleHuman, homofemaleHumanLeft, relationships));

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
        }
    }
}