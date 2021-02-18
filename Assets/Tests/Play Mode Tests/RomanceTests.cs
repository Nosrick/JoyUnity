﻿using System;
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
            this.scriptingEngine = new ScriptingEngine();

            this.RelationshipHandler = new EntityRelationshipHandler();

            ICultureHandler cultureHandler = Mock.Of<ICultureHandler>(
                handler => handler.Cultures == new ICulture[]
                {
                    Mock.Of<ICulture>(culture => culture.RomanceTypes ==
                                                 new string[]
                                                 {
                                                     "heteroromantic",
                                                     "homoromantic",
                                                     "panromantic",
                                                     "aromantic"
                                                 })
                });

            GlobalConstants.GameManager = Mock.Of<IGameManager>(
                manager => manager.RelationshipHandler == this.RelationshipHandler
                && manager.CultureHandler == cultureHandler);

            this.target = new EntityRomanceHandler();

            this.heteroromantic = this.target.Get("heteroromantic");
            this.homoromantic = this.target.Get("homoromantic");
            this.biromantic = this.target.Get("panromantic");
            this.aromantic = this.target.Get("aromantic");
        }

        [SetUp]
        public void SetUpHumans()
        {
            IGender femaleGender = Mock.Of<IGender>(gender => gender.Name == "female");
            IGender maleGender = Mock.Of<IGender>(gender => gender.Name == "male");

            this.heterofemaleHuman = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                && human.Romance == this.heteroromantic
                && human.Guid == Guid.NewGuid());

            this.heteroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == this.heteroromantic
                         && human.Guid == Guid.NewGuid());

            this.homoMaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == this.homoromantic
                         && human.Guid == Guid.NewGuid());

            this.homoMaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == this.homoromantic
                         && human.Guid == Guid.NewGuid());

            this.homofemaleHumanLeft = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == this.homoromantic
                         && human.Guid == Guid.NewGuid());

            this.homofemaleHumanRight = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == this.homoromantic
                         && human.Guid == Guid.NewGuid());

            this.biMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == this.biromantic
                         && human.Guid == Guid.NewGuid());

            this.bifemaleHuman = Mock.Of<IEntity>(
                human => human.Gender == femaleGender
                         && human.Romance == this.biromantic
                         && human.Guid == Guid.NewGuid());

            this.aroMaleHuman = Mock.Of<IEntity>(
                human => human.Gender == maleGender
                         && human.Romance == this.aromantic
                         && human.Guid == Guid.NewGuid());


            IEntity[] heteroCouple = new IEntity[] {this.heterofemaleHuman, this.heteroMaleHuman };
            IEntity[] homofemaleCouple = new IEntity[] {this.homofemaleHumanLeft, this.homofemaleHumanRight };
            IEntity[] homoMaleCouple = new IEntity[] {this.homoMaleHumanLeft, this.homoMaleHumanRight };
            IEntity[] biCoupleLeft = new IEntity[] {this.bifemaleHuman, this.homofemaleHumanLeft };
            IEntity[] biCoupleRight = new IEntity[] {this.bifemaleHuman, this.biMaleHuman };
            IEntity[] asexualCouple = new IEntity[] {this.aroMaleHuman, this.bifemaleHuman};

            this.RelationshipHandler.CreateRelationshipWithValue(heteroCouple, new[]{ "monoamorous" }, 500);
            this.RelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, new[]{ "monoamorous" }, 500);
            this.RelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, new[]{ "monoamorous" }, 500);
            this.RelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, new[]{ "monoamorous" }, 500);
            this.RelationshipHandler.CreateRelationshipWithValue(biCoupleRight, new[]{ "monoamorous" }, 500);
            this.RelationshipHandler.CreateRelationshipWithValue(asexualCouple, new[]{ "monoamorous" }, 500);
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_AcceptsHeteroPartners()
        {
            IJoyObject[] participants = new [] {this.heterofemaleHuman, this.heteroMaleHuman };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsTrue(this.heteroromantic.WillRomance(this.heterofemaleHuman, this.heteroMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Heteroromantic_Compatible_RejectsHomoPartners()
        {
            IJoyObject[] participants = new [] {this.heterofemaleHuman, this.homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsFalse(this.heteroromantic.WillRomance(this.heterofemaleHuman, this.homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_AcceptsHomoPartners()
        {
            IJoyObject[] participants = new [] {this.homoMaleHumanLeft, this.homoMaleHumanRight };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsTrue(this.homoromantic.WillRomance(this.homoMaleHumanLeft, this.homoMaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Homoromantic_Compatible_RejectsHeteroPartners()
        {
            IJoyObject[] participants = new[] {this.homofemaleHumanLeft, this.homofemaleHumanRight };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsFalse(this.homoromantic.WillRomance(this.homoMaleHumanLeft, this.homofemaleHumanRight, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHomoPartners()
        {
            IJoyObject[] participants = new[] {this.bifemaleHuman, this.homofemaleHumanLeft };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsTrue(this.biromantic.WillRomance(this.bifemaleHuman, this.homofemaleHumanLeft, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Biromantic_Compatible_WillAcceptHeteroPartners()
        {
            IJoyObject[] participants = new[] {this.bifemaleHuman, this.biMaleHuman };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsTrue(this.biromantic.WillRomance(this.bifemaleHuman, this.biMaleHuman, relationships));

            return null;
        }

        [UnityTest]
        public IEnumerator Aromantic_Compatible_WillRejectPartner()
        {
            IJoyObject[] participants = new[] {this.aroMaleHuman, this.bifemaleHuman };
            IEnumerable<IRelationship> relationships = this.RelationshipHandler.Get(participants);
            Assert.IsFalse(this.aromantic.WillRomance(this.aroMaleHuman, this.bifemaleHuman, relationships));

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