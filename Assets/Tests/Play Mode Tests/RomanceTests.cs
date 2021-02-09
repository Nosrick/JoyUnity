using System;
using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Romance;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
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
            ActionLog actionLog = new ActionLog();
            GlobalConstants.ActionLog = actionLog;
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
                && human.Romance == heteroromantic
                && human.Guid == Guid.NewGuid());

            heteroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == heteroromantic
                         && human.Guid == Guid.NewGuid());

            homoMaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == homoromantic
                         && human.Guid == Guid.NewGuid());

            homoMaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == homoromantic
                         && human.Guid == Guid.NewGuid());

            homofemaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == homoromantic
                         && human.Guid == Guid.NewGuid());

            homofemaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == homoromantic
                         && human.Guid == Guid.NewGuid());

            biMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == biromantic
                         && human.Guid == Guid.NewGuid());

            bifemaleHuman = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == biromantic
                         && human.Guid == Guid.NewGuid());

            aroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == aromantic
                         && human.Guid == Guid.NewGuid());


            IEntity[] heteroCouple = new IEntity[] { heterofemaleHuman, heteroMaleHuman };
            IEntity[] homofemaleCouple = new IEntity[] { homofemaleHumanLeft, homofemaleHumanRight };
            IEntity[] homoMaleCouple = new IEntity[] { homoMaleHumanLeft, homoMaleHumanRight };
            IEntity[] biCoupleLeft = new IEntity[] { bifemaleHuman, homofemaleHumanLeft };
            IEntity[] biCoupleRight = new IEntity[] { bifemaleHuman, biMaleHuman };
            IEntity[] asexualCouple = new IEntity[] {aroMaleHuman, bifemaleHuman};

            RelationshipHandler.CreateRelationshipWithValue(heteroCouple, new[]{ "monoamorous" }, 500);
            RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, new[]{ "monoamorous" }, 500);
            RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, new[]{ "monoamorous" }, 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, new[]{ "monoamorous" }, 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, new[]{ "monoamorous" }, 500);
            RelationshipHandler.CreateRelationshipWithValue(asexualCouple, new[]{ "monoamorous" }, 500);
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(heteroromantic.WillRomance(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(heteroromantic.WillRomance(heterofemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(homoromantic.WillRomance(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(homoromantic.WillRomance(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.WillRomance(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(biromantic.WillRomance(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Aromantic_Compatible_WillRejectPartner()
        {
            IJoyObject[] participants = new[] { aroMaleHuman, bifemaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(aromantic.WillRomance(aroMaleHuman, bifemaleHuman, relationships));

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
            GlobalConstants.ActionLog.Dispose();
        }
    }
}