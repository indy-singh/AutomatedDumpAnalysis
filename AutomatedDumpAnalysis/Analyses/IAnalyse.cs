using Microsoft.Diagnostics.Runtime;

namespace AutomatedDumpAnalysis.Analyses
{
    public interface IAnalyse
    {
        void Execute(ClrHeap heap);
    }
}