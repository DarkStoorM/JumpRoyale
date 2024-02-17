using JumpRoyale;
using JumpRoyale.Events;
using JumpRoyale.Utils;

namespace Tests;

[TestFixture]
public class JumperEventTests
{
    private bool _testCase;

    [SetUp]
    public void SetUp()
    {
        _testCase = false;
    }

    [Test]
    public void DummyTest()
    {
        JumperEvents.Instance.OnJumpEvent += Listener;

        JumperEvents.Instance.InvokeEvent<JumpCommandEventArgs>(JumperEventTypes.OnJumpEvent, new(1, 1));

        Assert.That(_testCase, Is.True);

        JumperEvents.Instance.OnJumpEvent -= Listener;
    }

    private void Listener(object sender, EventArgs args)
    {
        _testCase = true;
    }
}
