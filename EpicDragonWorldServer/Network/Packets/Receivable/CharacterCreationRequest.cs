using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class CharacterCreationRequest
{
    private static readonly string ACCOUNT_CHARACTER_QUERY = "SELECT * FROM characters WHERE access_level>'-1' AND account=@account";
    private static readonly string NAME_EXISTS_QUERY = "SELECT * FROM characters WHERE name=@name";
    private static readonly string CHARACTER_SELECTED_RESET_QUERY = "UPDATE characters SET selected=0 WHERE account=@account";
    private static readonly string CHARACTER_CREATE_QUERY = "INSERT INTO characters (account, name, slot, selected, race, height, belly, hair_type, hair_color, skin_color, eye_color, x, y, z, heading, experience, hp, mp) values (@account, @name, @slot, @selected, @race, @height, @belly, @hair_type, @hair_color, @skin_color, @eye_color, @x, @y, @z, @heading, @experience, @hp, @mp)";
    private static readonly string CHARACTER_CREATE_OPTIONS_QUERY = "INSERT INTO character_options (name) values (@name)";
    private static readonly string CHARACTER_ITEM_START = "INSERT INTO character_items VALUES ";

    private static readonly int INVALID_NAME = 0;
    private static readonly int NAME_IS_TOO_SHORT = 1;
    private static readonly int NAME_ALREADY_EXISTS = 2;
    private static readonly int CANNOT_CREATE_ADDITIONAL_CHARACTERS = 3;
    private static readonly int INVALID_PARAMETERS = 4;
    private static readonly int SUCCESS = 100;

    public CharacterCreationRequest(GameClient client, ReceivablePacket packet)
    {
        // Make sure player has authenticated.
        if ((client.GetAccountName() == null) || (client.GetAccountName().Length == 0))
        {
            return;
        }

        // Read data.
        string characterName = packet.ReadString();
        int race = packet.ReadByte();
        float height = packet.ReadFloat();
        float belly = packet.ReadFloat();
        int hairType = packet.ReadByte();
        int hairColor = packet.ReadInt();
        int skinColor = packet.ReadInt();
        int eyeColor = packet.ReadInt();

        // Replace illegal characters.
        foreach (char c in Util.ILLEGAL_CHARACTERS)
        {
            characterName = characterName.Replace(c, '\'');
        }

        // Name character checks.
        if (characterName.Contains("'"))
        {
            client.ChannelSend(new CharacterCreationResult(INVALID_NAME));
            return;
        }
        if ((characterName.Length < 2) || (characterName.Length > 12)) // 12 should not happen, checking it here in case of client cheat.
        {
            client.ChannelSend(new CharacterCreationResult(NAME_IS_TOO_SHORT));
            return;
        }
        // Visual exploit checks.
        if ((race < 0 || race > 1)
            || (height < 0.39 || height > 0.61)
            || (hairType < 0 || hairType > 3)
            /*|| (!Config.VALID_SKIN_COLORS.Contains(skinColor))*/) // TODO: Check palette.
        {
            client.ChannelSend(new CharacterCreationResult(INVALID_PARAMETERS));
            return;
        }

        // Account character count database check.
        int characterCount = 0;
        int lastCharacterSlot = 0;
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(ACCOUNT_CHARACTER_QUERY, con);
            cmd.Parameters.AddWithValue("account", client.GetAccountName());
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                characterCount++;
                int slot = reader.GetInt32("slot");
                if (slot > lastCharacterSlot)
                {
                    lastCharacterSlot = slot;
                }
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }
        if (characterCount >= Config.ACCOUNT_MAX_CHARACTERS)
        {
            client.ChannelSend(new CharacterCreationResult(CANNOT_CREATE_ADDITIONAL_CHARACTERS));
            return;
        }

        // Check database if name exists.
        bool characterExists = false;
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(NAME_EXISTS_QUERY, con);
            {
                cmd.Parameters.AddWithValue("name", characterName);
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    characterExists = true;
                }
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }
        if (characterExists)
        {
            client.ChannelSend(new CharacterCreationResult(NAME_ALREADY_EXISTS));
            return;
        }

        // Make existing characters selected value false.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_SELECTED_RESET_QUERY, con);
            cmd.Parameters.AddWithValue("account", client.GetAccountName());
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Create character.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_CREATE_QUERY, con);
            cmd.Parameters.AddWithValue("account", client.GetAccountName());
            cmd.Parameters.AddWithValue("name", characterName);
            cmd.Parameters.AddWithValue("slot", lastCharacterSlot + 1);
            cmd.Parameters.AddWithValue("selected", 1); // Selected character.
            cmd.Parameters.AddWithValue("race", race);
            cmd.Parameters.AddWithValue("height", height);
            cmd.Parameters.AddWithValue("belly", belly);
            cmd.Parameters.AddWithValue("hair_type", hairType);
            cmd.Parameters.AddWithValue("hair_color", hairColor);
            cmd.Parameters.AddWithValue("skin_color", skinColor);
            cmd.Parameters.AddWithValue("eye_color", eyeColor);
            cmd.Parameters.AddWithValue("x", Config.STARTING_LOCATION.GetX());
            cmd.Parameters.AddWithValue("y", Config.STARTING_LOCATION.GetY());
            cmd.Parameters.AddWithValue("z", Config.STARTING_LOCATION.GetZ());
            cmd.Parameters.AddWithValue("heading", Config.STARTING_LOCATION.GetHeading());
            cmd.Parameters.AddWithValue("experience", 0); // TODO: Implement Player level data.
            cmd.Parameters.AddWithValue("hp", 100); // TODO: Implement Player level data.
            cmd.Parameters.AddWithValue("mp", 100); // TODO: Implement Player level data.
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Create a character_options entry for this character.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_CREATE_OPTIONS_QUERY, con);
            cmd.Parameters.AddWithValue("name", characterName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Add starting items.
        int itemCount = Config.STARTING_ITEMS.Count;
        if (itemCount > 0)
        {
            // Prepare query.
            StringBuilder query = new StringBuilder(CHARACTER_ITEM_START);
            List<ItemSlot> usedEquipableSlots = new List<ItemSlot>();
            int inventorySlotCounter = 8; // First inventory item slot.
            foreach (int itemId in Config.STARTING_ITEMS)
            {
                query.Append("('");
                query.Append(characterName);
                query.Append("',");
                ItemTemplateHolder itemHolder = ItemData.GetItemTemplate(itemId);
                ItemSlot itemSlot = itemHolder.GetItemSlot();
                if (itemHolder.GetItemType() == ItemType.EQUIP && !usedEquipableSlots.Contains(itemSlot))
                {
                    usedEquipableSlots.Add(itemSlot);
                    query.Append((int)itemHolder.GetItemSlot());
                }
                else
                {
                    query.Append(inventorySlotCounter++);
                }
                query.Append(",");
                query.Append(itemId);
                query.Append(",1,0"); // quantity, enchant
                query.Append(")");
                query.Append(itemCount-- == 1 ? ";" : ",");
            }
            // Store new item records.
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(query.ToString(), con);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }
        }

        // Send success result.
        client.ChannelSend(new CharacterCreationResult(SUCCESS));
    }
}
