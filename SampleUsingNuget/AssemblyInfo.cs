using System;
using MrMeeseeks.StaticDelegate.SampleUsingNuget;
using MrMeeseeks.StaticDelegateGenerator;

[assembly: StaticDelegate(typeof(DateTime))]
[assembly: StaticDelegate(typeof(UngenericSampleClass))]