// This file is from RainBOT.
// 
// Copyright(c) 2022 Bujju
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("server", "Manage your server account.", false)]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    [GuildOnly]
    public class Server : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("register", "Create a server account.")]
        [SlashGuildBannable]
        public async Task ServerRegisterAsync(InteractionContext ctx)
        {
            if (!Data.GuildAccounts.Exists(x => x.GuildId == ctx.Guild.Id))
            {
                Data.GuildAccounts.Add(new GuildAccountData()
                {
                    GuildId = ctx.Guild.Id
                });
                Data.Update();

                await ctx.CreateResponseAsync("✅ Created a server account.", true);
            }
            else
            {
                await ctx.CreateResponseAsync("⚠️ This server is already registered.", true);
            }
        }

        [SlashCommand("settings", "Manage the settings of your server account.")]
        [SlashRequireGuildAccount]
        public async Task ServerSettingsAsync(InteractionContext ctx)
        {
            // Build main menu.
            var mainEmbed = new DiscordEmbedBuilder()
                .WithTitle("Server Settings")
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithDescription("Manage the settings of your RainBOT server account. Please select a module to configure.")
                .WithColor(new DiscordColor(3092790));

            var ventingButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("ventingButton"), "Venting", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👁‍🗨")));
            var bioButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("bioButton"), "Bio", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("📜")));

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .AddEmbed(mainEmbed)
                .AddComponents(ventingButton, bioButton)
                .AsEphemeral());

            // Respond to button input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                // Unfinished
            };
        }

        [SlashCommand("data", "Request your server data.")]
        public async Task ServerDataAsync(InteractionContext ctx)
        {
            var builder = new StringBuilder();

            foreach (var guildAccount in Data.GuildAccounts.FindAll(x => x.GuildId == ctx.Guild.Id)) builder.AppendLine($"**Server Account**\n```json\n{JsonConvert.SerializeObject(guildAccount, Formatting.Indented)}```");
            foreach (var guildBan in Data.GuildBans.FindAll(x => x.GuildId == ctx.Guild.Id)) builder.AppendLine($"**Server Ban**\n```json\n{JsonConvert.SerializeObject(guildBan, Formatting.Indented)}```");
            if (builder.Length == 0) builder.Append("There is no data associated with this server.");

            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
                .WithTitle("Your data")
                .WithDescription(builder.ToString())
                .WithColor(new DiscordColor(3092790)), true);
        }
    }
}