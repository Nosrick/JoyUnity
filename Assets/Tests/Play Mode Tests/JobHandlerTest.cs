using JoyLib.Code.Entities.Jobs;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    public class JobHandlerTest
    {
        private JobHandler target;

        private GameObject gameManager;

        [SetUp]
        public void SetUp()
        {
            gameManager = new GameObject("GameManager");

            target = gameManager.AddComponent<JobHandler>();
        }

        [Test]
        public void LoadTypes_ShouldHaveValidData()
        {
            //given

            //when
            JobType[] jobs = target.Jobs;

            //then
            Assert.That(jobs, Is.Not.Empty);
            foreach(JobType job in jobs)
            {
                Assert.That(job.SkillGrowths, Is.Not.Empty);
                Assert.That(job.StatisticGrowths, Is.Not.Empty);
                Assert.IsNotEmpty(job.Name);
            }

            GameObject.DestroyImmediate(gameManager);
        }
    }
}