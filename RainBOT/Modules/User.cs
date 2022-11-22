﻿// This file is from RainBOT (https://github.com/BujjuIsDumb/RainBOT)
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
using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Newtonsoft.Json;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("user", "Manage your user account.")]
    public class User : ApplicationCommandModule
    {
        public RbService Service { private get; set; }

        [SlashCommand("register", "Create a user account.")]
        [SlashUserBannable]
        public async Task UserRegisterAsync(InteractionContext ctx)
        {
            if (!Service.Data.UserAccounts.Exists(x => x.UserId == ctx.User.Id))
            {
                Service.Data.UserAccounts.Add(new UserAccountData()
                {
                    UserId = ctx.User.Id
                });
                Service.Data.Update();

                await ctx.CreateResponseAsync("✅ Created a user account.", true);
            }
            else
            {
                await ctx.CreateResponseAsync("⚠️ You already have an account.", true);
            }
        }

        [SlashCommand("settings", "Manage the settings of your user account.")]
        [SlashRequireUserAccount]
        public async Task UserSettingsAsync(InteractionContext ctx)
        {
            // Build main menu components.
            var mainEmbed = new DiscordEmbedBuilder()
                .WithTitle("User Settings")
                .WithThumbnail(ctx.User.AvatarUrl)
                .WithDescription("Manage the settings of your RainBOT user account. Please select a module to configure.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Allow Vent Responses", Service.GetUserAccount(ctx).AllowVentResponses ? "Enabled" : "Disabled")
                .AddField("Bio Style", "#" + Service.GetUserAccount(ctx).BioStyle);

            var ventingButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("ventingButton"), "Venting", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("👁‍🗨")));
            var bioButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("bioButton"), "Bio", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("📜")));

            var mainResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(mainEmbed)
                .AddComponents(ventingButton, bioButton)
                .AsEphemeral();

            // Build venting menu components.
            var ventingEmbed = new DiscordEmbedBuilder()
                .WithAuthor(name: "User Settings", iconUrl: ctx.User.AvatarUrl)
                .WithTitle("Venting Settings")
                .WithDescription("Configure the venting system.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Allow Vent Responses", Service.GetUserAccount(ctx).AllowVentResponses ? "Enabled" : "Disabled");

            var allowVentResponsesButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("allowVentResponsesButton"), "Allow Vent Responses", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("💬")));

            var ventingResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(ventingEmbed)
                .AddComponents(allowVentResponsesButton)
                .AsEphemeral();

            // Build bio menu components.
            var bioEmbed = new DiscordEmbedBuilder()
                .WithAuthor(name: "User Settings", iconUrl: ctx.User.AvatarUrl)
                .WithTitle("Bio Settings")
                .WithDescription("Configure the bio system.")
                .WithColor(new DiscordColor(3092790))
                .AddField("Bio Style", "#" + Service.GetUserAccount(ctx).BioStyle);

            var bioStyleButton = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("bioStyleButton"), "Bio Style", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🎨")));

            var bioResponse = new DiscordInteractionResponseBuilder()
                .AddEmbed(bioEmbed)
                .AddComponents(bioStyleButton)
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
                        if (args.Id == allowVentResponsesButton.CustomId)
                        {
                            await ventingInteraction.DeleteOriginalResponseAsync();

                            var allowVentResponsesSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("allowVentResponsesSelect"), "Select an option", new List<DiscordSelectComponentOption>()
                            {
                                new DiscordSelectComponentOption("Yes", "yes", "Allow users to respond to your vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅"))),
                                new DiscordSelectComponentOption("No", "no", "Do not allow users to respond to your vents.", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🚫")))
                            });

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("Please select whether or not you want people to respond to your vents.")
                                .AddComponents(allowVentResponsesSelect)
                                .AsEphemeral());

                            // Respond to select menu input.
                            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                            {
                                if (args.Id == allowVentResponsesSelect.CustomId)
                                {
                                    Service.GetUserAccount(ctx).AllowVentResponses = args.Values.First() == "yes";
                                    Service.Data.Update();

                                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                        .WithContent("✅ The setting has been set.")
                                        .AsEphemeral());
                                }
                            };
                        }
                    };
                }
                else if (args.Id == bioButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, bioResponse);

                    var bioInteraction = args.Interaction;

                    // Respond to button input.
                    ctx.Client.ComponentInteractionCreated += async (sender, args) =>
                    {
                        if (args.Id == bioStyleButton.CustomId)
                        {
                            await bioInteraction.DeleteOriginalResponseAsync();

                            var bioStyleSelect = new DiscordSelectComponent(Core.Utilities.CreateCustomId("bioStyleSelect"), "Select an option", new List<DiscordSelectComponentOption>()
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

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
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
                                            .WithCustomId(Core.Utilities.CreateCustomId("colorModal"))
                                            .AddComponents(new TextInputComponent(label: "Color (Hexidecimal)", customId: "color", placeholder: "#2F3136", style: TextInputStyle.Short, min_length: 7, max_length: 7));

                                        await args.Interaction.CreateResponseAsync(InteractionResponseType.Modal, colorModal);

                                        // Respond to modal submission.
                                        ctx.Client.ModalSubmitted += async (sender, args) =>
                                        {
                                            if (args.Interaction.Data.CustomId == colorModal.CustomId)
                                            {
                                                if (Regex.IsMatch(args.Values["color"], @"^#(?:[0-9a-fA-F]{3}){1,2}$"))
                                                {
                                                    Service.GetUserAccount(ctx).BioStyle = args.Values["color"].ToUpper().Substring(1);
                                                    Service.Data.Update();

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
                                            case "none": Service.GetUserAccount(ctx).BioStyle = "2F3136"; break;
                                            case "red": Service.GetUserAccount(ctx).BioStyle = "E91E63"; break;
                                            case "orange": Service.GetUserAccount(ctx).BioStyle = "E67E22"; break;
                                            case "yellow": Service.GetUserAccount(ctx).BioStyle = "F1C40F"; break;
                                            case "green": Service.GetUserAccount(ctx).BioStyle = "2ECC71"; break;
                                            case "blue": Service.GetUserAccount(ctx).BioStyle = "3498DB"; break;
                                            case "purple": Service.GetUserAccount(ctx).BioStyle = "9B59B6"; break;
                                            case "black": Service.GetUserAccount(ctx).BioStyle = "202225"; break;
                                            case "white": Service.GetUserAccount(ctx).BioStyle = "FFFFFF"; break;
                                        }
                                        Service.Data.Update();

                                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                            .WithContent("✅ The setting has been set.")
                                            .AsEphemeral());
                                    }
                                }
                            };
                        }
                    };
                }
            };
        }

        [SlashCommand("data", "Request your user data.")]
        public async Task UserDataAsync(InteractionContext ctx)
        {
            var builder = new StringBuilder();

            foreach (var userAccount in Service.Data.UserAccounts.FindAll(x => x.UserId == ctx.User.Id)) builder.AppendLine($"**User Account**\n```json\n{JsonConvert.SerializeObject(userAccount, Formatting.Indented)}```");
            foreach (var report in Service.Data.Reports.FindAll(x => x.UserId == ctx.User.Id || x.CreatorUserId == ctx.User.Id)) builder.AppendLine($"**Report**\n```json\n{JsonConvert.SerializeObject(report, Formatting.Indented)}```");
            foreach (var userBan in Service.Data.UserBans.FindAll(x => x.UserId == ctx.User.Id)) builder.AppendLine($"**User Ban**\n```json\n{JsonConvert.SerializeObject(userBan, Formatting.Indented)}```");
            if (builder.Length == 0) builder.Append("There is no data associated with this user.");

            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                .WithAuthor(ctx.User.Username, null, ctx.User.AvatarUrl)
                .WithTitle("Your data")
                .WithDescription(builder.ToString())
                .WithColor(new DiscordColor(3092790)), true);
        }
    }
}