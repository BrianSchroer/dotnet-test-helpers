using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SparkyTestHelpers.Exceptions;
using SparkyTestHelpers.Scenarios;
using SparkyTestHelpers.Scenarios.MsTest;

namespace SparkyTestHelpers.Scenarios.MsTest.UnitTests
{
    /// <summary>
    /// <see cref="MsTestScenarioTester{TScenario}" /> unit tests.
    /// </summary>
    [TestClass]
    public class MsTestScenarioTesterTests
    {
        [TestMethod]
        public void TestEach_should_throw_inconclusive_exception_when_all_exceptions_are_inconclusive()
        {
            var scenarios = new[] { "a", "b", "c" };

            AssertExceptionThrown
                .OfType<AssertInconclusiveException>()
                .WhenExecuting(() => new MsTestScenarioTester<string>(scenarios)
                  .TestEach(scenario => throw new AssertInconclusiveException("fail")));
        }

        [TestMethod]
        public void TestEach_should_throw_failure_exception_when_not_all_exceptions_are_inconclusive()
        {
            var scenarios = new[] { "a", "b", "c" };

            AssertExceptionThrown
                .OfType<ScenarioTestFailureException>()
                .WhenExecuting(() => new MsTestScenarioTester<string>(scenarios)
                  .TestEach(scenario =>
                  {
                      switch (scenario)
                      {
                          case "b":
                              throw new AssertInconclusiveException("inconclusive");
                          default:
                              throw new InvalidOperationException("fail");
                      }
                  }));
        }
    }
}
