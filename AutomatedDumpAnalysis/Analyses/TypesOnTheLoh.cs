using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class TypesOnTheLoh : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            var dict = new Dictionary<string, ulong>();

            foreach (var instance in heap.EnumerateObjects())
            {
                if (instance.Size >= 84000 || instance.Type.GetSize(instance.Address) >= 84000)
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

            foreach (var typeOnTheLoh in dict.OrderBy(x => x.Value))
            {
                Console.WriteLine(typeOnTheLoh);
            }
        }
    }
}