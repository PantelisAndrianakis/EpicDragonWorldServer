using System.Collections.Generic;
using System.Threading.Tasks;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class ObjectInfoRequest
{
    public ObjectInfoRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        long objectId = packet.ReadLong();

        // Get the acting player.
        Player player = client.GetActiveChar();
        // Send the information.
        List<WorldObject> objects = WorldManager.GetVisibleObjects(player);
        for (int i = 0; i < objects.Count; i++)
        {
            WorldObject obj = objects[i];
            if (obj.GetObjectId() == objectId)
            {
                if (obj.IsPlayer())
                {
                    client.ChannelSend(new PlayerInformation(obj.AsPlayer()));
                }
                else if (obj.IsNpc())
                {
                    client.ChannelSend(new NpcInformation(obj.AsNpc()));
                }

                // Send delayed animation update in case object was already moving.
                Task.Delay(1000).ContinueWith(task => SendAnimationInfo(client, obj));
                break;
            }
        }
    }

    private void SendAnimationInfo(GameClient client, WorldObject obj)
    {
        if (obj != null)
        {
            AnimationHolder animations = obj.GetAnimations();
            if (animations != null)
            {
                client.ChannelSend(new AnimatorUpdate(obj.GetObjectId(), animations.GetVelocityX(), animations.GetVelocityZ(), animations.IsTriggerJump(), animations.IsInWater(), animations.IsGrounded()));
            }
        }
    }
}
