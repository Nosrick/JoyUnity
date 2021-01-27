using System.Collections;
using System.Linq;
using JoyLib.Code;
using JoyLib.Code.Entities.Abilities;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Helpers;
using JoyLib.Code.Rollers;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class JobHandlerTest
    {
        private IJobHandler target;
        private IAbilityHandler abilityHandler;

        private ActionLog log;

        [SetUp]
        public void SetUp()
        {
            GlobalConstants.ActionLog = new ActionLog();
            this.abilityHandler = new AbilityHandler();
            this.target = new JobHandler(this.abilityHandler, new RNG());
        }

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
        {
            //given

            //when
            IJob[] jobs = target.Jobs.ToArray();

            //then
            Assert.That(jobs, Is.Not.Empty);
            foreach(IJob job in jobs)
            {
                Assert.That(job.SkillDiscounts, Is.Not.Empty);
                Assert.That(job.StatisticDiscounts, Is.Not.Empty);
                Assert.IsNotEmpty(job.Name);
            }

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GlobalConstants.ActionLog.Dispose();
        }
    }
}