using System.Collections;
using System.Linq;
using DevionGames.InventorySystem;
using JoyLib.Code;
using JoyLib.Code.Collections;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Graphics;
using JoyLib.Code.Scripting;
using JoyLib.Code.Unity.GUI;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SkillHandlerTest
    {
        private ScriptingEngine ScriptingEngine;
        
        private IEntitySkillHandler target;
        private INeedHandler NeedHandler;
    
        [SetUp]
        public void Initialise()
        {
            ScriptingEngine = new ScriptingEngine();
            
            NeedHandler = new NeedHandler();
            target = new EntitySkillHandler(NeedHandler);
        }
    
        [UnityTest]
        public IEnumerator GetDefaultSkillBlock_ShouldHave_ValidData()
        {
            BasicValueContainer<INeed> needs =
                new BasicValueContainer<INeed>(NeedHandler.GetManyRandomised(NeedHandler.NeedNames));
    
            BasicValueContainer<IGrowingValue> skills = target.GetDefaultSkillBlock(needs);
    
            foreach (IGrowingValue skill in skills.Values)
            {
                Assert.That(skill.GoverningNeeds, Is.Not.Empty);
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

