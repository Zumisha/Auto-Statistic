namespace Auto_Statistic
{
    partial class Profiler
    {
        public struct StatisticUnit
        {
            public readonly double time;
            public readonly float cpuUsage;
            public readonly float ramUsage;

            public StatisticUnit(float cpuUsage, float ramUsage, double time)
            {
                this.cpuUsage = cpuUsage;
                this.ramUsage = ramUsage;
                this.time = time;
            }

        }
    }
}
