using System.IO;
using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public sealed class DumpByteArrays : IAnalyse
    {
        public void Execute(ClrHeap heap)
        {
            ulong counter = 0;

            foreach (var instance in heap.EnumerateObjects())
            {
                if (instance.Type.IsArray && instance.Type.Name == "System.Byte[]")
                {
                    if (instance.Size > 84000 || instance.Type.GetSize(instance.Address) > 84000)
                    {
                        int arrayLength = instance.Type.GetArrayLength(instance.Address);

                        var buffer = new byte[arrayLength];

                        for (var index = 0; index < arrayLength; index++)
                        {
                            buffer[index] = (byte) instance.Type.GetArrayElementValue(instance.Address, index);
                        }

                        File.WriteAllBytes($"file_{counter++}_{instance.Size}.txt", buffer);
                    }
                }
            }
        }
    }
}