using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: May 5th 2019
 */
public class ItemData
{
    private static protected string RESTORE_ITEMS = "SELECT * FROM items";
    private static protected Dictionary<int, ItemTemplateHolder> ITEMS = new Dictionary<int, ItemTemplateHolder>();

    public static void Load()
    {
        Util.PrintSection("Items");

        ITEMS.Clear();
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(RESTORE_ITEMS, con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                int itemId = reader.GetInt32("item_id");
                int skillId = reader.GetInt32("skill_id");
                int skillLevel = reader.GetInt32("skill_level");
                SkillHolder skillHolder = SkillData.GetSkillHolder(skillId, skillLevel);
                if ((skillId > 0) && (skillHolder == null))
                {
                    LogManager.Log("ItemData: Could not find skill with id " + skillId + " and level " + skillLevel + " for item " + itemId + ".");
                }
                else
                {
                    ITEMS.Add(itemId, new ItemTemplateHolder(itemId, (ItemSlot)Enum.Parse(typeof(ItemSlot), reader.GetString("slot")), (ItemType)Enum.Parse(typeof(ItemType), reader.GetString("type")), reader.GetBoolean("stackable"), reader.GetBoolean("tradable"), reader.GetInt32("stamina"), reader.GetInt32("strength"), reader.GetInt32("dexterity"), reader.GetInt32("intelect"), skillHolder));
                }
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        LogManager.Log("ItemData: Loaded " + ITEMS.Count + " items.");
    }

    public static ItemTemplateHolder GetItemTemplate(int itemId)
    {
        return ITEMS[itemId];
    }
}
