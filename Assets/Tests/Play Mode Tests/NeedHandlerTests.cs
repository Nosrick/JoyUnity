using System.Collections;
using JoyLib.Code;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Scripting;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class NeedHandlerTests
    {
        private ScriptingEngine ScriptingEngine;
        
        private INeedHandler target;
    
        [SetUp]
        public void SetUp()
        {
            ScriptingEngine = new ScriptingEngine();
            
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
            GlobalConstants.GameManager = null;
        }
    }
}
