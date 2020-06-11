using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ClashLand.Extensions;
using ClashLand.Core.Networking;
using ClashLand.Core.Database;

namespace ClashLand.Core.API.Discord.Modules
{
    [Name("Core")]
    internal class Core_Module : ModuleBase<SocketCommandContext>
    {
        internal CommandService _CService;

        public Core_Module(CommandService CService)
        {
            _CService = CService;
        }

        [Command("status", RunMode = RunMode.Async)]
        public async Task Status()
        {
            var _Builder = new EmbedBuilder
            {
                Color = new Color(65, 105, 225),
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Requested by @{Context.User.Username}"
                },
                Timestamp = DateTime.UtcNow,
            };
            _Builder.WithTitle(Utils.ParseConfigString("DiscordServerName") + " Status");


            _Builder.AddInlineField("SocketAsyncEventArgs", $"Created: {Gateway.NumberOfArgsCreated}\nIn-use: {Gateway.NumberOfArgsInUse}\nAvailable: {Gateway.NumberOfArgs}");
            _Builder.AddInlineField("Buffer", $"Created: {Gateway.NumberOfBuffersCreated}\nIn-use: {Gateway.NumberOfBuffersInUse}\nAvailable: {Gateway.NumberOfBuffers}");

            _Builder.AddInlineField("Online Players", $"{ResourcesManager.OnlinePlayers.Count}");
            _Builder.AddInlineField("In Memory Clans", $"{ResourcesManager.GetInMemoryAllianceCount()}");

            await ReplyAsync("", false, _Builder);
        }

        [Command("statusextra", RunMode = RunMode.Async)]
        public async Task StatusExtra()
        {
            var _Builder = new EmbedBuilder
             {
                 Color = new Color(65, 105, 225),
                 Footer = new EmbedFooterBuilder()
                 {
                     Text = $"Requested by @{Context.User.Username}"
                 },
                 Timestamp = DateTime.UtcNow,
             };
            _Builder.WithTitle(Utils.ParseConfigString("DiscordServerName") + " Status");


            _Builder.AddInlineField("SocketAsyncEventArgs", $"Created: {Gateway.NumberOfArgsCreated}\nIn-use: {Gateway.NumberOfArgsInUse}\nAvailable: {Gateway.NumberOfArgs}");
            _Builder.AddInlineField("Buffer", $"Created: {Gateway.NumberOfBuffersCreated}\nIn-use: {Gateway.NumberOfBuffersInUse}\nAvailable: {Gateway.NumberOfBuffers}");

            _Builder.AddInlineField("Online Players", $"{ResourcesManager.OnlinePlayers.Count}");
            _Builder.AddInlineField("In Memory Clans", $"{ResourcesManager.GetInMemoryAllianceCount()}");
            _Builder.AddInlineField("Saved Players", MySQL_V2.GetPlayerCount());
            _Builder.AddInlineField("Saved Clans", MySQL_V2.GetClanCount());

            await ReplyAsync("", false, _Builder);
        }

        [Command("ping", RunMode = RunMode.Async)]
        public async Task Ping()
        {
            var sw = Stopwatch.StartNew();
            var msg = await Context.Channel.SendMessageAsync("🏓").ConfigureAwait(false);
            sw.Stop();
            await msg.DeleteAsync();

            await Context.Channel.SendMessageAsync($"{Format.Bold(Context.User.ToString())} 🏓 {(int)sw.Elapsed.TotalMilliseconds}ms").ConfigureAwait(false);
        }

        [Command("easystatus", RunMode = RunMode.Async)]
        public async Task Easystatus()
        {
            var _Builder = new EmbedBuilder
            {
                Color = new Color(65, 105, 225),
                Footer = new EmbedFooterBuilder()
                {
                    Text = $"Requested by @{Context.User.Username}"
                },
                Timestamp = DateTime.UtcNow,
            };
            _Builder.WithTitle(Utils.ParseConfigString("DiscordServerName") + " Status");


            _Builder.AddInlineField("SocketAsyncEventArgs", $"Created: {Gateway.NumberOfArgsCreated}\nIn-use: {Gateway.NumberOfArgsInUse}\nAvailable: {Gateway.NumberOfArgs}");
            _Builder.AddInlineField("Buffer", $"Created: {Gateway.NumberOfBuffersCreated}\nIn-use: {Gateway.NumberOfBuffersInUse}\nAvailable: {Gateway.NumberOfBuffers}");
            await ReplyAsync("", false, _Builder);
        }
    }
}