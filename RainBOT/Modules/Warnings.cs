// This file is from RainBOT (https://github.com/BujjuIsDumb/RainBOT)
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
using RainBOT.Core;
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
            [Option("warning", "What did the user do?")] string warning)
        {
            if (ctx.Guild.GetGuildAccount(Data).Warnings.ToList().Exists(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ You've already warned **{user.Username}**.", true);
                return;
            }

            var warnings = ctx.Guild.GetGuildAccount(Data).Warnings.ToList();
            warnings.Add(new WarnData()
            {
                UserId = user.Id,
                CreatorUserId = ctx.User.Id,
                Description = warning,
                CreationTimestamp = DateTime.UtcNow
            });

            ctx.Guild.GetGuildAccount(Data).Warnings = warnings.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync($"✅ Warned **{user.Username}**.", true);
        }

        [SlashCommand("revoke", "Remove a warning.")]
        public async Task WarnRevokeAsync(InteractionContext ctx,
            [Option("user", "The user to remove the warning from.")] DiscordUser user)
        {
            var warn = ctx.Guild.GetGuildAccount(Data).Warnings.ToList().Find(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id);

            if (warn is not null)
            {
                var warnings = ctx.Guild.GetGuildAccount(Data).Warnings.ToList();
                warnings.Remove(warn);

                ctx.Guild.GetGuildAccount(Data).Warnings = warnings.ToArray();
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
            if (ctx.Guild.GetGuildAccount(Data).Warnings.ToList().FindAll(x => x.UserId == user.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{user.Username}** has not been warned.", true);
                return;
            }

            // Create select menu options for each warning.
            var warnSelectOptions = new List<DiscordSelectComponentOption>();
            foreach (var warn in ctx.Guild.GetGuildAccount(Data).Warnings)
            {
                var creator = await ctx.Client.GetUserAsync(warn.CreatorUserId);
                warnSelectOptions.Add(new DiscordSelectComponentOption($"Warning from {creator.Username}", creator.Id.ToString()));
            }

            var warnSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("warnSelect"), "Select a warning", warnSelectOptions);

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Please select a warning to view.")
                .AddComponents(warnSelect)
                .AsEphemeral());

            // Respond to select menu input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == warnSelect.CustomId)
                {
                    var warn = ctx.Guild.GetGuildAccount(Data).Warnings.ToList().Find(x => x.UserId == user.Id && x.CreatorUserId == ulong.Parse(args.Values.First())) ?? new WarnData();

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).AvatarUrl)
                        .WithDescription(warn.Description)
                        .WithTimestamp(warn.CreationTimestamp)
                        .WithColor(new DiscordColor(3092790));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AsEphemeral());
                }
            };
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Warnings")]
        [ContextMenuRequireGuildAccount]
        public async Task ViewWarningsAsync(ContextMenuContext ctx)
        {
            if (ctx.Guild.GetGuildAccount(Data).Warnings.ToList().FindAll(x => x.UserId == ctx.TargetUser.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** has no warnings.", true);
                return;
            }

            // Create select menu options for each warning.
            var warnSelectOptions = new List<DiscordSelectComponentOption>();
            foreach (var warn in ctx.Guild.GetGuildAccount(Data).Warnings)
            {
                var creator = await ctx.Client.GetUserAsync(warn.CreatorUserId);
                warnSelectOptions.Add(new DiscordSelectComponentOption($"Warning from {creator.Username}", creator.Id.ToString()));
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
                    var warn = ctx.Guild.GetGuildAccount(Data).Warnings.ToList().Find(x => x.UserId == ctx.TargetUser.Id && x.CreatorUserId == ulong.Parse(args.Values.First())) ?? new WarnData();

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(warn.CreatorUserId)).AvatarUrl)
                        .WithDescription(warn.Description)
                        .WithTimestamp(warn.CreationTimestamp)
                        .WithColor(new DiscordColor(3092790));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AsEphemeral());
                }
            };
        }
    }
}