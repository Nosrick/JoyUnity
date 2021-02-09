using System;
using System.Collections;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using Moq;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class EntityRelationshipHandlerTests
    {
        private ScriptingEngine scriptingEngine;

        private IEntityRelationshipHandler target;
        
        private IEntity left;
        private IEntity right;
        
        [SetUp]
        public void SetUp()
        {
            GlobalConstants.ActionLog = new ActionLog();
            scriptingEngine = new ScriptingEngine();

            ILiveEntityHandler entityHandler = new LiveEntityHandler();
            IGameManager gameManager = Mock.Of<IGameManager>(
                manager => manager.EntityHandler == entityHandler);
            GlobalConstants.GameManager = gameManager;

            target = new EntityRelationshipHandler();
        }

        [SetUp]
        public void SetUpEntities()
        {
            left = Mock.Of<IEntity>(entity => entity.Guid == Guid.NewGuid());
            right = Mock.Of<IEntity>(entity => entity.Guid == Guid.NewGuid());
        }

        [UnityTest]
        public IEnumerator CreateRelationship_ShouldHave_ZeroValue()
        {
            //given
            IRelationship relationship = target.CreateRelationship(new[] {left, right}, new []{"friendship"});
            
            //when

            //then
            Assert.That(relationship.GetRelationshipValue(this.left.Guid, this.right.Guid), Is.EqualTo(0));

            return null;
        }

        [UnityTest]
        public IEnumerator CreateRelationshipWithValue_ShouldHave_NonZeroValue()
        {
            //given
            IRelationship relationship = target.CreateRelationshipWithValue(
                new[] {left, right},
                new [] {"friendship"},
                50);
            
            //when
            
            //then
            Assert.That(relationship.GetRelationshipValue(this.left.Guid, this.right.Guid), Is.EqualTo(50));

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