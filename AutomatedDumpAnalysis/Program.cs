using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using AutomatedDumpAnalysis.Analyses;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args[0].EndsWith(".dmp") == false)
            {
                Console.WriteLine("First argument should be the canonical path to the *.dmp file");
                Environment.Exit(1);
            }

            Console.WriteLine("Loading dump... (this can take a while)");

            var runtime = CreateRuntime(args[0], string.Empty);
            var heap = runtime.Heap;

            Console.WriteLine("Dump loaded!");

            var dict = new Dictionary<int, IAnalyse>();
            dict[1] = new CountArrays();
            dict[2] = new CountStrings();
            dict[3] = new DumpByteArrays();
            dict[4] = new DumpCharArrays();
            dict[5] = new RootedObjects();
            dict[6] = new TypesOnTheLoh();
            dict[7] = new GenerationStatistics();

            PresentAnalysers(dict);

            while (true)
            {
                Console.WriteLine(Environment.NewLine);

                int id = AskForAnalyserToRun();

                if (id == -1)
                {
                    Environment.Exit(0);
                }

                if (dict.ContainsKey(id))
                {
                    var sw = Stopwatch.StartNew();
                    dict[id].Execute(heap);
                    Console.WriteLine($"Took: {sw.Elapsed}");
                }
                else
                {
                    Console.WriteLine("Invalid ID");
                }
                
                PresentAnalysers(dict);
            }
        }

        private static int AskForAnalyserToRun()
        {
            Console.WriteLine("ID of analyser you want to run? ");
            return int.Parse(Console.ReadLine());
        }

        private static void PresentAnalysers(Dictionary<int, IAnalyse> dict)
        {
            Console.WriteLine(Environment.NewLine);
            Console.WriteLine("Analyses available: ");

            foreach(var keyValuePair in dict)
            {
                Console.WriteLine($"{keyValuePair.Key}. {keyValuePair.Value.GetType().Name}");
            }

            Console.WriteLine("-1 to exit");
        }

        private static ClrRuntime CreateRuntime(string dump, string dac)
        {
            // Create the data target.  This tells us the versions of CLR loaded in the target process.
            DataTarget dataTarget = DataTarget.LoadCrashDump(dump);

            // Now check bitness of our program/target:
            bool isTarget64Bit = dataTarget.PointerSize == 8;

            if (Environment.Is64BitProcess != isTarget64Bit)
            {
                throw new Exception($"Architecture mismatch:  Process is {(Environment.Is64BitProcess ? "64 bit" : "32 bit")} but target is {(isTarget64Bit ? "64 bit" : "32 bit")}");
            }

            // Note I just take the first version of CLR in the process.  You can loop over every loaded
            // CLR to handle the SxS case where both v2 and v4 are loaded in the process.
            ClrInfo version = dataTarget.ClrVersions[0];

            // Next, let's try to make sure we have the right Dac to load.  Note we are doing this manually for
            // illustration.  Simply calling version.CreateRuntime with no arguments does the same steps.
            if (dac != null && Directory.Exists(dac))
            {
                dac = Path.Combine(dac, version.DacInfo.FileName);
            }
            else if (dac == null || !File.Exists(dac))
            {
                dac = dataTarget.SymbolLocator.FindBinary(version.DacInfo);
            }
                
            // Finally, check to see if the dac exists.  If not, throw an exception.
            if (dac == null || !File.Exists(dac))
            {
                throw new FileNotFoundException("Could not find the specified dac.", dac);
            }
            
            // Now that we have the DataTarget, the version of CLR, and the right dac, we create and return a
            // ClrRuntime instance.
            return version.CreateRuntime(dac);
        }
    }
}