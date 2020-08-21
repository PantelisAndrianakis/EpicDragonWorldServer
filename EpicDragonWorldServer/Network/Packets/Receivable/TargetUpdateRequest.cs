using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: November 29th 2019
 */
public class TargetUpdateRequest
{
    public TargetUpdateRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        long targetObjectId = packet.ReadLong();

        // Get player.
        Player player = client.GetActiveChar();

        // Remove target.
        if (targetObjectId < 0)
        {
            player.SetTarget(null);
            return;
        }

        // Find target WorldObject.
        List<WorldObject> objects = WorldManager.GetVisibleObjects(player);
        for (int i = 0; i < objects.Count; i++)
        {
            WorldObject obj = objects[i];
            if (obj != null && obj.GetObjectId() == targetObjectId)
            {
                player.SetTarget(obj);
                return;
            }
        }
    }
}
