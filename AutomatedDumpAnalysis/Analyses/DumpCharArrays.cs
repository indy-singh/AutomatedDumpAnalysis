using System.IO;
using System.Text;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class DumpCharArrays : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            ulong counter = 0;

            foreach (var instance in heap.EnumerateObjects())
            {
                if (instance.Type.IsArray && instance.Type.Name == "System.Char[]")
                {
                    if (instance.Size > 84000 || instance.Type.GetSize(instance.Address) > 84000)
                    {
                        int arrayLength = instance.Type.GetArrayLength(instance.Address);

                        var chars = new char[arrayLength];

                        for (var index = 0; index < arrayLength; index++)
                        {
                            chars[index] = (char)instance.Type.GetArrayElementValue(instance.Address, index);
                        }

                        var sb = new StringBuilder();

                        foreach(var c in chars)
                        {
                            sb.Append(c);
                        }

                        File.WriteAllText($"file_{counter++}_{instance.Size}.txt", sb.ToString());
                    }
                }
            }
        }
    }
}