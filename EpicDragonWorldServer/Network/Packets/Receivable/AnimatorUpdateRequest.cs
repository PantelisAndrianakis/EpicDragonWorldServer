using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: February 4th 2019
 */
public class AnimatorUpdateRequest
{
    public AnimatorUpdateRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        float velocityX = packet.ReadFloat();
        float velocityZ = packet.ReadFloat();
        bool triggerJump = packet.ReadByte() == 1;
        bool isInWater = packet.ReadByte() == 1;
        bool isGrounded = packet.ReadByte() == 1;

        // Set last known world object animations.
        Player player = client.GetActiveChar();
        player.SetAnimations(new AnimationHolder(velocityX, velocityZ, triggerJump, isInWater, isGrounded));

        // Broadcast movement.
        AnimatorUpdate animatorUpdate = new AnimatorUpdate(player.GetObjectId(), velocityX, velocityZ, triggerJump, isInWater, isGrounded);
        List<Player> players = WorldManager.GetVisiblePlayers(player);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ChannelSend(animatorUpdate);
        }
    }
}

