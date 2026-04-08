using NUnit.Framework;

namespace EchoForest.Tests;

/// <summary>
/// Bootstrap tests that verify the CI test runner itself is operational.
/// These tests have no game logic dependencies.
///
/// TDD note: these are the first GREEN tests — they prove the pipeline works.
/// Real gameplay tests follow TDD RED → GREEN → REFACTOR from S0-03 onward.
/// </summary>
[TestFixture]
public class EngineBootTest
{
    [Test]
    public void CI_TestRunnerIsOperational() => Assert.Pass();

    [Test]
    public void NUnit_FrameworkLoads_Successfully() => Assert.Pass();

    [Test]
    public void SampleAssertion_TrueIsTrue() => Assert.That(true, Is.True);
}
