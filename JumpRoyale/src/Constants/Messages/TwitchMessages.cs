namespace Constants.Messages;

public static class TwitchMessages
{
    public const string OnChannelJoinMessage = "Successfully connected to the channel";

    public const string OnClientConnectedMessage = "Successfully connected to Twitch";

    /// <summary>
    /// Message string for Twitch reward redemption. Use <c>ReplaceInTemplate()</c> extension method to
    /// replace the template arguments.
    /// </summary>
    /// <remarks>
    /// This is a template string with the following arguments:
    /// <para>
    /// - <c>{0}</c>: user name.
    /// </para>
    /// <para>
    /// - <c>{1}</c>: reward id.
    /// </para>
    /// </remarks>
    public const string OnRewardRedeemMessage = "{0} redeemed a reward: {1}";

    public const string OnPubSubConnected = "Successfully connected to PubSub";
}