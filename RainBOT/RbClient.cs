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

using System.Reflection;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.EventHandling;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using RainBOT.Core;
using RainBOT.Core.Entities.Services;

namespace RainBOT
{
    public class RbClient
    {
        private Config _config;

        private DiscordShardedClient _client;
        
        private async Task InitializeAsync()
        {
            _config = new Config("config.json").Initialize();

            // Setup client.
            _client = new DiscordShardedClient(new DiscordConfiguration()
            {
                Token = _config.Token,
                TokenType = TokenType.Bot
            });
            _client.MessageCreated += Events.MessageCreated;

            // Setup slash commands.
            var slash = await _client.UseSlashCommandsAsync(new SlashCommandsConfiguration()
            {
                Services = new ServiceCollection()
                    .AddTransient(x => new Config("config.json").Initialize())
                    .AddTransient(x => new Data("data.json").Initialize())
                    .BuildServiceProvider()
            });

            // Use interactivity for pagination.
            await _client.UseInteractivityAsync(new InteractivityConfiguration()
            {
                AckPaginationButtons = true,
                ButtonBehavior = ButtonPaginationBehavior.Disable,
                PaginationButtons = new PaginationButtons()
                {
                    SkipLeft = new DiscordButtonComponent(ButtonStyle.Primary, Core.Utilities.CreateCustomId("first"), "First"),
                    Left = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("back"), "Back"),
                    Stop = new DiscordButtonComponent(ButtonStyle.Danger, Core.Utilities.CreateCustomId("lock"), string.Empty, false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("🔒"))),
                    Right = new DiscordButtonComponent(ButtonStyle.Secondary, Core.Utilities.CreateCustomId("next"), "Next"),
                    SkipRight = new DiscordButtonComponent(ButtonStyle.Primary, Core.Utilities.CreateCustomId("last"), "Last")
                }
            });

            // Register commands.
            foreach (var extension in slash.Values)
            {
                extension.RegisterCommands(Assembly.GetExecutingAssembly(), _config.GuildId);
                extension.SlashCommandErrored += Events.SlashCommandErrored;
            }

            // Start bot.
            await _client.StartAsync();
            _client.Ready += async (sender, args) => await _client.UpdateStatusAsync(new DiscordActivity(_config.Status, _config.StatusType));
            await Task.Delay(-1);
        }

        public void Initialize() => InitializeAsync().GetAwaiter().GetResult();
    }
}