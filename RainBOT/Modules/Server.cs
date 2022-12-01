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

using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("server", "Manage your server account.", false)]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    [GuildOnly]
    public class Server : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("register", "Create a server account.")]
        [SlashGuildBannable]
        public async Task ServerRegisterAsync(InteractionContext ctx)
        {
            if (!Data.GuildAccounts.Exists(x => x.GuildId == ctx.Guild.Id))
            {
                Data.GuildAccounts.Add(new GuildAccountData()
                {
                    GuildId = ctx.Guild.Id
                });
                Data.Update();

                await ctx.CreateResponseAsync("✅ Created a server account.", true);
            }
            else
            {
                await ctx.CreateResponseAsync("⚠️ This server is already registered.", true);
            }
        }

        [SlashCommand("settings", "Manage the settings of your server account.")]
        [SlashRequireGuildAccount]
        public async Task ServerSettingsAsync(InteractionContext ctx)
        {
            // Format list settings.
            var ventModerators = new StringBuilder();
            ctx.Guild.GetGuildAccount(Data).VentModerators.ToList().ForEach(async x => ventModerators.AppendLine((await ctx.Client.GetUserAsync(x)).Mention));
            if (string.IsNullOrEmpty(ventModerators.ToString())) ventModerators.Append("None");

            var verificationForm = new StringBuilder();
            ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions.ToList().ForEach(x => verificationForm.AppendLine(x));
            if (string.IsNullOrEmpty(verificationForm.ToString())) verificationForm.Append("None");

            // Build main menu components.
            var mainEmbed = new DiscordEmbedBuilder()
                .WithTitle("Server Settings")
                .WithThumbnail(ctx.Guild.IconUrl)
                .WithDescription("Manage the settings of your RainBOT server account. Please select a module to configure.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Vent Moderators", ventModerators.ToString())
                .AddField("Anonymous Venting", ctx.Guild.GetGuildAccount(Data).AnonymousVenting ? "Enabled" : "Disabled")
                .AddField("Delete Verification Requests", ctx.Guild.GetGuildAccount(Data).DeleteVerificationRequests ? "Enabled" : "Disabled")
                .AddField("Create Vetting Thread", ctx.Guild.GetGuildAccount(Data).CreateVettingThread ? "Enabled" : "Disabled")
                .AddField("Verification Form", verificationForm.ToString());

            var ventingButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("ventingButton"), "Venting", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👁‍🗨")));
            var verificationButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("verificationButton"), "Verification", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));

            var mainResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(mainEmbed)
                .AddComponents(ventingButton, verificationButton)
                .AsEphemeral();

            // Build venting menu components.
            var ventingEmbed = new DiscordEmbedBuilder()
                .WithAuthor("Server Settings", null, ctx.Guild.IconUrl)
                .WithTitle("Venting Settings")
                .WithDescription("Configure the venting system.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Vent Moderators", ventModerators.ToString())
                .AddField("Anonymous Venting", ctx.Guild.GetGuildAccount(Data).AnonymousVenting ? "Enabled" : "Disabled");

            var ventModeratorsButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("ventModeratorsButton"), "Vent Moderators", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🛡")));
            var anonymousVentingButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("anonymousVentingButton"), "Allow Anonymous Vents", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("😷")));

            var ventingResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(ventingEmbed)
                .AddComponents(ventModeratorsButton, anonymousVentingButton)
                .AsEphemeral();

            // Build verification menu components.
            var verificationEmbed = new DiscordEmbedBuilder()
                .WithAuthor("Server Settings", null, ctx.Guild.IconUrl)
                .WithTitle("Verification Settings")
                .WithDescription("Configure the verification system.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Delete Verification Requests", ctx.Guild.GetGuildAccount(Data).DeleteVerificationRequests ? "Enabled" : "Disabled")
                .AddField("Create Vetting Thread", ctx.Guild.GetGuildAccount(Data).CreateVettingThread ? "Enabled" : "Disabled")
                .AddField("Verification Form", verificationForm.ToString());

            var deleteVerificationRequestsButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("deleteVerificationRequestsButton"), "Delete Verification Requests", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🗑")));
            var createVettingThreadButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("createVettingThreadButton"), "Create Vetting Thread", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚖️")));
            var verificationFormButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("verificationFormButton"), "Verification Form", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("❓")));

            var verificationResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(verificationEmbed)
                .AddComponents(deleteVerificationRequestsButton, createVettingThreadButton, verificationFormButton)
                .AsEphemeral();

            // Send main menu message.
            await ctx.CreateResponseAsync(mainResponse);

            // Respond to button input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == ventingButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, ventingResponse);

                    var ventingInteraction = args.Interaction;

                    // Respond to button input.
                    ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                    {
                        if (args.Id == ventModeratorsButton.CustomId)
                        {
                            await ventingInteraction.DeleteOriginalResponseAsync();

                            var ventModeratorsSelect = new DiscordUserSelectComponent(Core.Utilities.CreateCustomId("ventModeratorsSelect"), "Select users", false, 1, 25);

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select the people who you want to be able to moderate anonymous vents.")
                                .AddComponents(ventModeratorsSelect)
                                .AsEphemeral());

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == ventModeratorsSelect.CustomId)
                                {
                                    var selectedUsers = new List<ulong>();
                                    foreach (string userId in args.Values)
                                    {
                                        selectedUsers.Add(Convert.ToUInt64(userId));
                                    }

                                    ctx.Guild.GetGuildAccount(Data).VentModerators = selectedUsers.ToArray();
                                    Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                        else if (args.Id == anonymousVentingButton.CustomId)
                        {
                            await ventingInteraction.DeleteOriginalResponseAsync();

                            var anonymousVentingSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("anonymousVentingSelect"), "Select an option", new List<DiscordSelectComponentOption>()
                            {
                                new DiscordSelectComponentOption("Yes", "yes", "Allow users to create anonymous vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                                new DiscordSelectComponentOption("No", "no", "Do not allow users to create anonymous vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                            });

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select whether or not you want people to be able to create anonymous vents.")
                                .AddComponents(anonymousVentingSelect)
                                .AsEphemeral());

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == anonymousVentingSelect.CustomId)
                                {
                                    ctx.Guild.GetGuildAccount(Data).AnonymousVenting = args.Values.First() == "yes";
                                    Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                    };
                }
                else if (args.Id == verificationButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, verificationResponse);

                    var verificationInteraction = args.Interaction;

                    // Respond to button input.
                    ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                    {
                        if (args.Id == deleteVerificationRequestsButton.CustomId)
                        {
                            await verificationInteraction.DeleteOriginalResponseAsync();

                            var deleteVerificationRequestsSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("deleteVerificationRequestsSelect"), "Select an option", new List<DiscordSelectComponentOption>()
                            {
                                new DiscordSelectComponentOption("Yes", "yes", "Delete verification requests after they're accepted/denied.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                                new DiscordSelectComponentOption("No", "no", "Do not delete verification requests after they're accepted/denied.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                            });

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select whether or not to delete verification requests after they're accepted/denied.")
                                .AddComponents(deleteVerificationRequestsSelect)
                                .AsEphemeral());

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == deleteVerificationRequestsSelect.CustomId)
                                {
                                    ctx.Guild.GetGuildAccount(Data).DeleteVerificationRequests = args.Values.First() == "yes";
                                    Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                        else if (args.Id == createVettingThreadButton.CustomId)
                        {
                            await verificationInteraction.DeleteOriginalResponseAsync();

                            var createVettingThreadSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("createVettingThreadSelect"), "Select an option", new List<DiscordSelectComponentOption>()
                            {
                                new DiscordSelectComponentOption("Yes", "yes", "Create vetting threads for verification requests.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                                new DiscordSelectComponentOption("No", "no", "Do not create vetting threads for verification requests.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                            });

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select whether or not to create vetting threads for verification requests.")
                                .AddComponents(createVettingThreadSelect)
                                .AsEphemeral());

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == createVettingThreadSelect.CustomId)
                                {
                                    ctx.Guild.GetGuildAccount(Data).CreateVettingThread = args.Values.First() == "yes";
                                    Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                        else if (args.Id == verificationFormButton.CustomId)
                        {
                            await verificationInteraction.DeleteOriginalResponseAsync();

                            // Build modal.
                            var verificationFormModal = new DiscordInteractionResponseBuilder()
                                .WithTitle("Create the verification form")
                                .WithCustomId(Core.Utilities.CreateCustomId("verificationFormModal"))
                                .AddComponents(new TextInputComponent(label: "Question 1", customId: "question1", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                                .AddComponents(new TextInputComponent(label: "Question 2", customId: "question2", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                                .AddComponents(new TextInputComponent(label: "Question 3", customId: "question3", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                                .AddComponents(new TextInputComponent(label: "Question 4", customId: "question4", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30));

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, verificationFormModal);

                            // Respond to modal submission.
                            ctx.Client.ModalSubmitted += async (sender, args) =>
                            {
                                if (args.Interaction.Data.CustomId == verificationFormModal.CustomId)
                                {
                                    var formQuestions = new List<string>();
                                    foreach (string question in args.Values.Values)
                                        if (!string.IsNullOrEmpty(question)) formQuestions.Add(question);

                                    ctx.Guild.GetGuildAccount(Data).VerificationFormQuestions = formQuestions.ToArray();
                                    Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                    };
                }
            };
        }

        [SlashCommand("data", "Request your server data.")]
        public async Task ServerDataAsync(InteractionContext ctx)
        {
            var builder = new StringBuilder();

            foreach (var guildAccount in Data.GuildAccounts.FindAll(x => x.GuildId == ctx.Guild.Id)) builder.AppendLine($"**Server Account**\n```json\n{JsonConvert.SerializeObject(guildAccount, Formatting.Indented)}```");
            foreach (var guildBan in Data.GuildBans.FindAll(x => x.GuildId == ctx.Guild.Id)) builder.AppendLine($"**Server Ban**\n```json\n{JsonConvert.SerializeObject(guildBan, Formatting.Indented)}```");
            if (builder.Length == 0) builder.Append("There is no data associated with this server.");

            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                .WithAuthor(ctx.Guild.Name, null, ctx.Guild.IconUrl)
                .WithTitle("Your data")
                .WithDescription(builder.ToString())
                .WithColor(new DiscordColor(3092790)), true);
        }
    }
}