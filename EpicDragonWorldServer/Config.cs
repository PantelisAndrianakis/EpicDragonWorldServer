using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class Config
{
    // --------------------------------------------------
    // Config File Definitions
    // --------------------------------------------------
    private static readonly string ACCOUNT_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Account.ini";
    private static readonly string DATABASE_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Database.ini";
    private static readonly string LOGGING_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Logging.ini";
    private static readonly string NETWORK_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Network.ini";
    private static readonly string PLAYER_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "Player.ini";
    private static readonly string WORLD_CONFIG_FILE = "Config" + Path.DirectorySeparatorChar + "World.ini";

    // --------------------------------------------------
    // Account
    // --------------------------------------------------
    public static bool ACCOUNT_AUTO_CREATE;
    public static int ACCOUNT_MAX_CHARACTERS;

    // --------------------------------------------------
    // Database
    // --------------------------------------------------
    public static string DATABASE_CONNECTION_PARAMETERS;
    public static int DATABASE_MAX_CONNECTIONS;

    // --------------------------------------------------
    // Logging
    // --------------------------------------------------
    public static bool LOG_FILE_SIZE_LIMIT_ENABLED = false; // Must be set before configs load.
    public static long LOG_FILE_SIZE_LIMIT = 1073741824; // Must be set before configs load.
    public static bool LOG_CHAT;
    public static bool LOG_WORLD;
    public static bool LOG_ADMIN;

    // --------------------------------------------------
    // Network
    // --------------------------------------------------
    public static int SERVER_PORT;
    public static int MAXIMUM_ONLINE_USERS;
    public static double CLIENT_VERSION;
    public static byte[] ENCRYPTION_SECRET_KEYWORD;

    // --------------------------------------------------
    // Player
    // --------------------------------------------------
    public static LocationHolder STARTING_LOCATION;
    public static List<int> STARTING_ITEMS = new List<int>();
    public static List<int> VALID_SKIN_COLORS = new List<int>();

    // --------------------------------------------------
    // World
    // --------------------------------------------------
    public static float WORLD_MINIMUM_X;
    public static float WORLD_MAXIMUM_X;
    public static float WORLD_MINIMUM_Y;
    public static float WORLD_MAXIMUM_Y;
    public static float WORLD_MINIMUM_Z;
    public static float WORLD_MAXIMUM_Z;

    public static void Load()
    {
        Util.PrintSection("Configs");

        // Initialize first so we can use logs as configured.
        ConfigReader loggingConfigs = new ConfigReader(LOGGING_CONFIG_FILE);
        LOG_FILE_SIZE_LIMIT_ENABLED = loggingConfigs.GetBool("LogFileSizeLimitEnabled", false);
        LOG_FILE_SIZE_LIMIT = loggingConfigs.GetLong("LogFileSizeLimit", 1073741824);
        LOG_CHAT = loggingConfigs.GetBool("LogChat", true);
        LOG_WORLD = loggingConfigs.GetBool("LogWorld", true);
        LOG_ADMIN = loggingConfigs.GetBool("LogAdmin", true);

        ConfigReader accountConfigs = new ConfigReader(ACCOUNT_CONFIG_FILE);
        ACCOUNT_AUTO_CREATE = accountConfigs.GetBool("AccountAutoCreate", false);
        ACCOUNT_MAX_CHARACTERS = accountConfigs.GetInt("AccountMaxCharacters", 5);

        ConfigReader databaseConfigs = new ConfigReader(DATABASE_CONFIG_FILE);
        DATABASE_CONNECTION_PARAMETERS = databaseConfigs.GetString("DbConnectionParameters", "Server=127.0.0.1;User ID=root;Password=;Database=edws");
        DATABASE_MAX_CONNECTIONS = databaseConfigs.GetInt("MaximumDbConnections", 50);

        ConfigReader networkConfigs = new ConfigReader(NETWORK_CONFIG_FILE);
        SERVER_PORT = networkConfigs.GetInt("ServerPort", 5055);
        MAXIMUM_ONLINE_USERS = networkConfigs.GetInt("MaximumOnlineUsers", 2000);
        CLIENT_VERSION = networkConfigs.GetDouble("ClientVersion", 1.0);
        ENCRYPTION_SECRET_KEYWORD = new MD5CryptoServiceProvider().ComputeHash(Encoding.UTF8.GetBytes(networkConfigs.GetString("SecretKeyword", "SECRET_KEYWORD")));

        ConfigReader playerConfigs = new ConfigReader(PLAYER_CONFIG_FILE);
        string[] startingLocation = playerConfigs.GetString("StartingLocation", "3924.109;67.42678;2329.238").Split(";");
        STARTING_LOCATION = new LocationHolder(float.Parse(startingLocation[0], CultureInfo.InvariantCulture), float.Parse(startingLocation[1], CultureInfo.InvariantCulture), float.Parse(startingLocation[2], CultureInfo.InvariantCulture), startingLocation.Length > 3 ? float.Parse(startingLocation[3], CultureInfo.InvariantCulture) : 0);
        STARTING_ITEMS.Clear();
        foreach (string itemString in playerConfigs.GetString("StartingItems", "").Split(";"))
        {
            string trimmedString = itemString.Trim();
            if (!trimmedString.Equals(""))
            {
                int itemId = int.Parse(trimmedString);
                if (itemId > 0)
                {
                    STARTING_ITEMS.Add(itemId);
                }
            }
        }
        VALID_SKIN_COLORS.Clear();
        foreach (string colorCode in playerConfigs.GetString("ValidSkinColorCodes", "F1D1BD;F1C4AD;E7B79C;E19F7E;AF7152;7E472E;4A2410;F7DDC0;F3D1A9;C5775A;B55B44;863923;672818;3F1508").Split(";"))
        {
            VALID_SKIN_COLORS.Add(Util.HexStringToInt(colorCode));
        }

        ConfigReader worldConfigs = new ConfigReader(WORLD_CONFIG_FILE);
        WORLD_MINIMUM_X = worldConfigs.GetFloat("MinimumX", -558.8f);
        WORLD_MAXIMUM_X = worldConfigs.GetFloat("MaximumX", 4441.2f);
        WORLD_MINIMUM_Y = worldConfigs.GetFloat("MinimumY", -100f);
        WORLD_MAXIMUM_Y = worldConfigs.GetFloat("MaximumY", 1000f);
        WORLD_MINIMUM_Z = worldConfigs.GetFloat("MinimumZ", -1445.3f);
        WORLD_MAXIMUM_Z = worldConfigs.GetFloat("MaximumZ", 3554.7f);

        LogManager.Log("Configs: Initialized.");
    }
}
