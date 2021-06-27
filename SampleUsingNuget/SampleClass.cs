using System;

namespace MrMeeseeks.StaticDelegate.SampleUsingNuget
{
    class UngenericSampleClass
    {
        public static int Ungeneric { get; set; }
        public static ValueTuple<int> Generic { get; set; }
        public static int UngenericGetOnly { get; }
        public static ValueTuple<int> GenericGetOnly { get; }
        public static int IntReturn() => 0;
        public static void VoidReturn() { }
        public static void OutParameter(out int i) => i = 1;
        public static void RefParameter(ref int i) => i = 1;
        public static void InParameter(in int i) { }
        public static void MultipleParameters(int i, int ii) { }
        public static void Params(params int[] args) { }
    }
}
