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
using RainBOT.Core.Attributes;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The verification module.
    /// </summary>
    public class Verification : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /verify command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("verify", "Request verification for the member role.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.CreatePrivateThreads | Permissions.ManageRoles | Permissions.KickMembers | Permissions.BanMembers | Permissions.ManageThreads)]
        [SlashGuildBannable]
        public async Task VerifyAsync(InteractionContext ctx)
        {
            DiscordInteraction originalInteraction = null;
            DiscordInteraction buttonInteraction = null;
            DiscordThreadChannel thread = null;

            // Add form questions.
            Dictionary<string, string> questions = new();
            for (int i = 0; i < ctx.Guild.GetGuildData(Data).VerificationFormQuestions.Length; i++)
                questions.Add($"question{i}", ctx.Guild.GetGuildData(Data).VerificationFormQuestions[i]);

            #region Components
            var verificationFormModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Verification Form")
                .WithCustomId($"verificationFormModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}");

            // Add questions to modal.
            questions.ToList().ForEach(x => verificationFormModal.AddComponents(new TextInputComponent(label: x.Value, customId: x.Key, required: true, style: TextInputStyle.Paragraph, min_length: 5, max_length: 500)));
            verificationFormModal.AddComponents(new TextInputComponent(label: "Notes", customId: "notes", required: false, style: TextInputStyle.Paragraph, min_length: 5, max_length: 500));

            var acceptButton = new DiscordButtonComponent(ButtonStyle.Success, $"acceptButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Accept", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));

            var denyButton = new DiscordButtonComponent(ButtonStyle.Danger, $"denyButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Deny", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⛔")));

            var acceptSelect = new DiscordRoleSelectComponent($"acceptSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select a role.");

            var denySelect = new DiscordSelectComponent($"denySelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an action.", new DiscordSelectComponentOption[]
            {
                new DiscordSelectComponentOption(label: "Kick", value: "kick", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👟"))),
                new DiscordSelectComponentOption(label: "Ban", value: "ban", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔨")))
            });
            #endregion

            #region Event Handlers
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == verificationFormModal.CustomId)
                {
                    if (args.Interaction.Data.CustomId == verificationFormModal.CustomId)
                    {
                        int reports = Data.Reports.FindAll(x => x.UserId == ctx.User.Id).Count;
                        string cmd = (await ctx.Client.GetGlobalApplicationCommandAsync("report")).GetSubcommandMention("list");
                        var embed = new DiscordEmbedBuilder()
                            .WithAuthor(name: ctx.User.Username, iconUrl: ctx.User.AvatarUrl)
                            .WithTitle("📨 A new verification request has arrived!")
                            .WithDescription(reports > 0 ? $"This user has {reports} report{(reports == 1 ? string.Empty : "s")}. ({cmd})" : null)
                            .WithFooter((DateTime.Now - ctx.User.CreationTimestamp.DateTime).TotalDays <= 7 ? "This account is less than a week old." : null)
                            .WithColor(new DiscordColor(6317300));

                        // Add form answers.
                        questions.ToList().ForEach(x => embed.AddField(x.Value, args.Values[x.Key]));
                        if (!string.IsNullOrEmpty(args.Values["notes"])) embed.AddField("Notes", args.Values["notes"]);

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .AddEmbed(embed)
                            .AddComponents(acceptButton, denyButton));

                        originalInteraction = args.Interaction;

                        // Create vetting thread.
                        if (ctx.Guild.GetGuildData(Data).CreateVettingThread)
                        {
                            thread = await ctx.Channel.CreateThreadAsync($"{ctx.User.Username}'s Verification Request", AutoArchiveDuration.Day, ChannelType.PrivateThread, $"Created vetting thread (/verify executed by {ctx.User.Username})");
                            await (await thread.SendMessageAsync($"{ctx.User.Mention}")).DeleteAsync();
                        }
                    }
                }
            };

            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {

                if (args.Id == acceptButton.CustomId)
                {
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Please select a role to give the user.")
                        .AddComponents(acceptSelect)
                        .AsEphemeral());

                    buttonInteraction = args.Interaction;
                }
                else if (args.Id == denyButton.CustomId)
                {
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("Please select how you want to deny the request.")
                        .AddComponents(denySelect)
                        .AsEphemeral());

                    buttonInteraction = args.Interaction;
                }
                else if (args.Id == acceptSelect.CustomId)
                {
                    var user = args.User as DiscordMember;

                    if (user.Permissions.HasPermission(Permissions.ManageRoles) || user.Permissions.HasPermission(Permissions.Administrator) || user.IsOwner)
                    {
                        try
                        {
                            await ctx.Member.GrantRoleAsync(ctx.Guild.GetRole(ulong.Parse(args.Values.First())), $"Accepted verification request (/verify executed by {ctx.User.Username})");
                        }
                        catch (UnauthorizedException)
                        {
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("⚠️ Insufficient permissions!")
                                .AsEphemeral());

                            return;
                        }
                        catch (NotFoundException)
                        {
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("⚠ The user couldn't be found. They may have left the server.")
                                .AsEphemeral());

                            return;
                        }

                        if (ctx.Guild.GetGuildData(Data).DeleteVerificationRequests)
                        {
                            await buttonInteraction.DeleteOriginalResponseAsync();
                            if (thread is not null) await thread.DeleteAsync($"Delete vetting thread (/verify executed by {ctx.User.Username})");
                        }
                        else
                        {
                            var embed = new DiscordEmbedBuilder()
                                .WithTitle("✅ This request has been accepted.")
                                .WithDescription($"Accepted by **{user.Username}**.")
                                .WithColor(new DiscordColor(6317300));

                            await buttonInteraction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

                            // Lock the thread.
                            if (thread is not null)
                            {
                                await thread.ModifyAsync(x =>
                                {
                                    x.IsArchived = true;
                                    x.Locked = true;
                                    x.AuditLogReason = $"Closed vetting thread (/verify executed by {ctx.User.Username})";
                                });
                            }
                        }

                        await buttonInteraction.DeleteOriginalResponseAsync();
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent($"✅ **{ctx.User.Username}** has been given the role!")
                            .AsEphemeral());
                    }
                    else
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("⚠️ You do not have permissions to give roles.")
                            .AsEphemeral());
                    }
                }
                else if (args.Id == denySelect.CustomId)
                {
                    var user = args.User as DiscordMember;

                    if (user.Permissions.HasPermission(args.Values.First() == "kick" ? Permissions.KickMembers : Permissions.BanMembers) || user.Permissions.HasPermission(Permissions.Administrator) || user.IsOwner)
                    {
                        try
                        {
                            if (args.Values.First() == "kick") await ctx.Member.RemoveAsync($"Denied verification request (/verify executed by {ctx.User.Username})");
                            else await ctx.Member.BanAsync(reason: $"Denied verification request (/verify executed by {ctx.User.Username})");
                        }
                        catch (UnauthorizedException)
                        {
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("⚠️ Insufficient permissions!")
                                .AsEphemeral());

                            return;
                        }
                        catch (NotFoundException)
                        {
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("⚠ The user couldn't be found. They may have left the server.")
                                .AsEphemeral());

                            return;
                        }

                        if (ctx.Guild.GetGuildData(Data).DeleteVerificationRequests)
                        {
                            await originalInteraction.DeleteOriginalResponseAsync();
                            if (thread is not null) await thread.DeleteAsync($"Delete vetting thread (/verify executed by {ctx.User.Username})");
                        }
                        else
                        {
                            var embed = new DiscordEmbedBuilder()
                                .WithTitle("⛔ This request has been denied.")
                                .WithDescription($"Denied by **{user.Username}**.")
                                .WithColor(new DiscordColor(6317300));

                            await originalInteraction.EditOriginalResponseAsync(new DiscordWebhookBuilder().AddEmbed(embed));

                            // Lock the thread.
                            if (thread is not null)
                            {
                                await thread.ModifyAsync(x =>
                                {
                                    x.IsArchived = true;
                                    x.Locked = true;
                                    x.AuditLogReason = $"Closed vetting thread (/verify executed by {ctx.User.Username})";
                                });
                            }
                        }

                        await buttonInteraction.DeleteOriginalResponseAsync();
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent($"✅ **{ctx.User.Username}** has been {(args.Values.First() == "kick" ? "kicked" : "banned")}!")
                            .AsEphemeral());
                    }
                    else
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent($"⚠️ You do not have permissions to {args.Values.First()} members.")
                            .AsEphemeral());
                    }
                }
            };
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, verificationFormModal);
        }
    }
}