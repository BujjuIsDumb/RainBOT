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

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Pagination;
using RainBOT.Core.Services;
using RainBOT.Core.Services.Models;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The report module.
    /// </summary>
    [SlashCommandGroup("report", "Report a user to warn the moderators when they join a server.")]
    public class Reporting : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /report create command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="user">The user option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("create", "Create a report.")]
        public async Task ReportCreateAsync(InteractionContext ctx,
            [Option("user", "The user to report.")] DiscordUser user)
        {
            if (Data.Reports.Exists(x => x.UserId == user.Id && x.CreatorUserId == ctx.User.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ You've already reported **{user.Username}**.", true);
                return;
            }

            #region Components
            var reportModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Detail the report")
                .WithCustomId($"reportModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Subject", customId: "subject", placeholder: $"Summarize the report. (i.e., Raider)", style: TextInputStyle.Short, min_length: 5, max_length: 30))
                .AddComponents(new TextInputComponent(label: "Body", customId: "body", placeholder: $"Explain what {user.Username} did.", style: TextInputStyle.Paragraph, min_length: 25, max_length: 1200));
            #endregion

            #region Event Handlers
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
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, reportModal);
        }

        /// <summary>
        ///     The /report revoke command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="user">The user option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("revoke", "Remove a report.")]
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

        /// <summary>
        ///     The /report list command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="user">The user option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
                pages.Add(new Page().WithEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(name: (await ctx.Client.GetUserAsync(report.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(report.CreatorUserId)).AvatarUrl)
                    .WithTitle(report.Subject)
                    .WithDescription(report.Body)
                    .WithTimestamp(report.CreationTimestamp)
                    .WithColor(new DiscordColor(6317300))));
            }

            await ctx.Interaction.CreatePaginatedResponseAsync(ctx.Client, pages, true);
        }

        /// <summary>
        ///     The View Reports context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
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
                pages.Add(new Page().WithEmbed(new DiscordEmbedBuilder()
                    .WithAuthor(name: (await ctx.Client.GetUserAsync(report.CreatorUserId)).Username, iconUrl: (await ctx.Client.GetUserAsync(report.CreatorUserId)).AvatarUrl)
                    .WithTitle(report.Subject)
                    .WithDescription(report.Body)
                    .WithTimestamp(report.CreationTimestamp)
                    .WithColor(new DiscordColor(6317300))));
            }

            await ctx.Interaction.CreatePaginatedResponseAsync(ctx.Client, pages, true);
        }
    }
}