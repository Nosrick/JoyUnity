using System;
using System.Collections.Generic;
using System.Linq;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Scripting;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Graphics;
using NUnit.Framework;
using Moq;
using UnityEngine;

namespace JoyTest
{
    public class SexualityTest
    {
        private ScriptingEngine scriptingEngine;
        private EntityTemplateHandler templateHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private ObjectIconHandler objectIconHandler;

        private EntityRelationshipHandler entityRelationshipHandler;



        private Entity heteroMaleHuman;
        private Entity heteroFemaleHuman;

        private Entity homoMaleHumanLeft;
        private Entity homoMaleHumanRight;

        private Entity homoFemaleHumanLeft;
        private Entity homoFemaleHumanRight;

        private Entity biMaleHuman;
        private Entity biFemaleHuman;

        [SetUp]
        public void SetUp()
        {
            scriptingEngine = new ScriptingEngine();
            templateHandler = new EntityTemplateHandler();
            jobHandler = new JobHandler();
            materialHandler = new MaterialHandler();
            objectIconHandler = new ObjectIconHandler(16);
            entityRelationshipHandler = new EntityRelationshipHandler();
        }

        [SetUp]
        public void SetUpHumans()
        {
            Mock<IBioSex> female = new Mock<IBioSex>();
            Mock<IBioSex> male = new Mock<IBioSex>();

            male.Setup(sex => sex.Name).Returns("male");
            male.Setup(sex => sex.CanBirth).Returns(false);

            female.Setup(sex => sex.Name).Returns("female");
            female.Setup(sex => sex.CanBirth).Returns(true);

            System.Type[] types = scriptingEngine.FetchTypeAndChildren(typeof(ISexuality));
            Dictionary<string, System.Type> keyedTypes = new Dictionary<string, System.Type>(types.Length);
            keyedTypes = types.ToDictionary(k => k.Name, e => e);

            ISexuality heterosexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.ToLower().Equals("heterosexual")).Value);
            ISexuality homosexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.ToLower().Equals("homosexual")).Value);
            ISexuality bisexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.ToLower().Equals("bisexual")).Value);

            Mock<BasicValueContainer<INeed>> emptyContainer = new Mock<BasicValueContainer<INeed>>();

            Mock<IGrowingValue> level = new Mock<IGrowingValue>();
            EntityTemplate humanTemplate = EntityTemplateHandler.instance.Get("human");

            JobType job = JobHandler.instance.GetRandom();

            heteroFemaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                female.Object,
                heterosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            heteroMaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                male.Object,
                heterosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoMaleHumanLeft = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                male.Object,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoMaleHumanRight = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                male.Object,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoFemaleHumanLeft = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                female.Object,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            homoFemaleHumanRight = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                female.Object,
                homosexual,
                Vector2Int.zero,
                null,
                null,
                null);

            biMaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                male.Object,
                bisexual,
                Vector2Int.zero,
                null,
                null,
                null);
            
            biFemaleHuman = LiveEntityHandler.instance.Create(
                humanTemplate,
                emptyContainer.Object,
                level.Object,
                job,
                female.Object,
                bisexual,
                Vector2Int.zero,
                null,
                null,
                null);


            JoyObject[] heteroCouple = new JoyObject[] { heteroFemaleHuman, heteroMaleHuman };
            JoyObject[] homoFemaleCouple = new JoyObject[] { homoFemaleHumanLeft, homoFemaleHumanRight };
            JoyObject[] homoMaleCouple = new JoyObject[] { homoMaleHumanLeft, homoMaleHumanRight };
            EntityRelationshipHandler.instance.CreateRelationship(new Entity[] { heteroFemaleHuman, heteroMaleHuman }, "monogamousrelationship");
            EntityRelationshipHandler.instance.CreateRelationship(new Entity[] { homoFemaleHumanLeft, homoFemaleHumanRight}, "monogamousrelationship");
            EntityRelationshipHandler.instance.CreateRelationship(new Entity[] { homoMaleHumanLeft, homoMaleHumanRight }, "monogamousrelationship");
            IJoyAction relationshipAction = ScriptingEngine.instance.FetchAction("modifyrelationshippointsaction");
            heteroFemaleHuman.PerformAction(relationshipAction, 
                                            heteroCouple,
                                            new string[] { "sexual" },
                                            new object[] { 500 });

            homoFemaleHumanLeft.PerformAction(relationshipAction,
                                                homoFemaleCouple,
                                                new string[] { "sexual" },
                                                new object[] { 500 });

            homoMaleHumanLeft.PerformAction(relationshipAction,
                                                homoMaleCouple,
                                                new string[] { "sexual" },
                                                new object[] { 500 });
        }

        [Test]
        public void Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            Assert.IsTrue(heteroFemaleHuman.Sexuality.WillMateWith(heteroFemaleHuman, heteroMaleHuman));
        }

        [Test]
        public void Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            Assert.IsFalse(heteroFemaleHuman.Sexuality.WillMateWith(heteroFemaleHuman, homoFemaleHumanLeft));
        }

        [Test]
        public void Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            Assert.IsTrue(homoMaleHumanLeft.Sexuality.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight));
        }

        [Test]
        public void Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            Assert.IsFalse(homoMaleHumanLeft.Sexuality.WillMateWith(homoMaleHumanLeft, homoFemaleHumanRight));
        }
    }
}