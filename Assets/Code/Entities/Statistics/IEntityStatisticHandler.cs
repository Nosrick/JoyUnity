using System.Collections.Generic;

namespace JoyLib.Code.Entities.Statistics
{
    public interface IEntityStatisticHandler
    {
        IEnumerable<string> StatisticNames { get; }
    }
}