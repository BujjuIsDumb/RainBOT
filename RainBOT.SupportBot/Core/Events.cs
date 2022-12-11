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
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;

namespace RainBOT.SupportBot.Core
{
    public class Events
    {
        public static async Task SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            await args.Context.CreateResponseAsync($"❌ An unexpected error has occurred.\n\n```{args.Exception.Message}```", true);
        }

        public static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
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
    }
}