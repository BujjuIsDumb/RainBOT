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
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    public class Verification : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("verify", "Request verification for the member role.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.CreatePublicThreads | Permissions.ManageRoles | Permissions.KickMembers | Permissions.BanMembers | Permissions.ManageThreads)]
        [SlashRequireGuildAccount]
        public async Task VerifyAsync(InteractionContext ctx)
        {
            // Build modal.
            var verificationFormModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Verification Form")
                .WithCustomId(Core.Utilities.CreateCustomId("verificationFormModal"));

            // Add text input component for every question.
            for (int i = 0; i < ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions.Count(); i++)
            {
                string formQuestion = ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions[i];
                verificationFormModal.AddComponents(new TextInputComponent(label: formQuestion, customId: $"question{i}", required: true, style: TextInputStyle.Paragraph, min_length: 5, max_length: 500));
            }

            verificationFormModal.AddComponents(new TextInputComponent(label: "Notes", customId: "notes", required: false, style: TextInputStyle.Paragraph, min_length: 5, max_length: 500));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, verificationFormModal);

            // Respond to modal input.
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == verificationFormModal.CustomId)
                {
                    int reports = Data.Reports.FindAll(x => x.UserId == ctx.User.Id).Count;
                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: ctx.User.Username, iconUrl: ctx.User.AvatarUrl)
                        .WithTitle("📨 A new verification request has arrived!")
                        .WithDescription($"{(reports > 0 ? $"This user has {reports} report{(reports == 1 ? string.Empty : "s")}. ({Core.Utilities.GetCommandMention(ctx.Client, "report list")})" : string.Empty)}")
                        .WithFooter((DateTime.Now - ctx.User.CreationTimestamp.DateTime).TotalDays <= 7 ? "This account is less than a week old." : null)
                        .WithColor(new DiscordColor(3092790));

                    // Add field for every question.
                    for (int i = 0; i < ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions.Count(); i++)
                    {
                        string formQuestion = ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions[i];
                        embed.AddField(formQuestion, args.Values[$"question{i}"]);
                    }

                    // Add notes field.
                    if (!string.IsNullOrEmpty(args.Values["notes"]))
                    {
                        embed.AddField("Notes", args.Values["notes"]);
                    }

                    var acceptButton = new DiscordButtonComponent(ButtonStyle.Success, Core.Utilities.CreateCustomId("acceptButton"), "Accept", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));
                    var denyButton = new DiscordButtonComponent(ButtonStyle.Danger, Core.Utilities.CreateCustomId("denyButton"), "Deny", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⛔")));

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AddComponents(acceptButton, denyButton));

                    // Create vetting thread.
                    DiscordThreadChannel thread = null;
                    if (ctx.Guild.GetGuildAccount(Data).CreateVettingThread)
                    {
                        thread = await ctx.Channel.CreateThreadAsync($"{ctx.User.Username}'s Verification Request", AutoArchiveDuration.Day, ChannelType.PrivateThread, $"Created vetting thread (/verify executed by {ctx.User.Username})");
                        await (await thread.SendMessageAsync($"{ctx.User.Mention}")).DeleteAsync();
                    }

                    var originalInteraction = args.Interaction;

                    // Respond to button input.
                    ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                    {
                        if (args.Id == acceptButton.CustomId)
                        {
                            var acceptSelect = new DiscordRoleSelectComponent(Core.Utilities.CreateCustomId("acceptSelect"), "Select a role.");

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select a role to give the user.")
                                .AddComponents(acceptSelect)
                                .AsEphemeral());

                            var acceptInteraction = args.Interaction;

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == acceptSelect.CustomId)
                                {
                                    var moderator = args.User as DiscordMember;

                                    if (moderator.Permissions.HasPermission(Permissions.ManageRoles) || moderator.Permissions.HasPermission(Permissions.Administrator) || moderator.IsOwner)
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

                                        if (ctx.Guild.GetGuildAccount(Data).DeleteVerificationRequests)
                                        {
                                            await originalInteraction.DeleteOriginalResponseAsync();
                                            if (thread is not null) await thread.DeleteAsync($"Delete vetting thread (/verify executed by {ctx.User.Username})");
                                        }
                                        else
                                        {
                                            var embed = new DiscordEmbedBuilder()
                                                .WithTitle("✅ This request has been accepted.")
                                                .WithDescription($"Accepted by **{moderator.Username}**.")
                                                .WithColor(new DiscordColor(3092790));

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

                                        await acceptInteraction.DeleteOriginalResponseAsync();
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
                            };
                        }
                        else if (args.Id == denyButton.CustomId)
                        {
                            var denySelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("denySelect"), "Select an action.", new DiscordSelectComponentOption[]
                            {
                                new DiscordSelectComponentOption(label: "Kick", value: "kick", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👟"))),
                                new DiscordSelectComponentOption(label: "Ban", value: "ban", emoji: new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔨")))
                            });

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select how you want to deny the request.")
                                .AddComponents(denySelect)
                                .AsEphemeral());

                            var denyInteraction = args.Interaction;

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == denySelect.CustomId)
                                {
                                    var moderator = args.User as DiscordMember;

                                    if (moderator.Permissions.HasPermission(args.Values.First() == "kick" ? Permissions.KickMembers : Permissions.BanMembers) || moderator.Permissions.HasPermission(Permissions.Administrator) || moderator.IsOwner)
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

                                        if (ctx.Guild.GetGuildAccount(Data).DeleteVerificationRequests)
                                        {
                                            await originalInteraction.DeleteOriginalResponseAsync();
                                            if (thread is not null) await thread.DeleteAsync($"Delete vetting thread (/verify executed by {ctx.User.Username})");
                                        }
                                        else
                                        {
                                            var embed = new DiscordEmbedBuilder()
                                                .WithTitle("⛔ This request has been denied.")
                                                .WithDescription($"Denied by **{moderator.Username}**.")
                                                .WithColor(new DiscordColor(3092790));

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

                                        await denyInteraction.DeleteOriginalResponseAsync();
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
                        }
                    };
                }
            };
        }
    }
}