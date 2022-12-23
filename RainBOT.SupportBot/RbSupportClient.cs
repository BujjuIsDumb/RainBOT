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
using DSharpPlus.SlashCommands.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using RainBOT.SupportBot.Core;
using RainBOT.SupportBot.Core.Services;

namespace RainBOT.SupportBot
{
    /// <summary>
    ///     The client used to connect to the bot.
    /// </summary>
    public class RbSupportClient
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
        private static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.MentionedUsers.Contains(sender.CurrentUser))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("🌈 RainBOT Support")
                    .WithDescription("This bot was designed for the RainBOT support server. It has commands that create responses with answers to common questions.")
                    .WithColor(new DiscordColor(3092790));

                await args.Message.RespondAsync(embed);
            }
        }

        /// <summary>
        ///     Handles the <see cref="SlashCommandsExtension.SlashCommandErrored"/> event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The args.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private static async Task SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            await args.Context.CreateResponseAsync($"❌ An unexpected error has occurred.\n\n```{args.Exception.Message}```", true);
        }
    }
}