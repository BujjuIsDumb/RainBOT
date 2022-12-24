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

using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The user configuration module.
    /// </summary>
    [SlashCommandGroup("user", "Configure user-side settings.")]
    [SlashUserBannable]
    public class User : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /user config command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="setting">The setting option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("configure", "Configure user-side settings.")]
        public async Task UserConfigureAsync(InteractionContext ctx,
            [Choice("Allow Vent Responses", 0)]
            [Choice("Bio Style", 1)]
            [Option("setting", "The setting to manage.")] long setting)
        {
            if (setting == 0)
            {
                var allowVentResponsesSelect = new DiscordSelectComponent($"allowVentResponsesSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an option", new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("Yes", "yes", "Allow users to respond to your vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                    new DiscordSelectComponentOption("No", "no", "Do not allow users to respond to your vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                });

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please select whether or not you want people to respond to your vents.")
                    .AddComponents(allowVentResponsesSelect)
                    .AsEphemeral());

                // Respond to select menu input.
                ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                {
                    if (args.Id == allowVentResponsesSelect.CustomId)
                    {
                        ctx.User.GetUserData(Data).AllowVentResponses = args.Values.First() == "yes";
                        Data.Update();

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("✅ The setting has been set.")
                            .AsEphemeral());
                    }
                };
            }
            else if (setting == 1)
            {
                var bioStyleSelect = new DiscordSelectComponent($"bioStyleSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select an option", new List<DiscordSelectComponentOption>()
                {
                    new DiscordSelectComponentOption("None", "none", "Don't use a color for your bio.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("❌"))),
                    new DiscordSelectComponentOption("Red", "red", "Make your bio color red.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔴"))),
                    new DiscordSelectComponentOption("Orange", "orange", "Make your bio orange.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🟠"))),
                    new DiscordSelectComponentOption("Yellow", "yellow", "Make your bio yellow.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🟡"))),
                    new DiscordSelectComponentOption("Green", "green", "Make your bio green.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🟢"))),
                    new DiscordSelectComponentOption("Blue", "blue", "Make your bio blue.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔵"))),
                    new DiscordSelectComponentOption("Purple", "purple", "Make your bio purple.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🟣"))),
                    new DiscordSelectComponentOption("Black", "black", "Make your bio black.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚫"))),
                    new DiscordSelectComponentOption("White", "white", "Make your bio white.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("⚪"))),
                    new DiscordSelectComponentOption("Custom", "custom", "Choose a custom color", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔘")))
                });

                await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                    .WithContent("Please select the bio color you want.")
                    .AddComponents(bioStyleSelect)
                    .AsEphemeral());

                // Respond to select menu input.
                ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                {
                    if (args.Id == bioStyleSelect.CustomId)
                    {
                        if (args.Values.First() == "custom")
                        {
                            // Build modal.
                            var colorModal = new DiscordInteractionResponseBuilder()
                                .WithTitle("Choose a color")
                                .WithCustomId($"colorModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                                .AddComponents(new TextInputComponent(label: "Color (Hexadecimal)", customId: "color", placeholder: "#2F3136", style: TextInputStyle.Short, min_length: 7, max_length: 7));

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, colorModal);

                            // Respond to modal submission.
                            ctx.Client.ModalSubmitted += async (sender, args) =>
                            {
                                if (args.Interaction.Data.CustomId == colorModal.CustomId)
                                {
                                    if (Regex.IsMatch(args.Values["color"], @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
                                    {
                                        ctx.User.GetUserData(Data).BioStyle = args.Values["color"].ToUpper()[1..];
                                        Data.Update();

                                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                            .WithContent("✅ The setting has been set.")
                                            .AsEphemeral());
                                    }
                                    else
                                    {
                                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                            .WithContent("⚠ That is not a valid hex code. Try the [hex color picker](https://g.co/kgs/eJtdm4).")
                                            .AsEphemeral());
                                    }
                                }
                            };
                        }
                        else
                        {
                            switch (args.Values.First())
                            {
                                case "none": ctx.User.GetUserData(Data).BioStyle = "2F3136"; break;
                                case "red": ctx.User.GetUserData(Data).BioStyle = "E91E63"; break;
                                case "orange": ctx.User.GetUserData(Data).BioStyle = "E67E22"; break;
                                case "yellow": ctx.User.GetUserData(Data).BioStyle = "F1C40F"; break;
                                case "green": ctx.User.GetUserData(Data).BioStyle = "2ECC71"; break;
                                case "blue": ctx.User.GetUserData(Data).BioStyle = "3498DB"; break;
                                case "purple": ctx.User.GetUserData(Data).BioStyle = "9B59B6"; break;
                                case "black": ctx.User.GetUserData(Data).BioStyle = "202225"; break;
                                case "white": ctx.User.GetUserData(Data).BioStyle = "FFFFFF"; break;
                            }

                            Data.Update();

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("✅ The setting has been set.")
                                .AsEphemeral());
                        }
                    }
                };
            }
        }
    }

    /// <summary>
    ///     The server configuration module.
    /// </summary>
    [SlashCommandGroup("server", "Configure server-side settings.")]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    [GuildOnly]
    public class Server : ApplicationCommandModule
    {
        /// <summary>
        ///     The database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /server config command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="setting">The setting option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("config", "Configure server-side settings.")]
        public async Task ServerConfigureAsync(InteractionContext ctx,
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

                        ctx.Guild.GetGuildData(Data).VentModerators = selectedUsers.ToArray();
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
                        ctx.Guild.GetGuildData(Data).AnonymousVenting = args.Values.First() == "yes";
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
                        ctx.Guild.GetGuildData(Data).DeleteVerificationRequests = args.Values.First() == "yes";
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
                        ctx.Guild.GetGuildData(Data).CreateVettingThread = args.Values.First() == "yes";
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

                        ctx.Guild.GetGuildData(Data).VerificationFormQuestions = formQuestions.ToArray();
                        Data.Update();

                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent("✅ The setting has been set.")
                            .AsEphemeral());
                    }
                };
            }
        }
    }
}