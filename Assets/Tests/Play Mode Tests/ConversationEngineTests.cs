using JoyLib.Code.Conversation;
using JoyLib.Code.Conversation.Conversations;
using JoyLib.Code.Entities.Jobs;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class ConversationEngineTests
    {
        private ConversationEngine target;

        private GameObject gameManager;
        
        [SetUp]
        public void SetUp()
        {
            gameManager = new GameObject("GameManager");

            target = gameManager.AddComponent<ConversationEngine>();
        }

        [Test]
        public void LoadData_ShouldNotBeEmpty()
        {
            //given
            
            //when
            System.Collections.Generic.List<ITopic> topics = target.LoadTopics();

            //then
            Assert.That(topics, Is.Not.Empty);
            foreach (ITopic topic in topics)
            {
                Assert.That(topic.Words.Length, Is.GreaterThan(0));
                Assert.That(topic.ID.Length, Is.GreaterThan(0));
            }
            
            GameObject.DestroyImmediate(gameManager);
        }
    }
}
