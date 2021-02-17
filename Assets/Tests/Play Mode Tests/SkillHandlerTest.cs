using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Statistics;
using JoyLib.Code.Helpers;
using JoyLib.Code.Scripting;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class SkillHandlerTest
    {
        private ScriptingEngine ScriptingEngine;
        
        private IEntitySkillHandler target;
    
        [SetUp]
        public void Initialise()
        {
            ActionLog actionLog = new ActionLog();
            GlobalConstants.ActionLog = actionLog;
            ScriptingEngine = new ScriptingEngine();
            
            target = new EntitySkillHandler();
        }
    
        [UnityTest]
        public IEnumerator GetDefaultSkillBlock_ShouldHave_ValidData()
        {
            IDictionary<string, IEntitySkill> skills = this.target.GetDefaultSkillBlock();
    
            foreach (IEntitySkill skill in skills.Values)
            {
                Assert.That(skill.Name, Is.Not.Empty);
                Assert.That(skill.Value, Is.Zero);
                Assert.That(skill.SuccessThreshold, Is.EqualTo(GlobalConstants.DEFAULT_SUCCESS_THRESHOLD));
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.GameManager = null;
            GlobalConstants.ActionLog.Dispose();
        }
    }
}

