﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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



        private Entity heteroMaleHuman;
        private Entity heterofemaleHuman;

        private Entity homoMaleHumanLeft;
        private Entity homoMaleHumanRight;

        private Entity homofemaleHumanLeft;
        private Entity homofemaleHumanRight;

        private Entity biMaleHuman;
        private Entity bifemaleHuman;

        private ISexuality heterosexual;
        private ISexuality homosexual;
        private ISexuality bisexual;

        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");

            scriptingEngine = new ScriptingEngine();

            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            templateHandler = container.AddComponent<EntityTemplateHandler>();
            cultureHandler = container.AddComponent<CultureHandler>();
            needHandler = container.AddComponent<NeedHandler>();
            entityRelationshipHandler = container.AddComponent<EntityRelationshipHandler>();
            materialHandler = container.AddComponent<MaterialHandler>();

            entityFactory = new EntityFactory();
        }

        [SetUp]
        public void SetUpHumans()
        {
            Mock<IBioSex> female = new Mock<IBioSex>();
            Mock<IBioSex> male = new Mock<IBioSex>();

            Mock<JobType> job = new Mock<JobType>();

            male.Setup(sex => sex.Name).Returns("male");
            male.Setup(sex => sex.CanBirth).Returns(false);

            female.Setup(sex => sex.Name).Returns("female");
            female.Setup(sex => sex.CanBirth).Returns(true);

            List<CultureType> cultures = cultureHandler.GetByCreatureType("human");

            System.Type[] types = scriptingEngine.FetchTypeAndChildren(typeof(ISexuality));
            Dictionary<string, System.Type> keyedTypes = new Dictionary<string, System.Type>(types.Length);
            keyedTypes = types.ToDictionary(k => k.Name, e => e);

            heterosexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.Equals("heterosexual", StringComparison.OrdinalIgnoreCase)).Value);
            homosexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.Equals("homosexual", StringComparison.OrdinalIgnoreCase)).Value);
            bisexual = (ISexuality)Activator.CreateInstance(keyedTypes.Single(p => p.Key.Equals("bisexual", StringComparison.OrdinalIgnoreCase)).Value);

            Mock<BasicValueContainer<INeed>> emptyContainer = new Mock<BasicValueContainer<INeed>>();

            Mock<IGrowingValue> level = new Mock<IGrowingValue>();
            EntityTemplate humanTemplate = templateHandler.Get("human");

            heterofemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                heterosexual,
                job.Object);

            heteroMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                male.Object,
                heterosexual,
                job.Object);

            homoMaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                male.Object,
                homosexual,
                job.Object);

            homoMaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                male.Object,
                homosexual,
                job.Object);

            homofemaleHumanLeft = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                homosexual,
                job.Object);

            homofemaleHumanRight = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                homosexual,
                job.Object);

            biMaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                male.Object,
                bisexual,
                job.Object);

            bifemaleHuman = entityFactory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                bisexual,
                job.Object);


            Entity[] heteroCouple = new Entity[] { heterofemaleHuman, heteroMaleHuman };
            Entity[] homofemaleCouple = new Entity[] { homofemaleHumanLeft, homofemaleHumanRight };
            Entity[] homoMaleCouple = new Entity[] { homoMaleHumanLeft, homoMaleHumanRight };
            Entity[] biCoupleLeft = new Entity[] { bifemaleHuman, homofemaleHumanLeft };
            Entity[] biCoupleRight = new Entity[] { bifemaleHuman, biMaleHuman };

            entityRelationshipHandler.CreateRelationshipWithValue(heteroCouple, "monogamousrelationship", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homofemaleCouple, "monogamousrelationship", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(homoMaleCouple, "monogamousrelationship", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleLeft, "monogamousrelationship", 500);
            entityRelationshipHandler.CreateRelationshipWithValue(biCoupleRight, "monogamousrelationship", 500);
            IJoyAction relationshipAction = ScriptingEngine.instance.FetchAction("modifyrelationshippointsaction");
            relationshipAction.Execute(
                heteroCouple,
                new string[] { "sexual" },
                new object[] { 500 });

            relationshipAction.Execute(
                homofemaleCouple,
                new string[] { "sexual" },
                new object[] { 500 });

            relationshipAction.Execute(
                homoMaleCouple,
                new string[] { "sexual" },
                new object[] { 500 });

            relationshipAction.Execute(
                biCoupleLeft,
                new string[] { "sexual" },
                new object[] { 500 });

            relationshipAction.Execute(
                biCoupleRight,
                new string[] { "sexual" },
                new object[] { 500 });
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_AcceptsHeteroPartners()
        {
            long[] participants = new long[] { heterofemaleHuman.GUID, heteroMaleHuman.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(heterosexual.WillMateWith(heterofemaleHuman, heteroMaleHuman, relationships));

            GameObject.Destroy(container);

            yield return new WaitForSeconds(0.01f);
        }

        [UnityTest]
        public IEnumerator Heterosexual_WillMateWith_RejectsHomoPartners()
        {
            long[] participants = new long[] { heterofemaleHuman.GUID, homofemaleHumanLeft.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(heterosexual.WillMateWith(heterofemaleHuman, homofemaleHumanLeft, relationships));

            GameObject.DestroyImmediate(container);

            yield return new WaitForSeconds(0.01f);
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_AcceptsHomoPartners()
        {
            long[] participants = new long[] { homoMaleHumanLeft.GUID, homoMaleHumanRight.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(homosexual.WillMateWith(homoMaleHumanLeft, homoMaleHumanRight, relationships));

            GameObject.DestroyImmediate(container);

            yield return new WaitForSeconds(0.01f);
        }

        [UnityTest]
        public IEnumerator Homosexual_WillMateWith_RejectsHeteroPartners()
        {
            long[] participants = new long[] { homofemaleHumanLeft.GUID, homofemaleHumanRight.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsFalse(homosexual.WillMateWith(homoMaleHumanLeft, homofemaleHumanRight, relationships));

            GameObject.DestroyImmediate(container);

            yield return new WaitForSeconds(0.01f);
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHomoPartners()
        {
            long[] participants = new long[] { bifemaleHuman.GUID, homofemaleHumanLeft.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, homofemaleHumanLeft, relationships));

            GameObject.DestroyImmediate(container);

            yield return new WaitForSeconds(0.01f);
        }

        [UnityTest]
        public IEnumerator Bisexual_WillMateWith_WillAcceptHeteroPartners()
        {
            long[] participants = new long[] { bifemaleHuman.GUID, biMaleHuman.GUID };
            IRelationship[] relationships = entityRelationshipHandler.Get(participants);
            Assert.IsTrue(bisexual.WillMateWith(bifemaleHuman, biMaleHuman, relationships));

            GameObject.DestroyImmediate(container);

            yield return new WaitForSeconds(0.01f);
        }
    }
}