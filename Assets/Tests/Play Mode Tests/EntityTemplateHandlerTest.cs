using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;
using System.Collections.Generic;
using DevionGames.InventorySystem;

namespace Tests
{
    public class EntityTemplateHandlerTest
    {
        private EntityTemplateHandler target;

        private NeedHandler needHandler;

        private GameObject container;

        private GameObject inventoryManager;

        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            
            needHandler = container.AddComponent<NeedHandler>();
            target = container.AddComponent<EntityTemplateHandler>();
        }

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
        {
            //given
            List<EntityTemplate> entityTemplates = target.Templates;

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
            GameObject.DestroyImmediate(container);
            GameObject.DestroyImmediate(inventoryManager);
        }
    }
}
