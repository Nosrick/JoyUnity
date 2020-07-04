using JoyLib.Code.Entities.Jobs;
using NUnit.Framework;

namespace Tests
{
    public class JobHandlerTest
    {
        private JobHandler target;

        [SetUp]
        public void SetUp()
        {
            target = new JobHandler();
        }

        [Test]
        public void LoadTypes_ShouldHaveValidData()
        {
            //given

            //when
            JobType[] jobs = JobHandler.instance.Jobs;

            //then
            Assert.That(jobs, Is.Not.Empty);
            foreach(JobType job in jobs)
            {
                Assert.That(job.SkillGrowths, Is.Not.Empty);
                Assert.That(job.StatisticGrowths, Is.Not.Empty);
                Assert.IsNotEmpty(job.Name);
            }
        }
    }
}