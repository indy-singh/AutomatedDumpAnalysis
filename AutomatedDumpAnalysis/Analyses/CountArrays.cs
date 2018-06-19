using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class CountArrays : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            var dict = new Dictionary<string, ulong>();

            foreach (var instance in heap.EnumerateObjects())
            {
                if (instance.Type.IsArray)
                {
                    if (dict.ContainsKey(instance.Type.Name))
                    {
                        dict[instance.Type.Name]++;
                    }
                    else
                    {
                        dict[instance.Type.Name] = 1;
                    }
                }
            }

            foreach (var pair in dict.OrderBy(x => x.Value))
            {
                Console.WriteLine(pair);
            }
        }
    }
}