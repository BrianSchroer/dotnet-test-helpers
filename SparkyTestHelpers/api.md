* **SparkyTestHelpers.Exceptions**: for testing exception expectations
* **SparkyTestHelpers.Scenarios**: for testing a method with a variety of different input scenarios

_see also_: 
* the rest of the [**"Sparky suite"** of .NET utilities and test helpers](https://www.nuget.org/profiles/BrianSchroer)
---
## AssertExceptionNotThrown
Assert exception is not thrown when an action is executed. This method doesn't do much,
but clarifies the intent of tests that wish to show that an action works correctly.

**Static Methods**

* _void_ **WhenExecuting**_(Action action)_ 

**Example**

```csharp
using SparkyTestHelpers.Exceptions;
. . .
AssertExceptionNotThrown.WhenExecuting(() => foo.Bar(baz));
```
---

## AssertExceptionThrown
Used to assert than an expected exception is thrown when a test action is executed.

Why use this instead of something like the VisualStudio TestTools ExpectedExceptionAttribute?

* It lets you to check the exception message.
* It lets you assert the exception is thrown for a specific statement, not just anywhere in the code under test.

There is no public constructor for this class. It's constructed using the "fluent" static factory method **AssertExceptionThrown.OfType<*TException*>()**.

**Example**

```csharp
using SparkyTestHelpers.Exceptions;
. . .
AssertExceptionThrown
    .OfType<ArgumentOutOfRangeException>()
    .WithMessage("Limit cannot be greater than 10.")
    .WhenExecuting(() => { var foo = new Foo(limit: 11); });
```

**Methods**

* _AssertExceptionThrown_ **WithMessage** _(String expected)_  
  Set up to test that the action under test throws an exception where the message exactly matche.
* _AssertExceptionThrown_ **WithMessageStartingWith** _(String expected)_  
  Set up to test that the action under test throws an exception where the message starts with expected string.
* _AssertExceptionThrown_ **WithMessageContaining** _(String expected)_  
  Set up to test that the action under test throws an exception where the message contains substring.
* _AssertExceptionThrown_ **WithMessageMatching** _(String regExPattern)_  
  Set up to test that the action under test throws an exception where the message matches RegEx pattern.
* _Exception_ **WhenExecuting** _(Action action)_  
  Call action that should throw an exception, and assert that the exception was thrown.

**Static Methods**

* _AssertExceptionThrown_.OfType<*TException*>()   
   Set up to test that the action under test throws an exception of this specific type.

* _AssertExceptionThrown_.OfTypeOrSubclassOfType<*TException*>\()   
   Set up to test that the action under test throws an exception of this type of a subclass of this type.
---

## ScenarioTester<*TScenario*>
VisualStudio.TestTools now has ["DataMemberTest" and "DataRow" attributes](http://pmichaels.net/2016/07/23/using-mstest-datarow-as-a-substitute-for-nunit-testcase/),  but they didn't when I started using the framework, so I wrote my own "scenerio testing" tool based my experience with NUnit "TestCase" attributes. This class provides the ability to execute the same test code for multiple test cases and, after all test cases have been executed, fail the unit test if any of the test cases failed.

Even if your test framework has attribute-based scenario testing, these helpers provide an alternative syntax that you might find useful.

This class is rarely used directly. It's easier to use with IEnumerable<*TScenario*>.**TestEach** or **ForTest.Scenarios** (see below).

When one or more of the scenarios fails, the failure exception shows which were unsuccessful, for example, this scenario test:

```csharp
ForTest.Scenarios
(
    new { DateString = "1/31/2023", IsGoodDate = true },
    new { DateString = "2/31/2023", IsGoodDate = true },
    new { DateString = "6/31/2023", IsGoodDate = false }
)
.TestEach(scenario =>
{
    DateTime dt;
    Assert.AreEqual(scenario.IsGoodDate, DateTime.TryParse(scenario.DateString, out dt));
});
```

...throws this exception:

```
Test method SparkyTestHelpers.UnitTests.DateTests threw exception:
SparkyTestHelpers.Scenarios.ScenarioTestFailureException: Scenario[1] (2 of 3) - Assert.AreEqual failed. Expected:<True>. Actual:<False>.

Scenario data - anonymousType: {"DateString":"2/31/2023","IsGoodDate":true}

Scenario[2] (3 of 3) - Assert.AreEqual failed. Expected:<True>. Actual:<False>.

Scenario data - anonymousType: {"DateString":"4/31/2023","IsGoodDate":true}
```

**Public Methods**

* *ScenarioTester* **BeforeEachTest**(Action<*TScenario*>)
 
    Defines action to called before each scenario is tested.

**Example**
```csharp
ForTest.Scenarios(*array*)
.BeforeEachTest(scenario => 
{
    // do something, e.g. reset mocks, log scenario details
});
.TestEach(scenario =>
{
    DateTime dt;
    Assert.AreEqual(scenario.IsGoodDate, DateTime.TryParse(scenario.DateString, out dt));
});
```
* *ScenarioTester* **AfterEachTest**(*Action<*TScenario*>*)
 
    Defines function called after each scenario is tested. The function receives the scenario and the exception (if any) caught by the test. If the function returns true, the scenario test is "passed". If false, exception is thrown to fail the test.

**Example**
```csharp
ForTest.Scenarios(*array*)
.AfterEachTest((scenario, ex) => 
{
    // do something, e.g. log scenario details, 
    // decide if scenario with exception should be passed
    return ex == null;
});
.TestEach(scenario =>
{
    DateTime dt;
    Assert.AreEqual(scenario.IsGoodDate, DateTime.TryParse(scenario.DateString, out dt));
});
```
 ---

## ScenarioTesterExtension
**ScenarioTester<*TScenario*>** extension methods.

**Static Methods**

* _ScenarioTester<*TScenario*>_ **TestEach**_(IEnumerable<*TScenario*> enumerable, Action<*TScenario*> test)_

**Example**

```csharp
using SparkyTestHelpers.Scenarios;
. . .
new []
{
    new { DateString = "1/31/2023", IsGoodDate = true },  
    new { DateString = "2/31/2023", IsGoodDate = false } 
}
.TestEach(scenario =>
{
    DateTime dt;
    Assert.AreEqual(scenario.IsGoodDate, DateTime.TryParse(scenario.DateString, out dt));  
});  
```
---

## ForTest
"Syntactic sugar" methods for working with **ScenarioTester<*TScenario*>**

**Static Methods**

* **ForTest.Scenarios**_(TScenario[] scenarios)_ - creates array of scenarios that can be "dotted" to the **TestEach** extension method:

**Example**

```csharp
using SparkyTestHelpers.Scenarios;
. . .
ForTest.Scenarios
(
    new { DateString = "1/31/2023", IsGoodDate = true },  
    new { DateString = "2/31/2023", IsGoodDate = false }
)
.TestEach(scenario =>
{
    DateTime dt;
    Assert.AreEqual(scenario.IsGoodDate, DateTime.TryParse(scenario.DateString, out dt));  
});  
```

* _IEnumerable<*TEnum*>_ **EnumValues**_()_ - tests each value in an enum.

**_Example_**

```csharp
using SparkyTestHelpers.Scenarios;
. . .
ForTest.EnumValues<OrderStatus>()
    .TestEach(orderStatus => foo.Bar(orderStatus));
```

* _IEnumerable<*TEnum*>_ **ExceptFor**_(IEnumerable<*TEnum*> values, TEnum[] valuesToExclude)_ - Exclude enum values from test scenarios.

**_Example_**

```csharp
using SparkyTestHelpers.Scenarios;
. . .
ForTest.EnumValues<OrderStatus>()
    .ExceptFor(OrderStatus.Cancelled)
    .TestEach(orderStatus => foo.Bar(orderStatus));
```