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
using RainBOT.SupportBot.Core;
using RainBOT.SupportBot.Core.Services;

namespace RainBOT.SupportBot.Modules
{
    /// <summary>
    ///     The modmail module.
    /// </summary>
    public class Modmail : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /modmail command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("modmail", "Send a message to the server's moderators.")]
        public async Task ModmailAsync(InteractionContext ctx)
        {
            var config = Configuration.Load("config.json");

            #region Components
            var modmailModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Send a modmail message")
                .WithCustomId($"modmailModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Message", customId: "message", style: TextInputStyle.Paragraph, min_length: 5, max_length: 1200));
            #endregion

            #region Event Handlers
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == modmailModal.CustomId)
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: ctx.User.Username, iconUrl: ctx.User.AvatarUrl)
                        .WithTitle("📨 A new modmail message has arrived")
                        .WithDescription(args.Values["message"])
                        .WithColor(new DiscordColor(3092790));

                    await (await ctx.Client.GetChannelAsync(config.ModmailChannelId)).SendMessageAsync(embed);

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Your message has been sent to the moderators.")
                        .AsEphemeral());
                }
            };
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, modmailModal);
        }
    }
}