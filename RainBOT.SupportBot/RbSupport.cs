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
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using RbSupport.Core;
using RbSupport.Core.Entities.Services;

namespace RbSupport
{
    public class RbSupport
    {
        private async Task InitializeAsync()
        {
            using (var config = new Config("config.json").Initialize())
            {
                // Setup client.
                var discord = new DiscordClient(new DiscordConfiguration()
                {
                    Token = config.Token,
                    TokenType = TokenType.Bot
                });
                discord.MessageCreated += Events.MessageCreated;

                // Setup slash commands.
                var slash = discord.UseSlashCommands(new SlashCommandsConfiguration()
                {
                    Services = new ServiceCollection()
                        .AddTransient(x => new Config("config.json").Initialize())
                        .AddTransient(x => new Data("data.json").Initialize())
                        .BuildServiceProvider()
                });

                // Register commands.
                slash.RegisterCommands(Assembly.GetExecutingAssembly(), config.GuildId);
                slash.SlashCommandErrored += Events.SlashCommandErrored;

                // Start bot.
                await discord.ConnectAsync(new DiscordActivity(config.Status, config.StatusType));
                await Task.Delay(-1);
            }
        }

        public void Initialize() => InitializeAsync().GetAwaiter().GetResult();
    }
}