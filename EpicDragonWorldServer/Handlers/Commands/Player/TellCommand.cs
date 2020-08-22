using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 29th 2019
 */
public class TellCommand
{
    public static void Handle(Player sender, string lowercaseMessage, string message)
    {
        string[] lowercaseMessageSplit = lowercaseMessage.Split(" ");
        if (lowercaseMessageSplit.Length < 3) // Check for parameters.
        {
            ChatManager.SendSystemMessage(sender, "Incorrect syntax. Use /tell [name] [message].");
            return;
        }

        Player receiver = WorldManager.GetPlayerByName(lowercaseMessageSplit[1]);
        if (receiver == null)
        {
            ChatManager.SendSystemMessage(sender, "Player was not found.");
        }
        else
        {
            // Step by step cleanup, to avoid problems with extra/double spaces on original message.
            message = message.Substring(lowercaseMessageSplit[0].Length, message.Length - lowercaseMessageSplit[0].Length).Trim(); // Remove command.
            message = message.Substring(lowercaseMessageSplit[1].Length, message.Length - lowercaseMessageSplit[1].Length).Trim(); // Remove receiver name.
            // Send message.
            ChatManager.SendPrivateMessage(sender, receiver, message);
            // Log chat.
            if (Config.LOG_CHAT)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(sender.GetName());
                sb.Append("] to [");
                sb.Append(receiver.GetName());
                sb.Append("] ");
                sb.Append(message);
                LogManager.LogChat(sb.ToString());
            }
        }
    }
}
