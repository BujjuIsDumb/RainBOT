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
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The report module.
    /// </summary>
    [SlashCommandGroup("report", "Report a user to warn the moderators when they join a server.")]
    public class Report : ApplicationCommandModule
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

        }

        /// <summary>
        ///     The View Reports context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Reports")]
        public async Task ViewReportsAsync(ContextMenuContext ctx)
        {

        }
    }
}