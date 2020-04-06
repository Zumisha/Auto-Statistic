using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Statistic
{
    public static class CoreNumParser
    {
        private static readonly string[] coreParams =
        {
            "--num-cores ",
            "-n ",
            "-N"
        };

        public static int parseCoreNum(string arguments)
        {
            int coreNum, corePos = -1, i = -1;
            while (corePos == -1 && i < coreParams.Length - 1)
            {
                ++i;
                corePos = arguments.IndexOf(coreParams[i]);
            }

            if (corePos == -1)
            {
                coreNum = 0;
            }
            else
            {
                string arg = arguments.Substring(corePos + coreParams[i].Length);
                int splitPos = arg.IndexOf(" ", StringComparison.Ordinal);
                if (splitPos != -1)
                    arg = arg.Substring(0, splitPos);
                if (!Int32.TryParse(arg, out coreNum))
                    coreNum = 0;
            }

            return coreNum;
        }
    }
}
