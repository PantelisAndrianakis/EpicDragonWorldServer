using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class CharacterSelectionInfoResult : SendablePacket
{
    private static readonly string CHARACTER_QUERY = "SELECT * FROM characters WHERE account=@account AND access_level>'-1' ORDER BY slot ASC";
    private static readonly string VISIBLE_ITEMS_QUERY = "SELECT * FROM character_items WHERE owner=@owner AND slot_id>'0' AND slot_id<'8' ORDER BY slot_id ASC"; // Visible equipment slots are 1 to 7.

    public CharacterSelectionInfoResult(string accountName)
    {
        // Local data.
        List<CharacterDataHolder> characterList = new List<CharacterDataHolder>();

        // Get data from database.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(CHARACTER_QUERY, con);
            cmd.Parameters.AddWithValue("account", accountName);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CharacterDataHolder characterData = new CharacterDataHolder();
                string name = reader.GetString("name");
                characterData.SetName(name);
                characterData.SetSlot((byte)reader.GetInt16("slot")); // TODO: Remove cast?
                characterData.SetSelected(reader.GetBoolean("selected"));
                characterData.SetRace((byte)reader.GetInt16("race")); // TODO: Remove cast?
                characterData.SetHeight((float)reader.GetDouble("height")); // Fixes known MySQL float issue.
                characterData.SetBelly((float)reader.GetDouble("belly")); // Fixes known MySQL float issue.
                characterData.SetHairType((byte)reader.GetInt16("hair_type")); // TODO: Remove cast?
                characterData.SetHairColor(reader.GetInt32("hair_color"));
                characterData.SetSkinColor(reader.GetInt32("skin_color"));
                characterData.SetEyeColor(reader.GetInt32("eye_color"));
                characterData.SetX((float)reader.GetDouble("x")); // Fixes known MySQL float issue.
                characterData.SetY((float)reader.GetDouble("y")); // Fixes known MySQL float issue.
                characterData.SetZ((float)reader.GetDouble("z")); // Fixes known MySQL float issue.
                characterData.SetHeading((float)reader.GetDouble("heading")); // Fixes known MySQL float issue.
                characterData.SetExperience(reader.GetInt64("experience"));
                characterData.SetHp(reader.GetInt64("hp"));
                characterData.SetMp(reader.GetInt64("mp"));
                characterData.SetAccessLevel((byte)reader.GetInt16("access_level")); // TODO: Remove cast?

                // Also get items for this character.
                try
                {
                    MySqlConnection con2 = DatabaseManager.GetConnection();
                    MySqlCommand cmd2 = new MySqlCommand(VISIBLE_ITEMS_QUERY, con2);
                    cmd2.Parameters.AddWithValue("owner", name);
                    MySqlDataReader reader2 = cmd2.ExecuteReader();
                    while (reader2.Read())
                    {
                        int slotId = reader2.GetInt32("slot_id");
                        int itemId = reader2.GetInt32("item_id");
                        switch (slotId)
                        {
                            case 1:
                                characterData.SetHeadItem(itemId);
                                break;

                            case 2:
                                characterData.SetChestItem(itemId);
                                break;

                            case 3:
                                characterData.SetLegsItem(itemId);
                                break;

                            case 4:
                                characterData.SetHandsItem(itemId);
                                break;

                            case 5:
                                characterData.SetFeetItem(itemId);
                                break;

                            case 6:
                                characterData.SetLeftHandItem(itemId);
                                break;

                            case 7:
                                characterData.SetRightHandItem(itemId);
                                break;
                        }
                    }
                    con2.Close();
                }
                catch (Exception e)
                {
                    LogManager.Log(e.ToString());
                }

                // Add gathered data to character list.
                characterList.Add(characterData);
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Send the data.
        WriteShort(2); // Packet id.
        WriteByte(characterList.Count);
        for (int i = 0; i < characterList.Count; i++)
        {
            CharacterDataHolder characterData = characterList[i];
            WriteString(characterData.GetName());
            WriteByte(characterData.GetSlot());
            WriteByte(characterData.IsSelected() ? 1 : 0);
            WriteByte(characterData.GetRace());
            WriteFloat(characterData.GetHeight());
            WriteFloat(characterData.GetBelly());
            WriteByte(characterData.GetHairType());
            WriteInt(characterData.GetHairColor());
            WriteInt(characterData.GetSkinColor());
            WriteInt(characterData.GetEyeColor());
            WriteInt(characterData.GetHeadItem());
            WriteInt(characterData.GetChestItem());
            WriteInt(characterData.GetLegsItem());
            WriteInt(characterData.GetHandsItem());
            WriteInt(characterData.GetFeetItem());
            WriteInt(characterData.GetLeftHandItem());
            WriteInt(characterData.GetRightHandItem());
            WriteFloat(characterData.GetX());
            WriteFloat(characterData.GetY());
            WriteFloat(characterData.GetZ());
            WriteFloat(characterData.GetHeading());
            WriteLong(characterData.GetExperience());
            WriteLong(characterData.GetHp());
            WriteLong(100); // TODO: Implement Player level data. - Max HP
            WriteLong(characterData.GetMp());
            WriteLong(100); // TODO: Implement Player level data. - Max MP
            WriteByte(characterData.GetAccessLevel());
        }
    }
}
