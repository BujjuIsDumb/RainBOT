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
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("report", "Report a user to warn the moderators when they join a server.")]
    public class Report : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("create", "Create a report.")]
        [SlashRequireUserAccount]
        public async Task ReportCreateAsync(InteractionContext ctx,
            [Option("user", "The user to report.")] DiscordUser user)
        {
            if (Data.Reports.Exists(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ You've already reported **{user.Username}**.", true);
                return;
            }

            // Build modal.
            var reportModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Detail the report")
                .WithCustomId(Core.Utilities.CreateCustomId("reportModal"))
                .AddComponents(new TextInputComponent(label: "Subject", customId: "subject", placeholder: $"Summarize the report. (i.e., Raider)", style: TextInputStyle.Short, min_length: 5, max_length: 30))
                .AddComponents(new TextInputComponent(label: "Body", customId: "body", placeholder: $"Explain what {user.Username} did.", style: TextInputStyle.Paragraph, min_length: 25, max_length: 1200));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, reportModal);

            // Respond to modal submission.
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == reportModal.CustomId)
                {
                    Data.Reports.Add(new ReportData()
                    {
                        UserId = user.Id,
                        CreatorUserId = ctx.User.Id,
                        Subject = args.Values["subject"],
                        Body = args.Values["body"],
                        CreationTimestamp = DateTime.UtcNow
                    });
                    Data.Update();

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent($"✅ Created a report for **{user.Username}**.")
                        .AsEphemeral());
                }
            };
        }

        [SlashCommand("revoke", "Remove a report.")]
        [SlashRequireUserAccount]
        public async Task ReportRevokeAsync(InteractionContext ctx,
            [Option("user", "The user to remove the report from.")] DiscordUser user)
        {
            var report = Data.Reports.Find(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id);

            if (report is not null)
            {
                Data.Reports.Remove(report);
                Data.Update();

                await ctx.CreateResponseAsync($"✅ Removed your report for **{user.Username}**.", true);
            }
            else
            {
                await ctx.CreateResponseAsync($"⚠️ You haven't reported **{user.Username}**.", true);
            }
        }

        [SlashCommand("list", "Get a list of reports for a user.")]
        public async Task ReportListAsync(InteractionContext ctx,
            [Option("user", "The user to view the reports of.")] DiscordUser user)
        {
            if (Data.Reports.FindAll(x => x.UserId == user.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{user.Username}** has no reports.", true);
                return;
            }

            // Generate pages.
            var pages = new List<Page>();
            foreach (var report in Data.Reports.FindAll(x => x.UserId == user.Id))
            {
                pages.Add(new Page(embed: new DiscordEmbedBuilder()
                    .WithAuthor(name: (await ctx.Client.GetUserAsync(report.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(report.CreatorUserId)).AvatarUrl)
                    .WithTitle(report.Subject)
                    .WithDescription(report.Body)
                    .WithTimestamp(report.CreationTimestamp)
                    .WithColor(new DiscordColor(3092790))));
            }

            await ctx.Client.GetInteractivity().SendPaginatedResponseAsync(ctx.Interaction, true, ctx.User, pages);
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Reports")]
        public async Task ViewReportsAsync(ContextMenuContext ctx)
        {
            if (Data.Reports.FindAll(x => x.UserId == ctx.TargetUser.Id).Count <= 0)
            {
                await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** has no reports.", true);
                return;
            }

            // Generate pages.
            var pages = new List<Page>();
            foreach (var report in Data.Reports.FindAll(x => x.UserId == ctx.TargetUser.Id))
            {
                pages.Add(new Page(embed: new DiscordEmbedBuilder()
                    .WithAuthor(name: (await ctx.Client.GetUserAsync(report.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(report.CreatorUserId)).AvatarUrl)
                    .WithTitle(report.Subject)
                    .WithDescription(report.Body)
                    .WithTimestamp(report.CreationTimestamp)
                    .WithColor(new DiscordColor(3092790))));
            }

            await ctx.Client.GetInteractivity().SendPaginatedResponseAsync(ctx.Interaction, true, ctx.User, pages);
        }
    }
}