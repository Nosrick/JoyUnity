using System.Collections;
using System.Linq;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SkillHandlerTest
    {
        private EntitySkillHandler target;
    
        private GameObject container;
    
        private NeedHandler needHandler;

        private ObjectIconHandler objectIconHandler;
    
        [SetUp]
        public void Initialise()
        {
            container = new GameObject("GameManager");

            GlobalConstants.GameManager = container;

            objectIconHandler = container.AddComponent<ObjectIconHandler>();
            needHandler = container.AddComponent<NeedHandler>();
            target = container.AddComponent<EntitySkillHandler>();
        }
    
        [UnityTest]
        public IEnumerator GetDefaultSkillBlock_ShouldHave_ValidData()
        {
            BasicValueContainer<INeed> needs =
                new BasicValueContainer<INeed>(
                    needHandler.Needs.Keys.Select(needName => needHandler.GetRandomised(needName))
                        .ToList());
    
            BasicValueContainer<IGrowingValue> skills = target.GetDefaultSkillBlock(needs);
    
            foreach (IGrowingValue skill in skills)
            {
                Assert.That(skill.GoverningNeeds, Is.Not.Empty);
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container);
        }
    }
}

