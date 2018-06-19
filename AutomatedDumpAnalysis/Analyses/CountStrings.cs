using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class CountStrings : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            var dict = new Dictionary<string, ulong>();

            foreach (var instance in heap.EnumerateObjects())
            {
                if (instance.Type.IsString)
                {
                    var value = (string) instance.Type.GetValue(instance.Address);

                    if (dict.ContainsKey(value))
                    {
                        dict[value]++;
                    }
                    else
                    {
                        dict[value] = 1;
                    }
                }
            }

            foreach (var pair in dict.OrderByDescending(x => x.Value).Take(50))
            {
                Console.WriteLine(pair);
            }
        }
    }
}