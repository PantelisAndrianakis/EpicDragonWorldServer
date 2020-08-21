using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class CharacterDeletionRequest
{
    private static readonly string ACCOUNT_CHARACTER_QUERY = "SELECT * FROM characters WHERE account=@account ORDER BY slot ASC";
    private static readonly string CHARACTER_SLOT_UPDATE_QUERY = "UPDATE characters SET slot=@slot, selected=@selected WHERE account=@account AND name=@name";
    private static readonly string CHARACTER_DELETION_QUERY = "UPDATE characters SET name=@name, access_level='-1' WHERE account=@account AND name=@oldname";
    private static readonly string CHARACTER_ITEM_DELETION_QUERY = "UPDATE character_items SET owner=@owner WHERE owner=@oldowner";
    private static readonly string CHARACTER_INTERFACE_DELETION_QUERY = "UPDATE character_options SET name=@name WHERE name=@oldname";

    public CharacterDeletionRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        byte slot = (byte)packet.ReadByte();

        // Get remaining character names.
        List<string> characterNames = new List<string>();
        string deletedCharacterName = "";
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(ACCOUNT_CHARACTER_QUERY, con);
            cmd.Parameters.AddWithValue("account", client.GetAccountName());
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if ((byte)reader.GetInt16("slot") == slot)  // TODO: Remove cast?
                {
                    deletedCharacterName = reader.GetString("name");
                }
                else
                {
                    characterNames.Add(reader.GetString("name"));
                }
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }


        // Delete character. (Prefer to set access level to -1 and rename to name + deletion time.)
        long deleteTime = DateTimeOffset.Now.ToUnixTimeSeconds();
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_DELETION_QUERY, con);
            cmd.Parameters.AddWithValue("name", deletedCharacterName + deleteTime);
            cmd.Parameters.AddWithValue("account", client.GetAccountName());
            cmd.Parameters.AddWithValue("oldname", deletedCharacterName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Delete character items. (Same as above, change item owner to name + deletion time.)
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_ITEM_DELETION_QUERY, con);
            cmd.Parameters.AddWithValue("owner", deletedCharacterName + deleteTime);
            cmd.Parameters.AddWithValue("oldowner", deletedCharacterName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Delete character interface. (Same as above, change interface name to name + deletion time.)
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_INTERFACE_DELETION_QUERY, con);
            cmd.Parameters.AddWithValue("name", deletedCharacterName + deleteTime);
            cmd.Parameters.AddWithValue("oldname", deletedCharacterName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Order remaining character slots.
        for (int i = 0; i < characterNames.Count; i++)
        {
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(CHARACTER_SLOT_UPDATE_QUERY, con);
                cmd.Parameters.AddWithValue("slot", i);
                cmd.Parameters.AddWithValue("selected", ((i == 1) && (slot == 1)) || (i == (slot - 1)) ? 1 : 0);
                cmd.Parameters.AddWithValue("account", client.GetAccountName());
                cmd.Parameters.AddWithValue("name", characterNames[i]);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }
        }

        // Notify the client that character was deleted.
        client.ChannelSend(new CharacterDeletionResult());
    }
}
