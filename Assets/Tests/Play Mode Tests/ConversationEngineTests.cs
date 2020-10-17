using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Castle.Core.Internal;
using DevionGames.InventorySystem;
using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Cultures;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Items;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Entities.Sexes;
using JoyLib.Code.Entities.Sexuality;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Rollers;
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ConversationEngineTests
    {
        private ScriptingEngine scriptingEngine;
        
        private ConversationEngine target;

        private GameObject gameManager;
        private GameObject conversationWindow;
        
        private EntityTemplateHandler templateHandler;

        private NeedHandler needHandler;

        private CultureHandler cultureHandler;

        private MaterialHandler materialHandler;

        private JobHandler jobHandler;

        private EntityRelationshipHandler entityRelationshipHandler;

        private ObjectIconHandler objectIconHandler;

        private Entity instigator;
        private Entity listener;
        
        [SetUp]
        public void SetUp()
        {
            gameManager = new GameObject("GameManager");
            gameManager.AddComponent<InventoryManager>();

            conversationWindow =
                GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/GUI/Conversation/Conversation Window"));
            conversationWindow.name = "Conversation Window";

            objectIconHandler = gameManager.AddComponent<ObjectIconHandler>();
            templateHandler = gameManager.AddComponent<EntityTemplateHandler>();
            cultureHandler = gameManager.AddComponent<CultureHandler>();
            needHandler = gameManager.AddComponent<NeedHandler>();
            entityRelationshipHandler = gameManager.AddComponent<EntityRelationshipHandler>();
            materialHandler = gameManager.AddComponent<MaterialHandler>();
            jobHandler = gameManager.AddComponent<JobHandler>();

            scriptingEngine = new ScriptingEngine();

            target = gameManager.AddComponent<ConversationEngine>();
            
            EntityFactory factory = new EntityFactory();
            
            Mock<IBioSex> female = new Mock<IBioSex>();
            female.Setup(sex => sex.Name).Returns("female");
            
            List<CultureType> cultures = cultureHandler.GetByCreatureType("human");
            
            Mock<ISexuality> sexuality = new Mock<ISexuality>();
            
            Mock<IGrowingValue> level = new Mock<IGrowingValue>();
            EntityTemplate humanTemplate = templateHandler.Get("human");
            
            instigator = factory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                sexuality.Object);
            
            listener = factory.CreateFromTemplate(
                humanTemplate,
                level.Object,
                Vector2Int.zero,
                cultures,
                female.Object,
                sexuality.Object);
        }

        [Test]
        public void LoadData_ShouldNotBeEmpty()
        {
            //given
            
            //when
            ReadOnlyCollection<ITopic> topics = target.AllTopics;

            //then
            Assert.That(topics, Is.Not.Empty);
            foreach (ITopic topic in topics)
            {
                Assert.That(topic.Words.Length, Is.GreaterThan(0));
                Assert.That(topic.ID.Length, Is.GreaterThan(0));
            }
            
            GameObject.DestroyImmediate(gameManager);
        }

        [Test]
        public void Converse_ShouldCompleteConversation()
        {
            int depth = 0;
            
            target.SetActors(instigator, listener);
            
            List<ITopic> topics = target.Converse("Greeting");
            while (topics.IsNullOrEmpty() == false)
            {
                int result = RNG.instance.Roll(0, topics.Count);
                topics = target.Converse(topics[result].ID);

                depth += 1;
            }

            Assert.That(depth, Is.Not.Zero);
            
            GameObject.DestroyImmediate(gameManager);
        }
    }
}
