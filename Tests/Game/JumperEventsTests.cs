using JumpRoyale;
using JumpRoyale.Commands;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using Tests.Mocks;

namespace Tests;

[TestFixture]
public class JumperEventTests
{
    private Jumper _fakeJumper;

    private JumpCommandEventArgs? _jumpCommandEventArgs;
    private DisableGlowEventArgs? _disableGlowEventArgs;
    private SetCharacterEventArgs? _characterChangeEventArgs;
    private SetGlowColorEventArgs? _glowColorChangeEventArgs;
    private SetNameColorEventArgs? _nameColorChangeEventArgs;

    [SetUp]
    public void SetUp()
    {
        _fakeJumper = new(FakePlayerData.Make());

        _fakeJumper.JumperEventsManager.OnJumpCommandEvent += JumpCommandListener;
        _fakeJumper.JumperEventsManager.OnDisableGlowEvent += DisableGlowListener;
        _fakeJumper.JumperEventsManager.OnSetCharacterEvent += SetCharacterListener;
        _fakeJumper.JumperEventsManager.OnSetGlowColorEvent += SetGlowColorListener;
        _fakeJumper.JumperEventsManager.OnSetNameColorEvent += SetNameColorListener;

        _jumpCommandEventArgs = null;
        _disableGlowEventArgs = null;
        _characterChangeEventArgs = null;
        _glowColorChangeEventArgs = null;
        _nameColorChangeEventArgs = null;
    }

    [TearDown]
    public void TearDown()
    {
        _fakeJumper.JumperEventsManager.OnJumpCommandEvent -= JumpCommandListener;
        _fakeJumper.JumperEventsManager.OnDisableGlowEvent -= DisableGlowListener;
        _fakeJumper.JumperEventsManager.OnSetCharacterEvent -= SetCharacterListener;
        _fakeJumper.JumperEventsManager.OnSetGlowColorEvent -= SetGlowColorListener;
        _fakeJumper.JumperEventsManager.OnSetNameColorEvent -= SetNameColorListener;
    }

    /// <summary>
    /// This test makes sure that we can get correct values for executed jump command in the arguments that were sent by
    /// the JumpCommand by Jumper's event invoker.
    /// </summary>
    [Test]
    public void CanListenToJumpCommandEvents()
    {
        JumpCommand jumpCommand = new("j", 20, 80);

        _fakeJumper.JumperEventsManager.InvokeEvent(
            JumperEventTypes.OnJumpCommandEvent,
            new JumpCommandEventArgs(jumpCommand)
        );

        Assert.Multiple(() =>
        {
            Assert.That(_jumpCommandEventArgs, Is.Not.Null);
            Assert.That(_jumpCommandEventArgs?.JumpAngle, Is.EqualTo(110));
            Assert.That(_jumpCommandEventArgs?.JumpPower, Is.EqualTo(80));
        });
    }

    /// <summary>
    /// This test just makes sure that we have correctly invoked the Disable Glow event from the Jumper.
    /// </summary>
    [Test]
    public void CanListenToDisableGlowEvents()
    {
        _fakeJumper.JumperEventsManager.InvokeEvent(JumperEventTypes.OnDisableGlow, new DisableGlowEventArgs());

        Assert.That(_disableGlowEventArgs, Is.Not.Null);
    }

    /// <summary>
    /// This test makes sure that we get the correct character choice after invoking the Character Change event.
    /// </summary>
    [Test]
    public void CanListenToSetCharacterEvents()
    {
        _fakeJumper.JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetCharacter, new SetCharacterEventArgs(9));

        Assert.Multiple(() =>
        {
            Assert.That(_characterChangeEventArgs, Is.Not.Null);
            Assert.That(_characterChangeEventArgs?.UserCharacterChoice, Is.EqualTo(9));
        });
    }

    /// <summary>
    /// This test makes sure that we get the correct color chosen by the user after invoking the Glow Color event.
    /// </summary>
    [Test]
    public void CanListenToGlowColorEvents()
    {
        _fakeJumper.JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetGlowColor, new SetGlowColorEventArgs("DDD"));

        Assert.Multiple(() =>
        {
            Assert.That(_glowColorChangeEventArgs, Is.Not.Null);
            Assert.That(_glowColorChangeEventArgs?.UserColorChoice, Is.EqualTo("DDD"));
        });
    }

    /// <summary>
    /// This test makes sure that we get the correct color chosen by the user after invoking the Name Color Event.
    /// </summary>
    [Test]
    public void CanListenToNameColorEvents()
    {
        _fakeJumper.JumperEventsManager.InvokeEvent(JumperEventTypes.OnSetNameColor, new SetNameColorEventArgs("EEE"));

        Assert.Multiple(() =>
        {
            Assert.That(_nameColorChangeEventArgs, Is.Not.Null);
            Assert.That(_nameColorChangeEventArgs?.UserColorChoice, Is.EqualTo("EEE"));
        });
    }

    private void JumpCommandListener(object sender, JumpCommandEventArgs args)
    {
        _jumpCommandEventArgs = args;
    }

    private void DisableGlowListener(object sender, DisableGlowEventArgs args)
    {
        _disableGlowEventArgs = args;
    }

    private void SetCharacterListener(object sender, SetCharacterEventArgs args)
    {
        _characterChangeEventArgs = args;
    }

    private void SetGlowColorListener(object sender, SetGlowColorEventArgs args)
    {
        _glowColorChangeEventArgs = args;
    }

    private void SetNameColorListener(object sender, SetNameColorEventArgs args)
    {
        _nameColorChangeEventArgs = args;
    }
}
