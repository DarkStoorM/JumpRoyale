using JumpRoyale;
using JumpRoyale.Commands;
using JumpRoyale.Events;
using JumpRoyale.Utils;
using JumpRoyale.Utils.Exceptions;
using Tests.Mocks;

namespace Tests;

/// <summary>
/// Test fixture providing tests for cases that would typically be handled by the Jumper Scene, which subscribes to
/// events inside the event manager of specific Jumper instance and gets specific data this manager has passed to the
/// events.
/// </summary>
[TestFixture]
public class JumperEventTests
{
    private Jumper _fakeJumper;
    private Jumper _fakeHelperJumper;

    private JumpCommandEventArgs? _jumpCommandEventArgs;
    private DisableGlowEventArgs? _disableGlowEventArgs;
    private SetCharacterEventArgs? _characterChangeEventArgs;
    private SetGlowColorEventArgs? _glowColorChangeEventArgs;
    private SetNameColorEventArgs? _nameColorChangeEventArgs;

    [SetUp]
    public void SetUp()
    {
        _fakeJumper = new(FakePlayerData.Make());
        _fakeHelperJumper = new(FakePlayerData.Make());

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

    /// <summary>
    /// This test makes sure that when trying to raise an event and we specify a wrong instance of event args, the
    /// invoker throws an exception. The reason for this is that we want the event invoker to be generic, so we need to
    /// provide a matching event args instance to make sure we pass the correct object with parameters required by the
    /// invoked event.
    /// </summary>
    [Test]
    public void CanThrowExceptionOnMismatchedTypes()
    {
        Assert.Throws<MismatchedGenericEventArgsTypeException>(() =>
        {
            _fakeJumper.JumperEventsManager.InvokeEvent(
                JumperEventTypes.OnJumpCommandEvent,
                new DisableGlowEventArgs()
            );
        });
    }

    /// <summary>
    /// This test makes sure that the events won't be picked up by external objects subscribed to specific event
    /// manager. The purpose of this is to check if one Jumper Scene that has an instance of a Jumper will not receive
    /// an event invocation if another Jumper raises an event from a different Event Manager.
    /// </summary>
    [Test]
    public void EventManagersAreNotConfused()
    {
        _fakeHelperJumper.JumperEventsManager.OnJumpCommandEvent += HelperListenerOnJumpCommand;

        JumpCommand jumpCommand = new("l");

        Assert.DoesNotThrow(() =>
        {
            _fakeJumper.JumperEventsManager.InvokeEvent(
                JumperEventTypes.OnJumpCommandEvent,
                new JumpCommandEventArgs(jumpCommand)
            );
        });

        // Sanity check
        Assert.Throws<Exception>(() =>
        {
            _fakeHelperJumper.JumperEventsManager.InvokeEvent(
                JumperEventTypes.OnJumpCommandEvent,
                new JumpCommandEventArgs(jumpCommand)
            );
        });

        _fakeHelperJumper.JumperEventsManager.OnJumpCommandEvent -= HelperListenerOnJumpCommand;
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

    private void HelperListenerOnJumpCommand(object sender, JumpCommandEventArgs args)
    {
        throw new Exception("This should not happen :)");
    }
}
