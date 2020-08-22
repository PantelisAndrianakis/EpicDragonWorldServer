using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 29th 2019
 */
public class LocCommand
{
    public static void Handle(Player player)
    {
        LocationHolder location = player.GetLocation();

        // Send player success message.
        StringBuilder sb = new StringBuilder();
        sb.Append("Your location is ");
        sb.Append(location.GetX());
        sb.Append(" ");
        sb.Append(location.GetZ());
        sb.Append(" ");
        sb.Append(location.GetY());
        ChatManager.SendSystemMessage(player, sb.ToString());
    }
}
