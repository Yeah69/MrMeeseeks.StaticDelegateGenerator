using System;
using MrMeeseeks.StaticDelegate.Sample;
using MrMeeseeks.StaticDelegateGenerator;

[assembly: StaticTypeToDelegate(typeof(DateTime))]

[assembly: StaticTypeToDelegate(typeof(UngenericSampleClass))]