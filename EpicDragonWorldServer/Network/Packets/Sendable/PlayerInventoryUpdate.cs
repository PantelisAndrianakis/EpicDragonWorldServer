using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: March 12th 2020
 */
public class PlayerInventoryUpdate : SendablePacket
{
    public PlayerInventoryUpdate(Player player)
    {
        WriteShort(13); // Packet id.

        // Get the item list.
        Dictionary<int, ItemHolder> items = player.GetInventory().GetItems();

        // Write information.
        WriteInt(items.Count);
        foreach (KeyValuePair<int, ItemHolder> item in items)
        {
            WriteInt(item.Value.GetTemplate().GetItemId()); // Item id.
            WriteInt(item.Key); // Slot.
            WriteInt(item.Value.GetQuantity()); // Quantity.
            WriteInt(item.Value.GetEnchant()); // Enchant.
        }
    }
}
