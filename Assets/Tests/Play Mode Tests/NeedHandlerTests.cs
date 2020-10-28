using System.Collections;
using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class NeedHandlerTests
    {
        private GameObject container;
    
        private NeedHandler target;
    
        [SetUp]
        public void SetUp()
        {
            container = new GameObject("GameManager");
            target = container.AddComponent<NeedHandler>();
        }
    
        [UnityTest]
        public IEnumerator Initialise_ShouldHave_ValidData()
        {
            //given
            
            //when
            
            //then
            Assert.That(target.Needs, Is.Not.Empty);
            foreach (INeed need in target.Needs.Values)
            {
                Assert.That(need.Name, Is.Not.Empty);
                Assert.That(need.Name, Is.Not.EqualTo("DEFAULT"));
            }
            
            yield return new WaitForSeconds(0.01f);
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(container);
        }
    }
}
