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
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.Core;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The mental health module.
    /// </summary>
    public class MentalHealth : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /vent command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="anonymous">The anonymous option.</param>
        /// <param name="tw">The tw option.</param>
        /// <returns></returns>
        [SlashCommand("vent", "Create a vent.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.AccessChannels | Permissions.SendMessages | Permissions.EmbedLinks)]
        public async Task VentAsync(InteractionContext ctx,
            [Option("anonymous", "Whether or not the vent will show you name to non-moderators.")] bool anonymous,
            [Option("tw", "Whether or not the vent contains potentially triggering content.")] bool tw)
        {
            if (anonymous && !ctx.Guild.GetGuildData(Data).AnonymousVenting)
            {
                await ctx.CreateResponseAsync("⚠️ You are not allowed to make anonymous vents in this server.", true);
                return;
            }

            var responseCache = new List<VentResponse>();

            #region Components
            var ventModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Create the vent")
                .WithCustomId($"ventModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Vent", customId: "vent", placeholder: "What do you want to get off your chest?", style: TextInputStyle.Paragraph, min_length: 25, max_length: 1200));

            var respondModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Respond to the vent")
                .WithCustomId($"respondModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Response", customId: "response", style: TextInputStyle.Paragraph, min_length: 5, max_length: 1200));

            var respondButton = new DiscordButtonComponent(ButtonStyle.Primary, $"respondButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Respond", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("💬")));

            var moderateButton = new DiscordButtonComponent(ButtonStyle.Secondary, $"moderateButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Moderate", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚙")));
            #endregion

            #region Event Handlers
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == ventModal.CustomId)
                {
                    await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                        .WithEmbed(new DiscordEmbedBuilder()
                        .WithAuthor(name: anonymous ? null : ctx.User.Username, iconUrl: anonymous ? null : ctx.User.AvatarUrl)
                        .WithTitle("📨 A new vent has arrived!")
                        .WithDescription((tw ? "||" : null) + args.Values["vent"] + (tw ? "||" : null))
                        .WithFooter(tw ? "This vent contains potentially triggering content!" : null)
                        .WithColor(new DiscordColor(3092790)))
                        .AddComponents(respondButton, moderateButton));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Your vent has been sent.")
                        .AsEphemeral());
                }
                else if (args.Interaction.Data.CustomId == respondModal.CustomId)
                {
                    try
                    {
                        await ctx.Member.SendMessageAsync(new DiscordEmbedBuilder()
                            .WithTitle("📨 A new vent response has arrived.")
                            .WithDescription("> " + args.Values["response"])
                            .WithColor(new DiscordColor(3092790)));

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("✅ Your response has been sent.")
                            .AsEphemeral());

                        responseCache.Add(new(args.Interaction.User, args.Values["response"], DateTimeOffset.Now));
                    }
                    catch (UnauthorizedException)
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("⚠️ The user doesn't have their DMs enabled for this server.")
                            .AsEphemeral());
                    }
                }
            };

            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == respondButton.CustomId)
                {
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, respondModal);
                }
                else if (args.Id == moderateButton.CustomId)
                {
                    var user = args.User as DiscordMember;

                    if (ctx.Guild.GetGuildData(Data).VentModerators.Contains(user.Id) || user.Permissions.HasPermission(Permissions.Administrator) || user.IsOwner)
                    {
                        var embed = new DiscordEmbedBuilder()
                            .WithTitle($"Vent from {ctx.User.Username}")
                            .WithThumbnail(ctx.User.AvatarUrl)
                            .WithDescription($"**Anonymous:** {(anonymous ? "Enabled" : "Disabled")}\n**Trigger Warning:** {(tw ? "Enabled" : "Disabled")}\n**Responses:** {responseCache.Count}")
                            .WithColor(new DiscordColor(3092790));

                        foreach (var response in responseCache)
                            embed.AddField($"Response from {response.creator.Username}", $"> {response.message}" + $"\n\nSent <t:{response.timestamp.ToUnixTimeSeconds()}:R>");

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed)
                            .AsEphemeral());
                    }
                    else
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("⚠️ You are not a vent moderator. Vent moderators can be set in server settings.")
                            .AsEphemeral());
                    }
                }
            };
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, ventModal);
        }

        /// <summary>
        ///     The /hotlines command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("hotlines", "Get a list of hotlines.")]
        public async Task HotlinesAsync(InteractionContext ctx)
        {

        }

        /// <summary>
        ///     A cached response to a vent.
        /// </summary>
        /// <param name="creator">The creator of the vent.</param>
        /// <param name="message">The message that was sent.</param>
        /// <param name="timestamp">When the message was sent.</param>
        private record VentResponse(DiscordUser creator, string message, DateTimeOffset timestamp);
    }
}