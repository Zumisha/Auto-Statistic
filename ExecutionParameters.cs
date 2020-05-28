using System;
using System.Collections.Generic;

namespace Auto_Statistic
{
    public partial class Executor
    {
        [Serializable]
        public class ExecutionParameters
        {
            public List<string> executionFilesPaths = new List<string>();
            public List<string> textProgramFilesPaths = new List<string>();
            public List<string> interprParams = new List<string>();
            public List<string> startParams = new List<string>();
            public List<string> referenceResults = new List<string>();
            public string checkAlgorithmUsingsText = "";
            public string checkAlgorithmClassesText = "";
            public string checkAlgorithmText = CheckAlg.defaultAlg;
            public ushort launchNum = 10;
            public float variance = 0.0005f;
            public byte backProcLimit = 10;
            public bool prohibitUsePageFile = true;
            public bool Interpr = true;
            public int timeLimit = 0;
        }
    }
}
