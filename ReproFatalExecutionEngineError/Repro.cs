using System;

namespace ReproFatalExecutionEngineError
{
    internal static class Repro
    {
        public static void MakeItHappen()
        {
            var cache = new SomeData[1];

            var executor = new CallbackExecutor(callbackData =>
                {
                    // - Enable Dynamic Program Analysis
                    // - put a breakpoint here (or use Debugger.Break) and step over
                    // Platform target x86 will run without any issue
                    // Platform target AnyCpu or x64 will die with a FatalExecutionEngineError
                    // running the code without stepping in debug is fine
                    // disabling 'Dynamic Program Analyisis' 

                    System.Diagnostics.Debugger.Break(); // you can comment this and put a breakpoint on the next line
                    var arrayIndex = callbackData.ArrayIndex;

                    var someData = cache[callbackData.ArrayIndex];
                    if (someData == null)
                    {
                        someData = new SomeData();
                        cache[callbackData.ArrayIndex] = someData;
                    }
                    someData.Values = callbackData.SomeFloats;
                    someData.OneFloat = callbackData.Float1;
                    someData.AnotherFloat = callbackData.Float2;
                });

            executor.ExecuteTheCallback(new ReadOnlySpan<float>(new[] { 1f, 2, 3, 4 }));
        }
    }

    
    internal class CallbackExecutor 
    {
        private readonly Action<CallbackData> m_dataCallback;

        public CallbackExecutor(Action<CallbackData> callback)
        {
            m_dataCallback = callback;
        }

        public void ExecuteTheCallback(ReadOnlySpan<float> samples)
        {
            var signalData = new CallbackData(0, 0, 0, samples.Slice(0, 1).ToArray());

            m_dataCallback(signalData);
        }
    }

    internal class SomeData
    {
        public float[] Values { get; set; }
        public float OneFloat { get; set; }
        public float AnotherFloat { get; set; }
    }

    internal class CallbackData 
    {
        public float[] SomeFloats { get; }
        public float[] OtherFloats { get; }

        public int ArrayIndex { get; }
        public float Float1 { get; }
        public float Float2 { get; }

        public CallbackData(int arrayIndex, float float2, float float1, float[] someFloats)
        {
            OtherFloats = null;
            SomeFloats = someFloats;
            ArrayIndex = arrayIndex;
            Float1 = float1;
            Float2 = float2;
        }
    }
}
