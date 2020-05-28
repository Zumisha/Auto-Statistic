using System;
using System.CodeDom.Compiler;
//using System.CodeDom.Compiler;
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using CSharpCodeProvider = Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider;

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

        private static string CompilerFullPath(string relativePath)
        {
            string frameworkFolder = Path.GetDirectoryName(typeof(object).Assembly.Location);
            string compilerFullPath = Path.Combine(frameworkFolder, relativePath);

            return compilerFullPath;
        }

        private const int DefaultCompilerServerTTL = 0; // set TTL to 0 to turn of keepalive switch
        private static readonly ProviderOptions providerOptions = new ProviderOptions(CompilerFullPath(@"csc.exe"), DefaultCompilerServerTTL);
        private static readonly CSharpCodeProvider provider = new CSharpCodeProvider(providerOptions);
        private static readonly CompilerParameters compilerParams = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false
        };

        private string makeProgram(string usings, string classes, string text)
        {
            var builder = new StringBuilder();
            builder.Append("using System; using System.Collections.Generic;");
            builder.Append(usings);
            builder.Append("namespace Checker {");
            builder.Append(classes);
            builder.Append(" public class Checker { public static bool Check(string output, string expected, Dictionary<string, Object> storage) {");
            builder.Append(text);
            builder.Append("} } }");

            return builder.ToString();
        }

        public CheckAlg()
        {
            string program = makeProgram("", "",defaultAlg);
            CompilerResults res = provider.CompileAssemblyFromSource(compilerParams, program);
            checkAlgorithm = res.CompiledAssembly.CreateInstance("Checker.Checker");
        }

        public CheckAlg(string usings, string classes, string text)
        {
            string program = makeProgram(usings, classes, text);

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

        public bool Check(string expected, string received, Dictionary<string, object> storage)
        {
            MethodInfo check = checkAlgorithm.GetType().GetMethod("Check");
            return (bool) check.Invoke(null, new object[] { received, expected, storage });
        }
    }
}
