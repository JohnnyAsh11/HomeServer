using Microsoft.AspNetCore.Mvc;
using RCON.Core;

namespace HomeServer.Api.Controllers
{
    /// <summary>
    /// Minecraft controller implementation.
    /// </summary>
    public class MinecraftController(
        MinecraftService minecraftClient,
        ILogger<MinecraftController> logger) : IMinecraftController
    {
        /// <inheritdoc/>
        public async Task<ActionResult<MinecraftResponseDto>> PostMinecraftCommandAsync(
            MinecraftCommandDto? body, 
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("POST minecraft command to server called.");
            string? command = body?.Command;

            if (command is null)
            {
                logger.LogError("Minecraft command is null.  Must have command to POST");
                return new BadRequestResult();
            }

            // Run the command & build the response object.
            MinecraftResponseDto response = new()
            {
                Response = await minecraftClient.ExecuteCommand(command)
            };

            // Response is only null if something has went wrong with the command.
            if (response.Response is null)
            {
                logger.LogWarning("Minecraft command execution possibly failed.  Response is null.");
            }

            return new OkObjectResult(response);
        }
    }
}
