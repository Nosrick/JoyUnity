using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Graphics;
using Moq;

namespace Tests
{
    public class EntityTemplateHandlerTest
    {
        private IEntityTemplateHandler target;
        
        private GameObject inventoryManager;
        [SetUp]
        public void SetUp()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            
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
                Assert.That(template.Statistics.Collection, Is.Not.Empty);
                Assert.That(template.Slots, Is.Not.Empty);
                Assert.That(template.Tags, Is.Not.Empty);
                Assert.False(template.JoyType == "DEFAULT");
                Assert.False(template.CreatureType == "DEFAULT");
                Assert.False(template.Tileset == "DEFAULT");
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(inventoryManager);
            GlobalConstants.GameManager = null;
        }
    }
}
