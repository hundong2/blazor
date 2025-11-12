using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace BlazorAppLee.Data
{
    public class ToolPlugins
    {
        [KernelFunction, Description("Function A: Use this function to get the initial data.")]
        public string FunctionA()
        {
            Console.WriteLine("Function A executed.");
            return "Data from A";
        }

        [KernelFunction, Description("Function B: Use this function to process the data from function A.")]
        public string FunctionB([Description("Data from function A")] string dataFromA)
        {
            Console.WriteLine($"Function B executed with data: {dataFromA}");
            return $"Processed data from B: {dataFromA}";
        }

        [KernelFunction, Description("Function C: Use this function for an alternative data processing path.")]
        public string FunctionC()
        {
            Console.WriteLine("Function C executed.");
            return "Data from C";
        }

        [KernelFunction, Description("Function D: Use this function to finalize the process.")]
        public string FunctionD([Description("Data from either function B or C")] string data)
        {
            Console.WriteLine($"Function D executed with data: {data}");
            return $"Finalized data from D: {data}";
        }
    }
}
