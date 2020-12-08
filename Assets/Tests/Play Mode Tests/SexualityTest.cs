﻿using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Gender;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class SexualityTest
    {
        private ScriptingEngine scriptingEngine;

        private IEntityRelationshipHandler RelationshipHandler;

        private IEntitySexualityHandler target;
        
        private IEntity heteroMaleHuman;
        private IEntity heterofemaleHuman;

        private IEntity homoMaleHumanLeft;
        private IEntity homoMaleHumanRight;

        private IEntity homofemaleHumanLeft;
        private IEntity homofemaleHumanRight;

        private IEntity biMaleHuman;
        private IEntity bifemaleHuman;

        private IEntity asexualMaleHuman;

        private ISexuality heterosexual;
        private ISexuality homosexual;
        private ISexuality bisexual;
        private ISexuality asexual;

        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();
            
            target = new EntitySexualityHandler();
            RelationshipHandler = new EntityRelationshipHandler();
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

            heterosexual = target.Get("heterosexual");
            homosexual = target.Get("homosexual");
            bisexual = target.Get("bisexual");
            asexual = target.Get("asexual");

            heterofemaleHuman = Mock.Of<IEntity>(
                entity => entity.Gender == femaleGender
                && entity.Sexuality == heterosexual
                && entity.GUID == 1);

            heteroMaleHuman = Mock.Of<IEntity>(
                entity => entity.Gender == maleGender
                          && entity.Sexuality == heterosexual
                          && entity.GUID == 2);

            homoMaleHumanLeft = Mock.Of<IEntity>(
                entity => entity.Gender == maleGender
                          && entity.Sexuality == homosexual
                          && entity.GUID == 3);

            homoMaleHumanRight = Mock.Of<IEntity>(
                entity => entity.Gender == maleGender
                          && entity.Sexuality == homosexual
                          && entity.GUID == 4);

            homofemaleHumanLeft = Mock.Of<IEntity>(
                entity => entity.Gender == femaleGender
                          && entity.Sexuality == homosexual
                          && entity.GUID == 5);

            homofemaleHumanRight = Mock.Of<IEntity>(
                entity => entity.Gender == femaleGender
                          && entity.Sexuality == homosexual
                          && entity.GUID == 6);

            biMaleHuman = Mock.Of<IEntity>(
                entity => entity.Gender == maleGender
                          && entity.Sexuality == bisexual
                          && entity.GUID == 7);

            bifemaleHuman = Mock.Of<IEntity>(
                entity => entity.Gender == femaleGender
                          && entity.Sexuality == bisexual
                          && entity.GUID == 8);

            asexualMaleHuman = Mock.Of<IEntity>(
                entity => entity.Gender == maleGender
                          && entity.Sexuality == asexual
                          && entity.GUID == 9);


            IEntity[] heteroCouple = new IEntity[] { heterofemaleHuman, heteroMaleHuman };
            IEntity[] homofemaleCouple = new IEntity[] { homofemaleHumanLeft, homofemaleHumanRight };
            IEntity[] homoMaleCouple = new IEntity[] { homoMaleHumanLeft, homoMaleHumanRight };
            IEntity[] biCoupleLeft = new IEntity[] { bifemaleHuman, homofemaleHumanLeft };
            IEntity[] biCoupleRight = new IEntity[] { bifemaleHuman, biMaleHuman };
            IEntity[] asexualCouple = new IEntity[] {asexualMaleHuman, bifemaleHuman};

            RelationshipHandler.CreateRelationshipWithValue(heteroCouple, new []{"sexual"}, 500);
            RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, new []{"sexual"}, 500);
            RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, new []{"sexual"}, 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, new []{"sexual"}, 500);
            RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, new []{"sexual"}, 500);
            RelationshipHandler.CreateRelationshipWithValue(asexualCouple, new []{"sexual"}, 500);
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, heteroMaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(heterosexual.WillMateWith(heterofemaleHuman, heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] { heterofemaleHuman, homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(heterosexual.WillMateWith(heterofemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] { homoMaleHumanLeft, homoMaleHumanRight };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(homosexual.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] { homofemaleHumanLeft, homofemaleHumanRight };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(homosexual.WillMateWith(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] { bifemaleHuman, biMaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Asexual_WillMateWith_RejectsPartner()
        {
            IJoyObject[] participants = new IJoyObject[] { asexualMaleHuman, bifemaleHuman };
            IEnumerable<IRelationship> relationships = RelationshipHandler.Get(participants);
            Assert.IsFalse(asexual.WillMateWith(asexualMaleHuman, bifemaleHuman, relationships));

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
        }
    }
}