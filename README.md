# MrMeeseeks.StaticDelegateGenerator

This Generator applies the Delegate pattern (similar or synonomous to Proxy, Wrapper, Adapter patterns) to the static elements (properties and methods) of a given type.

The explanations of the whys and hows here won't go much into the details of the basics. It's recommended to have a look into the Dependency Inversion Principle (DIP; it's one of the SOLID principles) & Dependency Injection and Object Oriented Programming (OOP) patterns like Proxy, Wrapper, Adapter and Delegate patterns. The patterns are all similar in the way how they are implemtented. They just differ in use cases. This project chose to go with the term Delegate, because it seemed to be most fitting.

## Why and which problem does it solve?

Let's directly start with a bad example:

```C#
internal class BadExampleUsingStaticDependencyInConstructor
{
    public BadExampleUsingStaticDependencyInConstructor()
    {
        Console.WriteLine(DateTime.Now);
    }
}
```

This is a violation of the DIP, because `DateTime.Now` is a dependency on a concrete implementation rather than an abstraction. That is always the case with references to static code. The issue here is that you cannot switch implementations. For example, this would make unit tests which depend on `DateTime.Now` returning a very concrete value impossible. 


This is just indirectly the problem which `MrMeeseeks.StaticDelegateGenerator` solves. The problem which this project solves directly can be inferred from the solution to the problem of the bad example abover. Meaning the solution being the next problem. Here the solution:

```C#
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
```

Solution: we delegate to the static code and wrap it into an concrete implementation implementing an interface which finally gets injected by constructor injection (DI). Let's call that the (Static) Delegate pattern. 

DIP is happy, problem solved, right? Not yet. Here comes the human component into play. Maintaining the Delegate pattern for all static references becomes tedious and monotonous fast. Also it bloats the code base (more code, more files) without doing something new (it just delegates to already existing functionality). 

And that is exactly the problem which `MrMeeseeks.StaticDelegateGenerator` is trying to solves.

## Usage

First, get the latest release version of the nuget package:

```
Install-Package MrMeeseeks.StaticDelegateGenerator
```

Then declare which type you want to get a Static Delegate from via the `StaticDelegate`-Attribute. For `DateTime` it would look like:

```C#
using System;
using MrMeeseeks.StaticDelegateGenerator;

[assembly: StaticDelegate(typeof(DateTime))]
```

The Source Generator will then generate an interface called `IDateTimeStaticDelegate` and a corresponding concrete implementation `DateTimeStaticDelegate` which you can use inplace of `DateTime`:

```C#
internal class SolvedExampleWithStaticDelegate
{
    public SolvedExampleWithStaticDelegate(
        IDateTimeStaticDelegate dateTimeNowStaticDelegate)
    {
        Console.WriteLine(dateTimeNowStaticDelegate.Now);
    }
}
```

Of course you would need to setup your DI to inject `DateTimeStaticDelegate` into this constructor parameter. However, this is a topic of its own which won't be covered here.
