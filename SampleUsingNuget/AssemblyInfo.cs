using System;
using MrMeeseeks.StaticDelegateGenerator;
using SampleUsingNuget;

[assembly: StaticTypeToDelegate(typeof(DateTime))]

[assembly: StaticTypeToDelegate(typeof(UngenericSampleClass))]