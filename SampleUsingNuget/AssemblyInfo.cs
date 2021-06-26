using System;
using MrMeeseeks.StaticDelegateGenerator;
using SampleUsingNuget;

[assembly: StaticDelegate(typeof(DateTime))]
[assembly: StaticDelegate(typeof(UngenericSampleClass))]