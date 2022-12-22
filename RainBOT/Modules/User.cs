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
using Newtonsoft.Json;
using RainBOT.Core;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("user", "Manage your user account.")]
    public class User : ApplicationCommandModule
    {
        public Database Data { private get; set; }

        [SlashCommand("settings", "Manage the settings of your user account.")]
        public async Task UserSettingsAsync(InteractionContext ctx,
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
                        ctx.User.GetUserAccount(Data).AllowVentResponses = args.Values.First() == "yes";
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
                                        ctx.User.GetUserAccount(Data).BioStyle = args.Values["color"].ToUpper()[1..];
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
                                case "none": ctx.User.GetUserAccount(Data).BioStyle = "2F3136"; break;
                                case "red": ctx.User.GetUserAccount(Data).BioStyle = "E91E63"; break;
                                case "orange": ctx.User.GetUserAccount(Data).BioStyle = "E67E22"; break;
                                case "yellow": ctx.User.GetUserAccount(Data).BioStyle = "F1C40F"; break;
                                case "green": ctx.User.GetUserAccount(Data).BioStyle = "2ECC71"; break;
                                case "blue": ctx.User.GetUserAccount(Data).BioStyle = "3498DB"; break;
                                case "purple": ctx.User.GetUserAccount(Data).BioStyle = "9B59B6"; break;
                                case "black": ctx.User.GetUserAccount(Data).BioStyle = "202225"; break;
                                case "white": ctx.User.GetUserAccount(Data).BioStyle = "FFFFFF"; break;
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

        [SlashCommand("data", "Request your user data.")]
        public async Task UserDataAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Your data")
                .WithColor(new DiscordColor(3092790));

            foreach (var userAccount in Data.Users.FindAll(x => x.UserId == ctx.User.Id)) embed.AddField("User Account", $"```json\n{JsonConvert.SerializeObject(userAccount, Formatting.Indented)}```");
            foreach (var report in Data.Reports.FindAll(x => x.UserId == ctx.User.Id || x.CreatorUserId == ctx.User.Id)) embed.AddField("Report", $"```json\n{JsonConvert.SerializeObject(report, Formatting.Indented)}```");
            foreach (var userBan in Data.Bans.FindAll(x => x.UserId == ctx.User.Id)) embed.AddField("User Ban", $"```json\n{JsonConvert.SerializeObject(userBan, Formatting.Indented)}```");
            if (embed.Fields.Count == 0) embed.WithDescription("There is no data associated with this user.");

            await ctx.CreateResponseAsync(embed, true);
        }
    }
}