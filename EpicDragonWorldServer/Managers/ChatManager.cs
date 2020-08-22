using System.Collections.Generic;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class ChatManager
{
    private static readonly byte CHAT_TYPE_SYSTEM = 0;
    private static readonly byte CHAT_TYPE_NORMAL = 1;
    private static readonly byte CHAT_TYPE_MESSAGE = 2;
    private static readonly string SYS_NAME = "System";
    private static readonly string MSG_TO = "To ";
    // Normal player commands
    private static readonly string COMMAND_PERSONAL_MESSAGE = "/tell ";
    private static readonly string COMMAND_LOCATION = "/loc";
    private static readonly string COMMAND_RETURN = "/return";
    // Administrator commands
    private static readonly string COMMAND_SPAWN = "/spawn ";
    private static readonly string COMMAND_DELETE = "/delete";

    public static void HandleChat(Player sender, string message)
    {
        // Check if message is empty.
        message = message.Trim();
        if (message.Length == 0)
        {
            return;
        }

        bool isAdmin = sender.GetAccessLevel() > 99;
        string lowercaseMessage = message.ToLowerInvariant().Replace("\\s{2,}", " "); // Also remove all double spaces.
        if (lowercaseMessage.Equals(COMMAND_LOCATION))
        {
            LocCommand.Handle(sender);
        }
        else if (lowercaseMessage.Equals(COMMAND_RETURN))
        {
            ReturnCommand.Handle(sender);
        }
        else if (lowercaseMessage.StartsWith(COMMAND_PERSONAL_MESSAGE))
        {
            TellCommand.Handle(sender, lowercaseMessage, message);
        }
        else if (isAdmin && lowercaseMessage.StartsWith(COMMAND_SPAWN))
        {
            SpawnCommand.Handle(sender, lowercaseMessage);
        }
        else if (isAdmin && lowercaseMessage.Equals(COMMAND_DELETE))
        {
            DeleteCommand.Handle(sender);
        }
        else // Normal message.
        {
            sender.ChannelSend(new ChatResult(CHAT_TYPE_NORMAL, sender.GetName(), message));
            List<Player> players = WorldManager.GetVisiblePlayers(sender);
            for (int i = 0; i < players.Count; i++)
            {
                players[i].ChannelSend(new ChatResult(CHAT_TYPE_NORMAL, sender.GetName(), message));
            }
            // Log chat.
            if (Config.LOG_CHAT)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[");
                sb.Append(sender.GetName());
                sb.Append("] ");
                sb.Append(message);
                LogManager.LogChat(sb.ToString());
            }
        }
    }

    public static void SendPrivateMessage(Player sender, Player receiver, string message)
    {
        sender.ChannelSend(new ChatResult(CHAT_TYPE_MESSAGE, MSG_TO + receiver.GetName(), message));
        receiver.ChannelSend(new ChatResult(CHAT_TYPE_MESSAGE, sender.GetName(), message));
    }

    public static void SendSystemMessage(Player player, string message)
    {
        player.ChannelSend(new ChatResult(CHAT_TYPE_SYSTEM, SYS_NAME, message));
    }
}
