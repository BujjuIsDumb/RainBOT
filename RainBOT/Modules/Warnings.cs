﻿// This file is from RainBOT (https://github.com/BujjuIsDumb/RainBOT)
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

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("warn", "Warn users to log rule violations.")]
    [GuildOnly]
    [SlashCommandPermissions(Permissions.ModerateMembers)]
    [SlashRequireGuildAccount]
    public class Warnings : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("create", "Create a warning.")]
        public async Task WarnCreateAsync(InteractionContext ctx,
            [Option("user", "The user to warn.")] DiscordUser user,
            [MaximumLength(30)]
            [Option("warning", "What did the user do?")] string warning)
        {
            if (Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().Exists(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ You've already warned **{user.Username}**.", true);
                return;
            }

            var warnings = Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList();
            warnings.Add(new WarnData()
            {
                UserId = user.Id,
                CreatorUserId = ctx.User.Id,
                Description = warning
            });

            Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings = warnings.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync($"✅ Created a warning for **{user.Username}**.", true);
        }

        [SlashCommand("revoke", "Remove a warn.")]
        public async Task WarnRevokeAsync(InteractionContext ctx,
            [Option("user", "The user to remove the warning from.")] DiscordUser user)
        {
            var warn = Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().Find(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id);

            if (warn is not null)
            {
                var warnings = Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList();
                warnings.Remove(warn);

                Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings = warnings.ToArray();
                Data.Update();

                await ctx.CreateResponseAsync($"✅ Removed your warning for **{user.Username}**.", true);
            }
            else
            {
                await ctx.CreateResponseAsync($"⚠️ You haven't warned **{user.Username}**.", true);
            }
        }

        [SlashCommand("list", "Get a list of warnings for a user.")]
        public async Task WarnListAsync(InteractionContext ctx,
            [Option("user", "The user to view the warnings of.")] DiscordUser user)
        {
            if (Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().FindAll(x => x.UserId == user.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{user.Username}** has no warnings.", true);
                return;
            }

            // Create select menu options for each warning.
            var warnSelectOptions = new List<DiscordSelectComponentOption>();
            foreach (WarnData warn in Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings)
            {
                DiscordUser creator = await ctx.Client.GetUserAsync(warn.CreatorUserId);
                warnSelectOptions.Add(new DiscordSelectComponentOption(warn.Description, creator.Id.ToString(), $"Created by {creator.Username}"));
            }

            var warnSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("warnSelect"), "Select a warning", warnSelectOptions);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Please select a warn to view.")
                .AddComponents(warnSelect)
                .AsEphemeral());

            // Respond to select menu input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == warnSelect.CustomId)
                {
                    var warn = Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().Find(x => x.UserId == user.Id && x.CreatorUserId == ulong.Parse(args.Values.First())) ?? new WarnData();

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).AvatarUrl)
                        .WithTitle("Warning")
                        .WithDescription(warn.Description)
                        .WithColor(new DiscordColor(3092790));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AsEphemeral());
                }
            };
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Warnings")]
        [SlashRequireGuildAccount]
        public async Task ViewWarningsAsync(ContextMenuContext ctx)
        {
            if (Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().FindAll(x => x.UserId == ctx.TargetUser.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** has no warnings.", true);
                return;
            }

            // Create select menu options for each warning.
            var warnSelectOptions = new List<DiscordSelectComponentOption>();
            foreach (WarnData warn in Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings)
            {
                DiscordUser creator = await ctx.Client.GetUserAsync(warn.CreatorUserId);
                warnSelectOptions.Add(new DiscordSelectComponentOption(warn.Description, creator.Id.ToString(), $"Created by {creator.Username}"));
            }

            var warnSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("warnSelect"), "Select a warning", warnSelectOptions);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Please select a warn to view.")
                .AddComponents(warnSelect)
                .AsEphemeral());

            // Respond to select menu input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == warnSelect.CustomId)
                {
                    var warn = Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id).Warnings.ToList().Find(x => x.UserId == ctx.TargetUser.Id && x.CreatorUserId == ulong.Parse(args.Values.First())) ?? new WarnData();

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).AvatarUrl)
                        .WithTitle("Warning")
                        .WithDescription(warn.Description)
                        .WithColor(new DiscordColor(3092790));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AsEphemeral());
                }
            };
        }
    }
}