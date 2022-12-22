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
using Newtonsoft.Json;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Services;
using RainBOT.Core.Services.Models;

namespace RainBOT.Modules
{
    [SlashCommandGroup("server", "Manage your server account.", false)]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    [GuildOnly]
    public class Server : ApplicationCommandModule
    {
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
        public async Task ServerSettingsAsync(InteractionContext ctx,
            [Choice("Vent Moderators", 0)]
            [Choice("Anonymous Venting", 1)]
            [Choice("Delete Verification Requests", 2)]
            [Choice("Create Vetting Thread", 3)]
            [Choice("Verification Form", 4)]
            [Option("setting", "The setting to manage.")] long setting)
        {
            if (setting == 0)
            {
                var ventModeratorsSelect = new DiscordUserSelectComponent($"ventModeratorsSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select users", false, 1, 25);

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
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
            else if (setting == 1)
            {
                var anonymousVentingSelect = new DiscordSelectComponent($"anonymousVentingSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an option", new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("Yes", "yes", "Allow users to create anonymous vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                    new DiscordSelectComponentOption("No", "no", "Do not allow users to create anonymous vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                });

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
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
            else if (setting == 2)
            {
                var deleteVerificationRequestsSelect = new DiscordSelectComponent($"deleteVerificationRequestsSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an option", new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("Yes", "yes", "Delete verification requests after they're accepted/denied.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                    new DiscordSelectComponentOption("No", "no", "Do not delete verification requests after they're accepted/denied.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                });

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
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
            else if (setting == 3)
            {
                var createVettingThreadSelect = new DiscordSelectComponent($"createVettingThreadSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an option", new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("Yes", "yes", "Create vetting threads for verification requests.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                    new DiscordSelectComponentOption("No", "no", "Do not create vetting threads for verification requests.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                });

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
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
            else if (setting == 4)
            {
                // Build modal.
                var verificationFormModal = new DiscordInteractionResponseBuilder()
                    .WithTitle("Create the verification form")
                    .WithCustomId($"verificationFormModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                    .AddComponents(new TextInputComponent(label: "Question 1", customId: "question1", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                    .AddComponents(new TextInputComponent(label: "Question 2", customId: "question2", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                    .AddComponents(new TextInputComponent(label: "Question 3", customId: "question3", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30))
                    .AddComponents(new TextInputComponent(label: "Question 4", customId: "question4", required: false, style: TextInputStyle.Short, min_length: 5, max_length: 30));

                await ctx.CreateResponseAsync(InteractionResponseType.Modal, verificationFormModal);

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
        }

        [SlashCommand("data", "Request your server data.")]
        public async Task ServerDataAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Your data")
                .WithColor(new DiscordColor(3092790));

            foreach (var guildAccount in Data.GuildAccounts.FindAll(x => x.GuildId == ctx.Guild.Id)) embed.AddField("Server Account", $"```json\n{JsonConvert.SerializeObject(guildAccount, Formatting.Indented)}```");
            foreach (var guildBan in Data.GuildBans.FindAll(x => x.GuildId == ctx.Guild.Id)) embed.AddField("Server Ban", $"```json\n{JsonConvert.SerializeObject(guildBan, Formatting.Indented)}```");
            if (embed.Fields.Count == 0) embed.WithDescription("There is no data associated with this user.");

            await ctx.CreateResponseAsync(embed, true);
        }
    }
}