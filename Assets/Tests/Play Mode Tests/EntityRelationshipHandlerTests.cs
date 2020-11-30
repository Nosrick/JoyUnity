using System.Collections;
using System.Collections.Generic;
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
using JoyLib.Code.Unity.GUI;
using JoyLib.Code.World;
using Moq;
using NUnit.Framework;
using UnityEngine;
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
            IRelationship relationship = target.CreateRelationship(new[] {left, right});
            
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
                "friendship",
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