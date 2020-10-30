using System.Collections;
using JoyLib.Code.Entities.Jobs;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

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

        [UnityTest]
        public IEnumerator LoadTypes_ShouldHaveValidData()
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

            return null;
        }

        [TearDown]
        public void TearDown()
        {
            GameObject.DestroyImmediate(gameManager);
        }
    }
}