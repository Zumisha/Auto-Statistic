using System.IO;
using System.Text;

namespace Auto_Statistic
{
    partial class Profiler
    {
        public class ProfilerResultStatistic
        {
            public float maxMemUsage = 0;
            public float maxCpuUsage = 0;
            public float avgCpuUsage = 0;
            public double execTime = 0;
            public StringBuilder programResult = new StringBuilder();

            public void LogToFile(StreamWriter outLogFS)
            {
                outLogFS.WriteLine(programResult.ToString());
                outLogFS.WriteLine("\n.........................");
                outLogFS.WriteLine($"Time: {execTime:F3}s");
                outLogFS.WriteLine($"Max mem usage: {maxMemUsage:F}MB");
                outLogFS.WriteLine($"Max CPU usage: {maxCpuUsage:P2}%");
                outLogFS.WriteLine($"Avg CPU usage: {avgCpuUsage:P2}%");
                outLogFS.WriteLine(".........................\n");
            }
        }
    }
}
