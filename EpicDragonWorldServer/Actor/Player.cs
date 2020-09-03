using MySql.Data.MySqlClient;
using System;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class Player : Creature
{
    private static readonly string RESTORE_CHARACTER = "SELECT * FROM characters WHERE name=@name";
    private static readonly string STORE_CHARACTER = "UPDATE characters SET name=@name, race=@race, x=@x, y=@y, z=@z, heading=@heading, experience=@experience, hp=@hp, mp=@mp WHERE account=@account AND name=@name";

    private readonly GameClient _client;
    private readonly string _name;
    private readonly byte _raceId;
    private readonly float _height;
    private readonly float _belly;
    private readonly int _hairType;
    private readonly int _hairColor;
    private readonly int _skinColor;
    private readonly int _eyeColor;
    private readonly long _experience;
    private readonly byte _accessLevel;
    private readonly Inventory _inventory;

    public Player(GameClient client, string name)
    {
        _client = client;
        _name = name;

        // Load information from database.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(RESTORE_CHARACTER, con);
            cmd.Parameters.AddWithValue("name", name);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                _raceId = (byte)reader.GetInt16("race"); // TODO: Remove cast?
                _height = (float)reader.GetDouble("height"); // Fixes known MySQL float issue.
                _belly = (float)reader.GetDouble("belly"); // Fixes known MySQL float issue.
                _hairType = (byte)reader.GetInt16("hair_type"); // TODO: Remove cast?
                _hairColor = reader.GetInt32("hair_color");
                _skinColor = reader.GetInt32("skin_color");
                _eyeColor = reader.GetInt32("eye_color");

                float locX = (float)reader.GetDouble("x"); // Fixes known MySQL float issue.
                float locY = (float)reader.GetDouble("y"); // Fixes known MySQL float issue.
                float locZ = (float)reader.GetDouble("z"); // Fixes known MySQL float issue.

                // Check if player is outside of world bounds.
                if (locX < Config.WORLD_MINIMUM_X || locX > Config.WORLD_MAXIMUM_X || locY < Config.WORLD_MINIMUM_Y || locY > Config.WORLD_MAXIMUM_Y || locZ < Config.WORLD_MINIMUM_Z || locZ > Config.WORLD_MAXIMUM_Z)
                {
                    // Move to initial area.
                    SetLocation(Config.STARTING_LOCATION);
                }
                else
                {
                    SetLocation(new LocationHolder(locX, locY, locZ, (float)reader.GetDouble("heading"))); // Fixes known MySQL float issue.
                }

                _experience = reader.GetInt64("experience");
                SetCurrentHp(reader.GetInt64("hp"));
                SetCurrentMp(reader.GetInt64("mp"));
                _accessLevel = (byte)reader.GetInt16("access_level"); // TODO: Remove cast?
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Initialize inventory.
        _inventory = new Inventory(name);
    }

    public void StoreMe()
    {
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(STORE_CHARACTER, con);
            cmd.Parameters.AddWithValue("name", _name);
            cmd.Parameters.AddWithValue("race", _raceId);
            cmd.Parameters.AddWithValue("x", GetLocation().GetX());
            cmd.Parameters.AddWithValue("y", GetLocation().GetY());
            cmd.Parameters.AddWithValue("z", GetLocation().GetZ());
            cmd.Parameters.AddWithValue("heading", GetLocation().GetHeading());
            cmd.Parameters.AddWithValue("experience", _experience);
            cmd.Parameters.AddWithValue("hp", GetCurrentHp());
            cmd.Parameters.AddWithValue("mp", GetCurrentMp());
            cmd.Parameters.AddWithValue("account", _client.GetAccountName());
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // Save inventory.
        _inventory.Store(_name);
    }

    public GameClient GetClient()
    {
        return _client;
    }

    public string GetName()
    {
        return _name;
    }

    public int GetRaceId()
    {
        return _raceId;
    }

    public float GetHeight()
    {
        return _height;
    }

    public float GetBelly()
    {
        return _belly;
    }

    public int GetHairType()
    {
        return _hairType;
    }

    public int GetHairColor()
    {
        return _hairColor;
    }

    public int GetSkinColor()
    {
        return _skinColor;
    }

    public int GetEyeColor()
    {
        return _eyeColor;
    }

    public long GetExperience()
    {
        return _experience;
    }

    public byte GetAccessLevel()
    {
        return _accessLevel;
    }

    public Inventory GetInventory()
    {
        return _inventory;
    }

    public void ChannelSend(SendablePacket packet)
    {
        _client.ChannelSend(packet);
    }

    public override bool IsPlayer()
    {
        return true;
    }

    public override Player AsPlayer()
    {
        return this;
    }

    public override string ToString()
    {
        return "Player [" + _name + "]";
    }
}
