using System.Collections;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Relationships;
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
            scriptingEngine = new ScriptingEngine();

            target = new EntityRelationshipHandler();
        }

        [SetUp]
        public void SetUpEntities()
        {
            left = Mock.Of<IEntity>(entity => entity.GUID == 1);
            right = Mock.Of<IEntity>(entity => entity.GUID == 2);
        }

        [UnityTest]
        public IEnumerator CreateRelationship_ShouldHave_ZeroValue()
        {
            //given
            IRelationship relationship = target.CreateRelationship(new[] {left, right}, new []{"friendship"});
            
            //when

            //then
            Assert.That(relationship.GetRelationshipValue(left.GUID, right.GUID), Is.EqualTo(0));

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
            Assert.That(relationship.GetRelationshipValue(left.GUID, right.GUID), Is.EqualTo(50));

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
        }
    }
}