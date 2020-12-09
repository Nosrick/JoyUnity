using System.Collections.Generic;

namespace JoyLib.Code.Entities.Jobs
{
    public interface IJobHandler
    {
        IJob Get(string jobName);
        IJob GetRandom();
        IEnumerable<IJob> Jobs { get; }
    }
}