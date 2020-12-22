using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Graphics;
using Moq;

namespace Tests
{
    public class EntityTemplateHandlerTest
    {
        private IEntityTemplateHandler target;
        
        [SetUp]
        public void SetUp()
        {
            
            IEntitySkillHandler skillHandler = Mock.Of<IEntitySkillHandler>(
                handler => handler.GetCoefficients(It.IsAny<List<string>>(), It.IsAny<string>())
                == new NonUniqueDictionary<INeed, float>());
            
            target = new EntityTemplateHandler(skillHandler);
        }

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
        {
            //given
            List<IEntityTemplate> entityTemplates = target.Templates;

            //when

            //then
            Assert.That(entityTemplates, Is.Not.Empty);
            foreach(EntityTemplate template in entityTemplates)
            {
                Assert.That(template.Statistics.Values, Is.Not.Empty);
                Assert.That(template.Slots, Is.Not.Empty);
                Assert.That(template.Tags, Is.Not.Empty);
                Assert.False(template.JoyType == "DEFAULT");
                Assert.False(template.CreatureType == "DEFAULT");
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
        }
    }
}
