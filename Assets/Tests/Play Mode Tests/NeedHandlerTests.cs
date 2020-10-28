using JoyLib.Code.Entities.Needs;
using NUnit.Framework;
using UnityEngine;

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
    
        [Test]
        public void Initialise_ShouldHave_ValidData()
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
            
            GameObject.DestroyImmediate(container);
        }
    }
}
