using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;

/**
 * Author: Pantelis Andrianakis
 * Date: November 29th 2019
 */
public class SpawnCommand
{
    private static readonly string SPAWN_SAVE_QUERY = "INSERT INTO spawnlist (npc_id, x, y, z, heading, respawn_delay) values (@npc_id, @x, @y, @z, @heading, @respawn_delay)";

    public static void Handle(Player player, string command)
    {
        // Gather information from parameters.
        string[] commandSplit = command.Split(' ');
        int npcId;
        if (commandSplit.Length > 1)
        {
            npcId = int.Parse(commandSplit[1]);
        }
        else
        {
            ChatManager.SendSystemMessage(player, "Proper syntax is /spawn npcId delaySeconds(optional).");
            return;
        }
        int respawnDelay = 60;
        if (commandSplit.Length > 2)
        {
            respawnDelay = int.Parse(commandSplit[2]);
        }

        // Log admin activity.
        LocationHolder playerLocation = player.GetLocation();
        LocationHolder npcLocation = new LocationHolder(playerLocation.GetX(), playerLocation.GetY(), playerLocation.GetZ(), playerLocation.GetHeading());
        StringBuilder sb = new StringBuilder();
        if (Config.LOG_ADMIN)
        {
            sb.Append(player.GetName());
            sb.Append(" used command /spawn ");
            sb.Append(npcId);
            sb.Append(" ");
            sb.Append(respawnDelay);
            sb.Append(" at ");
            sb.Append(npcLocation);
            LogManager.LogAdmin(sb.ToString());
            sb.Clear();
        }

        // Spawn NPC.
        Npc npc = SpawnData.SpawnNpc(npcId, npcLocation, respawnDelay);

        // Broadcast NPC information.
        NpcInformation info = new NpcInformation(npc);
        player.ChannelSend(info);
        List<Player> players = WorldManager.GetVisiblePlayers(player);
        for (int i = 0; i < players.Count; i++)
        {
            players[i].ChannelSend(info);
        }

        // Send player success message.
        sb.Append("You have spawned ");
        sb.Append(npcId);
        sb.Append(" at ");
        sb.Append(npcLocation);
        ChatManager.SendSystemMessage(player, sb.ToString());

        // Store in database.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(SPAWN_SAVE_QUERY, con);
            cmd.Parameters.AddWithValue("npc_id", npcId);
            cmd.Parameters.AddWithValue("x", npcLocation.GetX());
            cmd.Parameters.AddWithValue("y", npcLocation.GetY());
            cmd.Parameters.AddWithValue("z", npcLocation.GetZ());
            cmd.Parameters.AddWithValue("heading", npcLocation.GetHeading());
            cmd.Parameters.AddWithValue("respawn_delay", respawnDelay);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }
    }
}
