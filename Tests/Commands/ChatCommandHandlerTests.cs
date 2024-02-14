using JumpRoyale.Commands;
using JumpRoyale.Utils;

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
        foreach (string command in ChatCommandMatcher.AvailableCommands)
        {
            ChatCommandHandler commandHandler = new(command, "12345", "fakeUser", "ffffff", true);

            GenericActionHandler<object>? matchedCommand = commandHandler.TryGetCommandFromChatMessage();

            Assert.That(matchedCommand is not null && commandHandler.ExecutedCommand.Name == command);
        }
    }
}
