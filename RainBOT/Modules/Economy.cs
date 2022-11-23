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

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("economy", "Earn RainBOT coins. RainBOT coins do not do anything.")]
    public class Economy : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("work", "Earn money by working.")]
        [SlashCooldown(1, 86400, SlashCooldownBucketType.User)]
        [SlashRequireUserAccount]
        public async Task EconomyWorkAsync(InteractionContext ctx)
        {
            Data.UserAccounts.Find(x => x.UserId == ctx.User.Id).Coins += 100;
            Data.Update();

            await ctx.CreateResponseAsync($"✅ You have worked for the day. You now have ¤{Data.UserAccounts.Find(x => x.UserId == ctx.User.Id).Coins}!", true);
        }

        [SlashCommand("coins", "See how many coins you have.")]
        [SlashRequireUserAccount]
        public async Task EconomyCoinsAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync($"You have ¤{Data.UserAccounts.Find(x => x.UserId == ctx.User.Id).Coins}", true);
        }

        [SlashCommand("leaderboard", "View the leaderboard.")]
        public async Task EconomyLeaderboardAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Leaderboard")
                .WithColor(new DiscordColor(3092790));

            var leaderboard = Data.UserAccounts.OrderBy(x => x.Coins * -1).Take(10);

            foreach (var account in leaderboard)
            {
                // Set place number.
                string placeString = (leaderboard.ToList().IndexOf(account) + 1).ToString() + ". ";

                // Set place number to medals for first three users on leaderboard.
                switch (leaderboard.ToList().IndexOf(account))
                {
                    case 0: placeString = "🥇 "; break;
                    case 1: placeString = "🥈 "; break;
                    case 2: placeString = "🥉 "; break;
                }

                embed.AddField(placeString + (await ctx.Client.GetUserAsync(account.UserId)).Username, "¤" + account.Coins);
            }

            await ctx.CreateResponseAsync(embed, true);
        }
    }
}