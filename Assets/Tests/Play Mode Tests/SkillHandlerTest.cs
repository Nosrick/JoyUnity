using System.Linq;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class SkillHandlerTest
    {
        private EntitySkillHandler target;
    
        private GameObject container;
    
        private NeedHandler needHandler;
    
        [SetUp]
        public void Initialise()
        {
            container = new GameObject("GameManager");
            needHandler = container.AddComponent<NeedHandler>();
            target = container.AddComponent<EntitySkillHandler>();
        }
    
        [Test]
        public void GetDefaultSkillBlock_ShouldHave_ValidData()
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
            
            Object.DestroyImmediate(container);
        }
    }
}

