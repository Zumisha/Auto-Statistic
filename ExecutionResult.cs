using System;
using Auto_Statistic.Storage;

namespace Auto_Statistic
{
    public partial class Executor
    {
        [Serializable]
        public struct ExecutionResult
        {
            public string programName;
            public string interpArgs;
            public string arguments;
            public int threadsCount;
            public double avgExecTime;
            public double variance;
            public float maxMemUsage;
            public float maxCpuUsage;
            public float avgCpuUsage;
            public ExecutionStatus execStatus;

            private static string CsvStringBuilder(string programName, string interprArgs, string arguments, string threadsCount,
                string avgExecTime, string variance, string maxMemUsage, string maxCpuUsage, string avgCpuUsage,
                string execStatus)
            {
                return $"\"{programName}\";\"{arguments}\";\"{threadsCount}\";\"{avgExecTime}\";\"{variance}\";\"{maxMemUsage}\";\"{maxCpuUsage}\";\"{avgCpuUsage}\";\"{execStatus}\"";
            }

            public static string CsvNames()
            {
                return CsvStringBuilder("Program", "Interpr Args", "Arguments", "Threads", "Time, s", "Variance", "Max RAM, MB",
                    "Max CPU, %", "Avg CPU, %", "Status");
            }

            public static string CsvEmpty()
            {
                return CsvStringBuilder("","", "", "", "", "", "", "", "", "");
            }

            public string CsvResults()
            {
                return CsvStringBuilder(programName, interpArgs, arguments, threadsCount.ToString(), avgExecTime.ToString("F3"),
                    variance.ToString("F4"), maxMemUsage.ToString("F"), maxCpuUsage.ToString("P2"),
                    avgCpuUsage.ToString("P2"), execStatus.ToString());
            }
        }
    }
}
