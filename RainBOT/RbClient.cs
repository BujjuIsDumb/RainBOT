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

using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using RainBOT.Core;
using RainBOT.Core.Services;
using RainBOT.Core.Services.Models;

namespace RainBOT
{
    /// <summary>
    ///     The client used to connect to the bot.
    /// </summary>
    public class RbClient
    {
        /// <summary>
        ///     The configuration.
        /// </summary>
        private Configuration _config;

        /// <summary>
        ///     The Discord client.
        /// </summary>
        private DiscordClient _client;

        /// <summary>
        ///     Starts the bot.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            // Load the configuration.
            _config = Configuration.Load("config.json");

            // Create the Discord client.
            _client = new DiscordClient(new DiscordConfiguration()
            {
                Token = _config.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            });
            _client.MessageCreated += MessageCreated;
            _client.GuildCreated += GuildCreated;

            // Create the slash command service.
            var slash = _client.UseSlashCommands(new SlashCommandsConfiguration()
            {
                Services = new ServiceCollection()
                    .AddTransient(x => new Database("data.json").Initialize())
                    .BuildServiceProvider()
            });
            slash.RegisterCommands(Assembly.GetExecutingAssembly(), _config.GuildId);
            slash.SlashCommandErrored += SlashCommandErrored;

            // Connect to the Discord gateway.
            await _client.ConnectAsync(new DiscordActivity("for pings!", ActivityType.Watching));
            await Task.Delay(-1);
        }

        /// <summary>
        ///     Handles the <see cref="DiscordClient.MessageCreated"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.MentionedUsers.Contains(sender.CurrentUser))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("👋 Get started")
                    .WithDescription($"Welcome to RainBOT! I use slash commands, so you can view all of my commands by typing a `/` symbol.")
                    .WithImageUrl("https://i.imgur.com/sWoBYi6.png")
                    .WithFooter("Hint: Try /info!")
                    .WithColor(new DiscordColor(3092790));

                await args.Message.RespondAsync(embed);
            }
        }

        /// <summary>
        ///     Handles the <see cref="DiscordClient.GuildCreated"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static Task GuildCreated(DiscordClient sender, GuildCreateEventArgs args)
        {
            return Task.Run(() =>
            {
                using var data = new Database("data.json").Initialize();
                if (!data.Guilds.Exists(x => x.GuildId == args.Guild.Id))
                {
                    data.Guilds.Add(new GuildData() { GuildId = args.Guild.Id });
                    data.Update();
                }
            });
        }

        /// <summary>
        ///     Handles the <see cref="SlashCommandsExtension.SlashCommandErrored"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public static async Task SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            if (args.Exception is SlashExecutionChecksFailedException slashExecutionChecksFailedException)
            {
                if (slashExecutionChecksFailedException.FailedChecks[0] is SlashCooldownAttribute slashCooldownAttribute)
                    await args.Context.CreateResponseAsync($"⚠️ This command is on cooldown. (Finished <t:{((DateTimeOffset)DateTime.Now.Add(slashCooldownAttribute.GetRemainingCooldown(args.Context))).ToUnixTimeSeconds()}:R>)", true);
                else if (slashExecutionChecksFailedException.FailedChecks[0] is SlashRequireBotPermissionsAttribute slashRequireBotPermissionsAttribute)
                    await args.Context.CreateResponseAsync($"⚠️ I need `{slashRequireBotPermissionsAttribute.Permissions}` permissions for this command to work.", true);
            }
            else
            {
                await args.Context.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"❌ An unexpected error has occurred. If you think this is a bug, please report it.\n\n```{args.Exception.Message}```")
                    .AddComponents(new DiscordLinkButtonComponent("https://github.com/BujjuIsDumb/RainBOT/issues", "Report"))
                    .AsEphemeral());
            }
        }
    }
}