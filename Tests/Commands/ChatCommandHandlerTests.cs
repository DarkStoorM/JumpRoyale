using JumpRoyale;
using JumpRoyale.Commands;
using JumpRoyale.Utils;
using Tests.Mocks;

namespace Tests.Commands;

[TestFixture]
public class CommandHandlerTests
{
    /// <summary>
    /// This test calls the CommandHandler directly as if it was coming from the raised event and goes through the
    /// entire command detection scope. This will test if the sent chat message was able to get the delegate in return
    /// for the requested command. Fails if no delegate was returned.
    /// </summary>
    [Test]
    public void CanExecuteAllAvailableCommands()
    {
        FakeTwitchChatter chatter = new(string.Empty);

        foreach (string command in ChatCommandMatcher.AvailableCommands)
        {
            ChatCommandHandler commandHandler =
                new(command, chatter.UserId, chatter.DisplayName, chatter.ColorHex, true);

            GenericActionHandler<Jumper>? matchedCommand = commandHandler.TryGetCommandFromChatMessage();

            Assert.That(matchedCommand is not null && commandHandler.ExecutedCommand.Name == command);
        }
    }

    /// <summary>
    /// The purpose of this test if to make sure the properties are correctly assigned in the command handler.
    /// </summary>
    [Test]
    public void HasCorrectDataAssigned()
    {
        FakeTwitchChatter chatter = new(string.Empty);
        ChatCommandHandler command = new("join", chatter.UserId, chatter.DisplayName, chatter.ColorHex, true);

        Assert.Multiple(() =>
        {
            Assert.That(command.ColorHex, Is.EqualTo(chatter.ColorHex));
            Assert.That(command.DisplayName, Is.EqualTo(chatter.DisplayName));
            Assert.That(command.IsPrivileged, Is.EqualTo(true));
            Assert.That(command.UserId, Is.EqualTo(chatter.UserId));
        });
    }
}
