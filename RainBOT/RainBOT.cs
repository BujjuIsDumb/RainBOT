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
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using RainBOT.Core;
using RainBOT.Core.Entities.Services;

namespace RainBOT
{
    public class RainBOT
    {
        private async Task InitializeAsync()
        {
            using (var config = new Config("config.json"))
            {
                config.Initialize();

                // Setup client.
                var discord = new DiscordShardedClient(new DiscordConfiguration()
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot
                });
                discord.MessageCreated += Events.MessageCreated;

                // Setup slash commands.
                var slash = await discord.UseSlashCommandsAsync(new SlashCommandsConfiguration()
                {
                    Services = new ServiceCollection()
                        .AddTransient(x => new Config("config.json"))
                        .AddTransient(x => new Data("data.json"))
                        .BuildServiceProvider()
                });
                
                // Register commands.
                foreach (var extension in slash.Values)
                {
                    extension.RegisterCommands(Assembly.GetExecutingAssembly(), config.GuildId);
                    extension.SlashCommandErrored += Events.SlashCommandErrored;
                }

                // Start bot.
                await discord.StartAsync();
                discord.Ready += async (sender, args) => await discord.UpdateStatusAsync(new DiscordActivity(config.Status, config.StatusType));
                await Task.Delay(-1);
            }
        }

        public void Initialize() => InitializeAsync().GetAwaiter().GetResult();
    }
}