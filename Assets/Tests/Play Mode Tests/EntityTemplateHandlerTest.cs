using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections;

namespace Tests
{
    public class EntityTemplateHandlerTest
    {
        private EntityTemplateHandler target;

        private NeedHandler needHandler;

        private GameObject container;

        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            
            needHandler = container.AddComponent<NeedHandler>();
            target = container.AddComponent<EntityTemplateHandler>();
        }

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
        {
            //given
            EntityTemplate[] entityTemplates = target.Templates;

            //when
            yield return new WaitForSeconds(0.1f);

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

            Object.DestroyImmediate(container);
        }
    }
}
