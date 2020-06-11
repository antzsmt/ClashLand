using System;
using ClashLand.Extensions;
using Discord;
using Discord.WebSocket;

namespace ClashLand.Core.Core.API.Discord
{
    internal static class Client
    {
        internal static DiscordSocketClient _Client;
        internal static CommandHandler CommandHandler;

        internal static async void Initialize()
        {
            try
            {
                _Client = new DiscordSocketClient(
                    new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Error,
                    });
                Console.WriteLine(@"Diskord Bot run!" + Environment.NewLine);
                CommandHandler = new CommandHandler();
                CommandHandler.InstallAsync(_Client).Wait();

                await _Client.LoginAsync(TokenType.Bot, Constants.DiscordToken);
                await _Client.StartAsync();
                await _Client.SetGameAsync("Waiting for Status request");
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "Failed to connect to the Discord API server.");
            }
        }
        internal static async void Deinitialize()
        {
            try
            {
                await _Client.LogoutAsync();
                await _Client.StopAsync();
            }
            catch (Exception ex)
            {
                ExceptionLogger.Log(ex, "Failed to diconnect from the Discord API server.");
            }
        }

    }
}