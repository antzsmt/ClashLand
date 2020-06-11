using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using ClashLand.Extensions;
using ClashLand.Core.API.Discord.Modules;

namespace ClashLand.Core.Core.API.Discord
{
    internal class CommandHandler
    {
        internal DiscordSocketClient _DiscordClient;
        internal CommandService _CommandService;

        public async Task InstallAsync(DiscordSocketClient DiscordClient)
        {
            _DiscordClient = DiscordClient;
            _CommandService = new CommandService();

            await _CommandService.AddModuleAsync(typeof(Core_Module));

            _DiscordClient.MessageReceived += HandleCommandAsync;
        }

        private async Task HandleCommandAsync(SocketMessage _SocketMessage)
        {
            var _Message = _SocketMessage as SocketUserMessage;
            if (_Message == null)
                return;

            var _Context = new SocketCommandContext(_DiscordClient, _Message);

            var argPos = 0;
            if (_Message.HasStringPrefix(Constants.DiscordPrefix, ref argPos) ||
                _Message.HasMentionPrefix(_DiscordClient.CurrentUser, ref argPos)) 
                await _CommandService.ExecuteAsync(_Context, argPos);
        }
    }
}