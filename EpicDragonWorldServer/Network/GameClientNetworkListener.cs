using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

/**
 * Author: Pantelis Andrianakis
 * Date: November 7th 2018
 */
public class GameClientNetworkListener
{
    private static readonly List<Task> CONNECTIONS = new List<Task>(); // Pending connections.

    public static void Init()
    {
        new GameClientNetworkListener().StartListener().Wait();
    }

    // The core server task.
    private async Task StartListener()
    {
        TcpListener tcpListener = TcpListener.Create(Config.SERVER_PORT);
        tcpListener.Start();
        LogManager.Log("Listening on port " + Config.SERVER_PORT + ".");
        while (true)
        {
            TcpClient tcpClient = await tcpListener.AcceptTcpClientAsync();
            Task task = StartHandleConnectionAsync(tcpClient);
            // If already faulted, re-throw any error on the calling context.
            if (task.IsFaulted)
            {
                task.Wait();
            }
        }
    }

    // Register and handle the connection.
    private async Task StartHandleConnectionAsync(TcpClient tcpClient)
    {
        // Start the new connection task.
        Task connectionTask = HandleConnectionAsync(tcpClient);

        // Add it to the list of pending task.
        lock (CONNECTIONS)
        {
            CONNECTIONS.Add(connectionTask);
        }

        // Catch all errors of HandleConnectionAsync.
        try
        {
            await connectionTask;
            // We may be on another thread after "await".
        }
        catch (Exception e)
        {
            // Log the error.
            LogManager.Log(e.ToString());
        }
        finally
        {
            // Remove pending task.
            lock (CONNECTIONS)
            {
                CONNECTIONS.Remove(connectionTask);
            }
        }
    }

    // Handle new connection.
    private async Task HandleConnectionAsync(TcpClient tcpClient)
    {
        // Continue asynchronously on another threads.
        await Task.Yield();

        // Initialize game client.
        NetworkStream networkStream = tcpClient.GetStream();
        GameClient gameClient = new GameClient(networkStream, tcpClient.Client.RemoteEndPoint.ToString());

        byte[] bufferLength = new byte[2]; // We use 2 bytes for short value.
        byte[] bufferData;
        short length; // Since we use short value, max length should be 32767.

        while (!(tcpClient.Client.Poll(0, SelectMode.SelectRead) && !networkStream.DataAvailable))
        {
            try
            {
                // Get packet data length.
                await networkStream.ReadAsync(bufferLength, 0, 2);
                length = BitConverter.ToInt16(bufferLength);
                // Get packet data.
                bufferData = new byte[length];
                await networkStream.ReadAsync(bufferData, 0, length);
                // Handle packet.
                GameClientPacketHandler.Handle(gameClient, new ReceivablePacket(Encryption.Process(bufferData))); // Decrypt
            }
            catch (Exception)
            {
                // When client is disconnected remove player from the world.
                tcpClient.Close();
                WorldManager.RemoveClient(gameClient);
                break;
            }
        }

        // Disconnected.
        WorldManager.RemoveClient(gameClient);
    }
}
