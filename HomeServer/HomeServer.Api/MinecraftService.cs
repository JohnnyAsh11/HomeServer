using Microsoft.AspNetCore.Identity;
using RCON.Core;
using RCON.Core.Interfaces;

namespace HomeServer.Api
{
    public class MinecraftService
    {
        private readonly ILogger<MinecraftService> _logger;
        private const int MinecraftPort = 25575;
        private const string MinecraftHost = "homeserver-minecraft";
        private const string MinecraftPassword = "admin";
        private const int MinecraftTimeout = 10 * 1_000;

        /// <summary>
        /// Constructs the Minecraft command executing service.
        /// </summary>
        public MinecraftService(
            ILogger<MinecraftService> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Executes the passed in command on the Minecraft server.
        /// </summary>
        public async Task<string?> ExecuteCommand(string command)
        {
            // Creating an RconClient for interactions with the Minecraft server.
            IRconClient client = RconClientBuilder.Create()
                .WithTimeout(MinecraftTimeout)
                .WithHost(MinecraftHost)
                .WithPort(MinecraftPort)
                .WithPassword(MinecraftPassword)
                .Build();

            try
            {
                // Connecting to the server.
                await client.ConnectAsync();
                _logger.LogInformation("Connected to Minecraft server at {host}:{port}", MinecraftHost, MinecraftPort);

                // Executing the passed in command.
                _logger.LogInformation("Executing the following command on Minecraft server: {command}", command);
                return await client.ExecuteCommandAsync(command);
            }
            catch (Exception error)
            {
                _logger.LogError("An error occurred while executing a Minecraft command: {err}", error.Message);
            }
            finally
            {
                // Ensuring the client disconnects from the server.
                await client.DisconnectAsync();
                client.Dispose();
            }

            return null;
        }
    }
}
