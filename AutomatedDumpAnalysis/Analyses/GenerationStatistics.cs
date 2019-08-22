using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class GenerationStatistics : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            var dict = CreateDictionary();

            foreach (var instance in heap.EnumerateObjects())
            {
                var generation = heap.GetGeneration(instance.Address);
                dict[generation].Add(instance);
            }

            Console.WriteLine();
            Console.WriteLine($"Gen 0 size: {ObjectSizeSum(dict[0])}");
            Console.WriteLine($"Gen 0 object count: {dict[0].Count}");
            Console.WriteLine();
            Console.WriteLine($"Gen 1 size: {ObjectSizeSum(dict[1])}");
            Console.WriteLine($"Gen 1 object count: {dict[1].Count}");
            Console.WriteLine();
            Console.WriteLine($"Gen 2 size: {ObjectSizeSum(dict[2])}");
            Console.WriteLine($"Gen 2 object count: {dict[2].Count}");
        }

        private static ulong ObjectSizeSum(IEnumerable<ClrObject> objects)
        {
            return objects
                .Select(c => c.Size)
                .Aggregate((a, c) => a + c);
        }

        private static Dictionary<int, List<ClrObject>> CreateDictionary()
        {
            var dict = new Dictionary<int, List<ClrObject>>
            {
                {0, new List<ClrObject>()}, {1, new List<ClrObject>()}, {2, new List<ClrObject>()}
            };
            return dict;
        }
    }
}
