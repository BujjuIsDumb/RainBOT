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

using System.Diagnostics;
using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    public class Main : ApplicationCommandModule
    {
        public Data Data { private get; set; }

        [SlashCommand("about", "Get information about me.")]
        public async Task InfoAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle($"RainBOT v{Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}")
                .WithDescription("A bot designed to help start, manage, and moderate 2SLGBTQIA+ safespace servers.")
                .WithColor(new DiscordColor(3092790))
                .WithThumbnail("https://i.imgur.com/IHdrwiJ.png");

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("source", "View my source code.")]
        public async Task SourceAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to view my source code.")
                .AddComponents(new DiscordLinkButtonComponent("https://github.com/BujjuIsDumb/RainBOT", "Source"))
                .AsEphemeral());
        }

        [SlashCommand("support", "Join the support server.")]
        public async Task SupportAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to join the support server.")
                .AddComponents(new DiscordLinkButtonComponent("https://discord.gg/tKsqy5ZWFZ", "Server"))
                .AsEphemeral());
        }

        [SlashCommand("invite", "Invite me to your server.")]
        public async Task InviteAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to invite me to your server.")
                .AddComponents(new DiscordLinkButtonComponent("https://discord.com/api/oauth2/authorize?client_id=1004158946740809839&permissions=86167809030&scope=bot%20applications.commands", "Invite"))
                .AsEphemeral());
        }

        [SlashCommand("wiki", "Go to my wiki.")]
        public async Task WikiAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to go to my wiki.")
                .AddComponents(new DiscordLinkButtonComponent("https://github.com/BujjuIsDumb/RainBOT/wiki", "Wiki"))
                .AsEphemeral());
        }
    }
}