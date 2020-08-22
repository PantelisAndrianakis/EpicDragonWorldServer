using System;
using System.Collections.Generic;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class WorldManager
{
    private static readonly int VISIBILITY_RADIUS = 10000;
    private static readonly int MOVEMENT_BROADCAST_RADIUS = VISIBILITY_RADIUS + 100; // Need the extra distance to send location of objects getting out of range.
    private static readonly double REGION_RADIUS = Math.Sqrt(VISIBILITY_RADIUS);
    private static readonly int REGION_SIZE_X = (int)(Config.WORLD_MAXIMUM_X / REGION_RADIUS);
    private static readonly int REGION_SIZE_Z = (int)(Config.WORLD_MAXIMUM_Z / REGION_RADIUS);
    private static readonly RegionHolder[][] REGIONS = new RegionHolder[REGION_SIZE_X][];
    private static readonly List<GameClient> ONLINE_CLIENTS = new List<GameClient>();
    private static readonly List<Player> PLAYER_OBJECTS = new List<Player>();

    public static void Init()
    {
        Util.PrintSection("World");

        // Initialize regions.
        for (int x = 0; x < REGION_SIZE_X; x++)
        {
            REGIONS[x] = new RegionHolder[REGION_SIZE_Z];
            for (int z = 0; z < REGION_SIZE_Z; z++)
            {
                REGIONS[x][z] = new RegionHolder(x, z);
            }
        }

        // Set surrounding regions.
        for (int x = 0; x < REGION_SIZE_X; x++)
        {
            for (int z = 0; z < REGION_SIZE_Z; z++)
            {
                List<RegionHolder> surroundingRegions = new List<RegionHolder>(9);
                for (int sx = x - 1; sx <= (x + 1); sx++)
                {
                    for (int sz = z - 1; sz <= (z + 1); sz++)
                    {
                        if (((sx >= 0) && (sx < REGION_SIZE_X) && (sz >= 0) && (sz < REGION_SIZE_Z)))
                        {
                            surroundingRegions.Add(REGIONS[sx][sz]);
                        }
                    }
                }
                REGIONS[x][z].SetSurroundingRegions(surroundingRegions);
            }
        }

        LogManager.Log("WorldManager: Initialized " + REGION_SIZE_X + " by " + REGION_SIZE_Z + " regions.");
    }

    public static RegionHolder GetRegion(WorldObject obj)
    {
        int x = (int)(obj.GetLocation().GetX() / REGION_RADIUS);
        int z = (int)(obj.GetLocation().GetZ() / REGION_RADIUS);
        if (x < 0)
        {
            x = 0;
        }
        if (z < 0)
        {
            z = 0;
        }
        if (x >= REGION_SIZE_X)
        {
            x = REGION_SIZE_X - 1;
        }
        if (z >= REGION_SIZE_Z)
        {
            z = REGION_SIZE_Z - 1;
        }
        return REGIONS[x][z];
    }

    public static void AddObject(WorldObject obj)
    {
        if (obj.IsPlayer())
        {
            Player player = obj.AsPlayer();

            if (!PLAYER_OBJECTS.Contains(player))
            {
                lock (ONLINE_CLIENTS)
                {
                    ONLINE_CLIENTS.Add(player.GetClient());
                }
                lock (PLAYER_OBJECTS)
                {
                    PLAYER_OBJECTS.Add(player);
                }

                // Log world access.
                if (Config.LOG_WORLD)
                {
                    LogManager.LogWorld("Player [" + player.GetName() + "] Account [" + player.GetClient().GetAccountName() + "] Entered the world.");
                }
            }
        }
    }

    public static void RemoveObject(WorldObject obj)
    {
        // Broadcast deletion to nearby players.
        List<Player> players = GetVisiblePlayers(obj);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ChannelSend(new DeleteObject(obj));
        }

        // Remove from list and take necessary actions.
        if (obj.IsPlayer())
        {
            Player player = obj.AsPlayer();

            lock (PLAYER_OBJECTS)
            {
                PLAYER_OBJECTS.Remove(player);
            }

            // Store player.
            player.StoreMe();

            // Log world access.
            if (Config.LOG_WORLD && player.GetClient().GetActiveChar() != null)
            {
                LogManager.LogWorld("Player [" + player.GetName() + "] Account [" + player.GetClient().GetAccountName() + "] Left the world.");
            }
        }

        obj.GetRegion().RemoveObject(obj);
    }

    public static List<WorldObject> GetVisibleObjects(WorldObject obj)
    {
        List<WorldObject> result = new List<WorldObject>();
        List<RegionHolder> regions = obj.GetRegion().GetSurroundingRegions();
        for (int i = 0; i < regions.Count; i++)
        {
            List<WorldObject> objects = regions[i].GetObjects();
            for (int j = 0; j < objects.Count; j++)
            {
                WorldObject nearby = objects[j];
                if (nearby.GetObjectId() == obj.GetObjectId())
                {
                    continue;
                }
                if (obj.CalculateDistance(nearby) < MOVEMENT_BROADCAST_RADIUS)
                {
                    result.Add(nearby);
                }
            }
        }
        return result;
    }

    public static List<Player> GetVisiblePlayers(WorldObject obj)
    {
        List<Player> result = new List<Player>();
        List<RegionHolder> regions = obj.GetRegion().GetSurroundingRegions();
        for (int i = 0; i < regions.Count; i++)
        {
            List<WorldObject> objects = regions[i].GetObjects();
            for (int j = 0; j < objects.Count; j++)
            {
                WorldObject nearby = objects[j];
                if (!nearby.IsPlayer())
                {
                    continue;
                }
                if (nearby.GetObjectId() == obj.GetObjectId())
                {
                    continue;
                }
                if (obj.CalculateDistance(nearby) < MOVEMENT_BROADCAST_RADIUS)
                {
                    result.Add(nearby.AsPlayer());
                }
            }
        }
        return result;
    }

    public static Player GetPlayerByName(string name)
    {
        for (int i = 0; i < PLAYER_OBJECTS.Count; i++)
        {
            Player player = PLAYER_OBJECTS[i];
            if (player.GetName().ToLowerInvariant().Equals(name.ToLowerInvariant()))
            {
                return player;
            }
        }
        return null;
    }

    public static int GetOnlineCount()
    {
        return ONLINE_CLIENTS.Count;
    }

    public static void AddClient(GameClient client)
    {
        lock (ONLINE_CLIENTS)
        {
            if (!ONLINE_CLIENTS.Contains(client))
            {
                ONLINE_CLIENTS.Add(client);
            }
        }
    }

    public static void RemoveClient(GameClient client)
    {
        // Store and remove player.
        Player player = client.GetActiveChar();
        if (player != null)
        {
            RemoveObject(player);
            client.SetActiveChar(null);
        }

        // Remove from list.
        lock (ONLINE_CLIENTS)
        {
            ONLINE_CLIENTS.Remove(client);
        }
    }

    public static GameClient GetClientByAccountName(string accountName)
    {
        for (int i = 0; i < ONLINE_CLIENTS.Count; i++)
        {
            GameClient client = ONLINE_CLIENTS[i];
            if (client.GetAccountName().Equals(accountName))
            {
                return client;
            }
        }
        return null;
    }
}
