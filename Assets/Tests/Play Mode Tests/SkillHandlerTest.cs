using System.Collections;
using System.Collections.Generic;
using JoyLib.Code;
using JoyLib.Code.Entities;
using JoyLib.Code.Entities.Needs;
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
        private INeedHandler NeedHandler;
    
        [SetUp]
        public void Initialise()
        {
            ActionLog actionLog = new ActionLog();
            GlobalConstants.ActionLog = actionLog;
            ScriptingEngine = new ScriptingEngine();
            
            NeedHandler = new NeedHandler();
            target = new EntitySkillHandler(NeedHandler);
        }
    
        [UnityTest]
        public IEnumerator GetDefaultSkillBlock_ShouldHave_ValidData()
        {
            IDictionary<string, INeed> needs = new Dictionary<string, INeed>();
            ICollection<INeed> collection = NeedHandler.GetManyRandomised(NeedHandler.NeedNames);
            foreach (INeed need in collection)
            {
                needs.Add(need.Name, need);
            }
    
            IDictionary<string, IEntitySkill> skills = target.GetDefaultSkillBlock(needs.Values);
    
            foreach (EntitySkill skill in skills.Values)
            {
                Assert.That(skill.GoverningNeeds, Is.Not.Empty);
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

