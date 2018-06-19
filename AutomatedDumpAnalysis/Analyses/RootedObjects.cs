using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class RootedObjects : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            //var rootNames = new HashSet<string>();
            //rootNames.Add("Strong handle");
            //rootNames.Add("Pinned handle");
            //rootNames.Add("SizedRef handle");
            //rootNames.Add("AsyncPinned handle");
            //rootNames.Add("RefCount handle");
            //rootNames.Add("finalization handle");
            //rootNames.Add("local var");

            //foreach (var rootName in rootNames)
            //{
                var dict = new Dictionary<string, ulong>();

                foreach (var root in heap.EnumerateRoots())
                {
                    //if (root.Name == rootName)
                    //{
                        if (dict.ContainsKey(root.Name))
                        {
                            dict[root.Name]++;
                        }
                        else
                        {
                            dict[root.Name] = 1;
                        }
                    //}
                }

                //File.WriteAllLines($"{rootName}.txt", dict.Select(pair => pair.ToString()));
            //}

            foreach (var keyValuePair in dict.OrderBy(x => x.Value))
            {
                Console.WriteLine(keyValuePair);

            }
        }
    }
}