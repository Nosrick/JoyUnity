using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Helpers
{
    public static class StatisticVarianceHelper
    {
        public static Dictionary<StatisticIndex, int> Get(Entity entity)
        {
            Dictionary<StatisticIndex, int> returnStats = entity.Statistics;

            foreach(StatisticIndex key in returnStats.Keys.ToList())
            {
                returnStats[key] += entity.Culture.StatVariance();
            }

            return returnStats;
        }
    }
}
