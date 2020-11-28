using System.Collections;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Unity.GUI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SkillHandlerTest
    {
        private IEntitySkillHandler target;
    
        private IGameManager container;

        private GameObject inventoryManager;
    
        [SetUp]
        public void Initialise()
        {
            inventoryManager = new GameObject();
            inventoryManager.AddComponent<InventoryManager>();
            container = new GameObject("GameManager").AddComponent<GameManager>();

            GlobalConstants.GameManager = container;
            target = container.SkillHandler;
        }
    
        [UnityTest]
        public IEnumerator GetDefaultSkillBlock_ShouldHave_ValidData()
        {
            BasicValueContainer<INeed> needs =
                new BasicValueContainer<INeed>(
                    container.NeedHandler.GetManyRandomised(container.NeedHandler.NeedNames));
    
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
            GameObject.DestroyImmediate(container.MyGameObject);
        }
    }
}

