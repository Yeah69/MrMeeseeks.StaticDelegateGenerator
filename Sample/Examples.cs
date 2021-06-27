using System;

namespace MrMeeseeks.StaticDelegate.Sample
{
    internal class BadExampleUsingStaticDependencyInConstructor
    {
        public BadExampleUsingStaticDependencyInConstructor()
        {
            Console.WriteLine(DateTime.Now);
        }
    }

    internal interface IDateTimeNowStaticDelegate
    {
        DateTime Now { get; }
    }

    internal class DateTimeNowStaticDelegate : IDateTimeNowStaticDelegate
    {
        public DateTime Now => DateTime.Now;
    }
    
    internal class SolvedExampleWithConstructorInjection
    {
        public SolvedExampleWithConstructorInjection(
            IDateTimeNowStaticDelegate dateTimeNowStaticDelegate)
        {
            Console.WriteLine(dateTimeNowStaticDelegate.Now);
        }
    }
    
    internal class SolvedExampleWithStaticDelegate
    {
        public SolvedExampleWithStaticDelegate(
            IDateTimeStaticDelegate dateTimeNowStaticDelegate)
        {
            Console.WriteLine(dateTimeNowStaticDelegate.Now);
        }
    }
}