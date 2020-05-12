using System.Threading.Tasks;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class EnterWorldRequest
{
    public EnterWorldRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        string characterName = packet.ReadString();

        // Create a new PlayerInstance.
        Player player = new Player(client, characterName);
        // Add object to the world.
        WorldManager.AddObject(player);
        // Assign this player to client.
        client.SetActiveChar(player);

        // Send user interface information to client.
        client.ChannelSend(new PlayerOptionsInformation(player));

        // Send all inventory items to client.
        client.ChannelSend(new PlayerInventoryUpdate(player));
        
        // Use a task to send and receive nearby player information,
        // because we need to have player initialization be complete in client side.
        Task.Delay(1000).ContinueWith(task => BroadcastAndReceiveInfo(player));
    }

    private void BroadcastAndReceiveInfo(Player player)
    {
        // Send and receive visible object information.
        PlayerInformation playerInfo = new PlayerInformation(player);
        foreach (Player nearby in WorldManager.GetVisiblePlayers(player))
        {
            // Send the information to the current player.
            player.ChannelSend(new PlayerInformation(nearby));
            // Send information to the other player as well.
            nearby.ChannelSend(playerInfo);
        }

        // Send nearby NPC information.
        foreach (WorldObject nearby in WorldManager.GetVisibleObjects(player))
        {
            if (!nearby.IsNpc())
            {
                continue;
            }
            player.ChannelSend(new NpcInformation(nearby.AsNpc()));
        }
    }
}
