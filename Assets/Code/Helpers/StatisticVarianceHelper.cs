using JoyLib.Code.Entities;
using System.Collections.Generic;
using System.Linq;

namespace JoyLib.Code.Helpers
{
    public static class StatisticVarianceHelper
    {
        public static Dictionary<StatisticIndex, EntityStatistic> Get(Entity entity)
        {
            Dictionary<StatisticIndex, EntityStatistic> returnStats = entity.Statistics;

            foreach(StatisticIndex key in returnStats.Keys.ToList())
            {
                returnStats[key].Modify(entity.Culture.StatVariance());
            }

            return returnStats;
        }
    }
}
