/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class PlayerInformation : SendablePacket
{
    public PlayerInformation(Player player)
    {
        // Packet id.
        WriteShort(6);
        // Player information.
        WriteLong(player.GetObjectId());
        WriteString(player.GetName());
        WriteByte(player.GetRaceId());
        WriteFloat(player.GetHeight());
        WriteFloat(player.GetBelly());
        WriteByte(player.GetHairType());
        WriteInt(player.GetHairColor());
        WriteInt(player.GetSkinColor());
        WriteInt(player.GetEyeColor());
        WriteInt(player.GetInventory().GetItemIdBySlot(1)); // Head
        WriteInt(player.GetInventory().GetItemIdBySlot(2)); // Chest
        WriteInt(player.GetInventory().GetItemIdBySlot(3)); // Legs
        WriteInt(player.GetInventory().GetItemIdBySlot(4)); // Hands
        WriteInt(player.GetInventory().GetItemIdBySlot(5)); // Feet
        WriteInt(player.GetInventory().GetItemIdBySlot(6)); // Left hand
        WriteInt(player.GetInventory().GetItemIdBySlot(7)); // Right hand
        WriteFloat(player.GetLocation().GetX());
        WriteFloat(player.GetLocation().GetY());
        WriteFloat(player.GetLocation().GetZ());
        WriteFloat(player.GetLocation().GetHeading());
        WriteLong(player.GetCurrentHp());
        WriteLong(player.GetMaxHp());
    }
}
