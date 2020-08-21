using MySql.Data.MySqlClient;
using System;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class AccountAuthenticationRequest
{
    private static readonly string ACCOUNT_INFO_QUERY = "SELECT * FROM accounts WHERE account=@account";
    private static readonly string ACCOUNT_INFO_UPDATE_QUERY = "UPDATE accounts SET last_active=@last_active, last_ip=@ip WHERE account=@account";
    private static readonly string ACCOUNT_CREATE_QUERY = "INSERT INTO accounts (account, password, status) values (@account, @password, 3)";
    private static readonly int STATUS_NOT_FOUND = 0;
    private static readonly int STATUS_WRONG_PASSWORD = 3;
    private static readonly int STATUS_ALREADY_ONLINE = 4;
    private static readonly int STATUS_TOO_MANY_ONLINE = 5;
    private static readonly int STATUS_INCORRECT_CLIENT = 6;
    private static readonly int STATUS_AUTHENTICATED = 100;

    public AccountAuthenticationRequest(GameClient client, ReceivablePacket packet)
    {
        // Read data.
        double clientVersion = packet.ReadDouble();
        string accountName = packet.ReadString().ToLowerInvariant();
        string passwordHash = packet.ReadString();

        // Client version check.
        if (clientVersion != Config.CLIENT_VERSION)
        {
            client.ChannelSend(new AccountAuthenticationResult(STATUS_INCORRECT_CLIENT));
            return;
        }

        // Replace illegal characters.
        for (int i = 0; i < Util.ILLEGAL_CHARACTERS.Length; i++)
        {
            accountName = accountName.Replace(Util.ILLEGAL_CHARACTERS[i], '\'');
        }

        // Account name checks.
        if ((accountName.Length < 2) || (accountName.Length > 20) || accountName.Contains("'") || (passwordHash.Length == 0)) // 20 should not happen, checking it here in case of client cheat.
        {
            client.ChannelSend(new AccountAuthenticationResult(STATUS_NOT_FOUND));
            return;
        }

        // Get data from database.
        string storedPassword = "";
        int status = STATUS_NOT_FOUND;
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(ACCOUNT_INFO_QUERY, con);
            cmd.Parameters.AddWithValue("account", accountName);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                storedPassword = reader.GetString("password");
                status = reader.GetInt32("status");
            }
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }

        // In case of auto create accounts configuration.
        if ((status == 0) && Config.ACCOUNT_AUTO_CREATE)
        {
            // Create account.
            try
            {
                MySqlConnection con = DatabaseManager.GetConnection();
                MySqlCommand cmd = new MySqlCommand(ACCOUNT_CREATE_QUERY, con);
                cmd.Parameters.AddWithValue("account", accountName);
                cmd.Parameters.AddWithValue("password", passwordHash);
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception e)
            {
                LogManager.Log(e.ToString());
            }
            LogManager.Log("Created account " + accountName + ".");
        }
        else // Account status issue.
        {
            // 0 does not exist, 1 banned, 2 requires activation, 3 wrong password, 4 too many online, 100 authenticated
            if (status < STATUS_WRONG_PASSWORD)
            {
                client.ChannelSend(new AccountAuthenticationResult(status));
                return;
            }

            // Wrong password.
            if (!passwordHash.Equals(storedPassword))
            {
                client.ChannelSend(new AccountAuthenticationResult(STATUS_WRONG_PASSWORD));
                return;
            }
        }

        // Kick existing logged client.
        GameClient existingClient = WorldManager.GetClientByAccountName(accountName);
        if (existingClient != null)
        {
            existingClient.ChannelSend(new Logout());
            WorldManager.RemoveClient(existingClient);
            client.ChannelSend(new AccountAuthenticationResult(STATUS_ALREADY_ONLINE));
            return;
        }

        // Too many online users.
        if (WorldManager.GetOnlineCount() >= Config.MAXIMUM_ONLINE_USERS)
        {
            client.ChannelSend(new AccountAuthenticationResult(STATUS_TOO_MANY_ONLINE));
            return;
        }

        // Authentication was successful.
        WorldManager.AddClient(client);
        client.SetAccountName(accountName);
        client.ChannelSend(new AccountAuthenticationResult(STATUS_AUTHENTICATED));

        // Update last login date and IP address.
        try
        {
            MySqlConnection con = DatabaseManager.GetConnection();
            MySqlCommand cmd = new MySqlCommand(ACCOUNT_INFO_UPDATE_QUERY, con);
            cmd.Parameters.AddWithValue("last_active", DateTimeOffset.Now.ToUnixTimeSeconds());
            cmd.Parameters.AddWithValue("ip", client.GetIp());
            cmd.Parameters.AddWithValue("account", accountName);
            cmd.ExecuteNonQuery();
            con.Close();
        }
        catch (Exception e)
        {
            LogManager.Log(e.ToString());
        }
    }
}
