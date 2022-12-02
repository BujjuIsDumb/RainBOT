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
using System.Text.RegularExpressions;
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
    [SlashCommandGroup("user", "Manage your user account.")]
    public class User : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("register", "Create a user account.")]
        [SlashUserBannable]
        public async Task UserRegisterAsync(InteractionContext ctx)
        {
            if (!Data.UserAccounts.Exists(x => x.UserId == ctx.User.Id))
            {
                Data.UserAccounts.Add(new UserAccountData()
                {
                    UserId = ctx.User.Id
                });
                Data.Update();

                await ctx.CreateResponseAsync("✅ Created a user account.", true);
            }
            else
            {
                await ctx.CreateResponseAsync("⚠️ You already have an account.", true);
            }
        }

        [SlashCommand("settings", "Manage the settings of your user account.")]
        [SlashRequireUserAccount]
        public async Task UserSettingsAsync(InteractionContext ctx)
        {
            // Build main menu.
            var mainEmbed = new DiscordEmbedBuilder()
                .WithTitle("User Settings")
                .WithThumbnail(ctx.User.AvatarUrl)
                .WithDescription("Manage the settings of your RainBOT user account. Please select a module to configure.")
                .WithColor(new DiscordColor(3092790));

            var ventingButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("ventingButton"), "Venting", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👁‍🗨")));
            var verificationButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("verificationButton"), "Verification", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .AddEmbed(mainEmbed)
                .AddComponents(ventingButton, verificationButton)
                .AsEphemeral());

            // Respond to button input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                // Unfinished
            };
        }

        [SlashCommand("data", "Request your user data.")]
        public async Task UserDataAsync(InteractionContext ctx)
        {
            var builder = new StringBuilder();

            foreach (var userAccount in Data.UserAccounts.FindAll(x => x.UserId == ctx.User.Id)) builder.AppendLine($"**User Account**\n```json\n{JsonConvert.SerializeObject(userAccount, Formatting.Indented)}```");
            foreach (var report in Data.Reports.FindAll(x => x.UserId == ctx.User.Id || x.CreatorUserId == ctx.User.Id)) builder.AppendLine($"**Report**\n```json\n{JsonConvert.SerializeObject(report, Formatting.Indented)}```");
            foreach (var userBan in Data.UserBans.FindAll(x => x.UserId == ctx.User.Id)) builder.AppendLine($"**User Ban**\n```json\n{JsonConvert.SerializeObject(userBan, Formatting.Indented)}```");
            if (builder.Length == 0) builder.Append("There is no data associated with this user.");

            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                .WithTitle("Your data")
                .WithDescription(builder.ToString())
                .WithColor(new DiscordColor(3092790)), true);
        }
    }
}