using System.Collections;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Graphics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NeedHandlerTests
    {
        private INeedHandler target;
    
        [SetUp]
        public void SetUp()
        {
            target = new NeedHandler();
        }
    
        [UnityTest]
        public IEnumerator Initialise_ShouldHave_ValidData()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.Needs, Is.Not.Empty);
            foreach (INeed need in target.Needs)
            {
                Assert.That(need.Name, Is.Not.Empty);
                Assert.That(need.Name, Is.Not.EqualTo("DEFAULT"));
            }
            
            return null;
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
