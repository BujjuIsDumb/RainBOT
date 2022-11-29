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
using DSharpPlus.Exceptions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    public class Venting : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("vent", "Create a vent.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.AccessChannels | Permissions.SendMessages | Permissions.EmbedLinks)]
        [SlashRequireUserAccount]
        [SlashRequireGuildAccount]
        public async Task VentAsync(InteractionContext ctx,
            [Option("anonymous", "Whether or not the vent will show you name to non-moderators.")] bool anonymous,
            [Option("tw", "Whether or not the vent contains potentially triggering content.")] bool tw)
        {
            var responseCache = new List<(DiscordUser creator, string response)>();

            if (anonymous && !ctx.Guild.GetGuildAccount(Data).AnonymousVenting)
            {
                await ctx.CreateResponseAsync("⚠️ You are not allowed to make anonymous vents in this server.", true);
                return;
            }

            // Build modal.
            var ventModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Create the vent")
                .WithCustomId(Core.Utilities.CreateCustomId("ventModal"))
                .AddComponents(new TextInputComponent(label: "Vent", customId: "vent", placeholder: "What do you want to get off your chest?", style: TextInputStyle.Paragraph, min_length: 25, max_length: 1200));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, ventModal);

            // Respond to modal submission.
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == ventModal.CustomId)
                {
                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: anonymous ? null : ctx.User.Username, iconUrl: anonymous ? null : ctx.User.AvatarUrl)
                        .WithTitle("📨 A new vent has arrived!")
                        .WithDescription((tw ? "||" : null) + args.Values["vent"] + (tw ? "||" : null))
                        .WithFooter(tw ? "This vent contains potentially triggering content!" : null)
                        .WithColor(new DiscordColor(3092790));

                    var respondButton = new DiscordButtonComponent(ButtonStyle.Primary, Core.Utilities.CreateCustomId("respondButton"), "Respond", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("💬")));
                    var moderateButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("moderateButton"), "Moderate", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚙")));

                    var message = await ctx.Channel.SendMessageAsync(new DiscordMessageBuilder()
                        .WithEmbed(embed)
                        .AddComponents(respondButton, moderateButton));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Your vent has been sent.")
                        .AsEphemeral());

                    // Respond to button input.
                    ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                    {
                        if (args.Id == respondButton.CustomId)
                        {
                            if (ctx.User.GetUserAccount(Data).AllowVentResponses)
                            {
                                // Build modal.
                                var respondModal = new DiscordInteractionResponseBuilder()
                                    .WithTitle("Respond to the vent")
                                    .WithCustomId(Core.Utilities.CreateCustomId("respondModal"))
                                    .AddComponents(new TextInputComponent(label: "Response", customId: "response", style: TextInputStyle.Paragraph, min_length: 5, max_length: 1200));

                                await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, respondModal);

                                // Respond to modal submission.
                                ctx.Client.ModalSubmitted += async (sender, args) =>
                                {
                                    if (args.Interaction.Data.CustomId == respondModal.CustomId)
                                    {
                                        try
                                        {
                                            var embed = new DiscordEmbedBuilder()
                                                .WithTitle("📨 A new vent response has arrived.")
                                                .WithDescription("> " + args.Values["response"])
                                                .WithColor(new DiscordColor(3092790));

                                            await ctx.Member.SendMessageAsync(embed);
                                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                .WithContent("✅ Your response has been sent.")
                                                .AsEphemeral());

                                            responseCache.Add(new(args.Interaction.User, args.Values["response"]));
                                        }
                                        catch (UnauthorizedException)
                                        {
                                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                                .WithContent("⚠️ The user doesn't have their DMs turned on.")
                                                .AsEphemeral());
                                        }
                                    }
                                };
                            }
                            else
                            {
                                await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                    .WithContent("⚠️ This user doesn't allow people to respond to their vents.")
                                    .AsEphemeral());
                            }
                        }
                        else if (args.Id == moderateButton.CustomId)
                        {
                            var moderator = args.User as DiscordMember;

                            if (ctx.Guild.GetGuildAccount(Data).VentModerators.Contains(moderator.Id) || moderator.Permissions.HasPermission(Permissions.Administrator) || moderator.IsOwner)
                            {
                                var embed = new DiscordEmbedBuilder()
                                    .WithTitle($"Vent from {ctx.User.Username}")
                                    .WithThumbnail(ctx.User.AvatarUrl)
                                    .WithDescription($"**Anonymous:** {(anonymous ? "Enabled" : "Disabled")}\n**Trigger Warning:** {(tw ? "Enabled" : "Disabled")}\n**Responses:** {responseCache.Count}")
                                    .WithColor(new DiscordColor(3092790));

                                responseCache.ForEach(x => embed.AddField("Response from " + x.creator.Username, "> " + x.response));

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
                }
            };
        }
    }
}