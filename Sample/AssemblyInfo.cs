using System;
using MrMeeseeks.StaticDelegate.Sample;
using MrMeeseeks.StaticDelegateGenerator;

[assembly: StaticDelegate(typeof(DateTime))]

[assembly: StaticDelegate(typeof(UngenericSampleClass))]