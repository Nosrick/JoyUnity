using System.Collections;
using JoyLib.Code;
using JoyLib.Code.Entities.Jobs;
using JoyLib.Code.Rollers;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Tests
{
    public class JobHandlerTest
    {
        private IJobHandler target;

        [SetUp]
        public void SetUp()
        {
            target = new JobHandler(new RNG());
        }

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
        {
            //given

            //when
            IJob[] jobs = target.Jobs;

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
            GlobalConstants.GameManager = null;
        }
    }
}