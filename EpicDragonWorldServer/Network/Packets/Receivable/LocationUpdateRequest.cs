using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class LocationUpdateRequest
{
    private readonly static int MAX_MOVE_DISTANCE = 300;

    public LocationUpdateRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        float posX = packet.ReadFloat();
        float posY = packet.ReadFloat();
        float posZ = packet.ReadFloat();
        float heading = packet.ReadFloat();

        // Get player.
        Player player = client.GetActiveChar();

        // Check if player is outside of world bounds.
        if (posX < Config.WORLD_MINIMUM_X || posX > Config.WORLD_MAXIMUM_X || posY < Config.WORLD_MINIMUM_Y || posY > Config.WORLD_MAXIMUM_Y || posZ < Config.WORLD_MINIMUM_Z || posZ > Config.WORLD_MAXIMUM_Z)
        {
            player.SetLocation(Config.STARTING_LOCATION);
            client.ChannelSend(new Logout());
            return;
        }

        // Check if player moved too far away via probable exploit.
        if (!player.IsTeleporting() && player.CalculateDistance(posX, posY, posZ) > MAX_MOVE_DISTANCE)
        {
            player.SetLocation(Config.STARTING_LOCATION);
            client.ChannelSend(new Logout());
            return;
        }

        // Update player location.
        player.SetLocation(new LocationHolder(posX, posY, posZ, heading));

        // Broadcast movement.
        LocationUpdate locationUpdate = new LocationUpdate(player);
        List<Player> players = WorldManager.GetVisiblePlayers(player);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ChannelSend(locationUpdate);
        }
    }
}
