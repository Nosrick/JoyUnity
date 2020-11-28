namespace JoyLib.Code.Entities.Jobs
{
    public interface IJobHandler
    {
        IJob Get(string jobName);
        IJob GetRandom();
        IJob[] Jobs { get; }
    }
}