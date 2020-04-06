using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;

namespace Auto_Statistic
{
    public class CheckAlg
    {
        private readonly object checkAlgorithm;

        public static readonly string defaultAlg =
            @"return (output.IndexOf(""Error"") == -1) && 
            (output.IndexOf(expected) != -1);";

        /*string[] outFile = System.IO.File.ReadAllText("out.txt").Replace("\n"," ").Trim().Split(' ');
        string[] refFile = System.IO.File.ReadAllText("reference.txt").Replace("\n", " ").Trim().Split(' ');
        for (var i = 0; i < System.Math.Min(outFile.Length,refFile.Length); ++i)
        {
            if (System.Math.Abs(double.Parse(outFile[i], CultureInfo.InvariantCulture) - double.Parse(refFile[i], CultureInfo.InvariantCulture)) > double.Epsilon)
            {
                return false;
            }
        }
        return true;
        */

        private static readonly Dictionary<string, string> providerOptions = new Dictionary<string, string> { { "CompilerVersion", "v4.0" } };
        private static readonly CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
        private static readonly CompilerParameters compilerParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };

        private string makeProgram(string text)
        {
            var builder = new StringBuilder();

            builder.Append("namespace Checker { public class Checker { public static bool Check(string output, string expected) {");
            builder.Append(text);
            builder.Append("} } }");

            return builder.ToString();
        }

        public CheckAlg()
        {
            string program = makeProgram(defaultAlg);
            CompilerResults res = provider.CompileAssemblyFromSource(compilerParams, program);
            checkAlgorithm = res.CompiledAssembly.CreateInstance("Checker.Checker");
        }

        public CheckAlg(string text)
        {
            string program = makeProgram(text);

            CompilerResults res = provider.CompileAssemblyFromSource(compilerParams, program);
            
            if (res.Errors.Count != 0)
            {
                StringBuilder errors = new StringBuilder();
                foreach (var error in res.Errors)
                {
                    errors.AppendLine(error.ToString());
                }

                throw new Exception(errors.ToString());
            }

            checkAlgorithm = res.CompiledAssembly.CreateInstance("Checker.Checker");
        }

        public bool Check(string expected, string received)
        {
            MethodInfo check = checkAlgorithm.GetType().GetMethod("Check");
            return (bool) check.Invoke(null, new object[] { received, expected });
        }
    }
}
