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

using System.Diagnostics;
using System.Reflection;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    public class Basics : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("help", "Get help with something.")]
        public async Task HelpAsync(InteractionContext ctx,
            [Choice("Setting up verification", 0)]
            [Choice("Reporting banned users", 1)]
            [Choice("Setting up command permissions", 2)]
            [Choice("Buttons/select menus/modals not working", 3)]
            [Choice("Bugs, typos, and suggestions", 4)]
            [Choice("Other", 5)]
            [Option("help", "What do you need help with?")] long help)
        {
            if (help == 0)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Setting up verification")
                    .WithDescription("If your server is commonly raided or trolled, you can set " +
                    "up a verification system to vet new members before they can access your " +
                    "server. I have a verification system built in, but you'll need to do some " +
                    "work around the server to ensure unverified members can't do anything bad." +
                    "\n\nThe first thing you'll need to do is make a member role. Give the " +
                    "member role all the permissions that the @everyone role currently has. " +
                    "Then disable the permissions for the @everyone role. This will ensure your " +
                    "unverified members can not do any trolling, raiding, or spamming.\n\nThen, " +
                    "you can create a channel with permission overrides to allow unverified " +
                    "members to use the verification system. They should have permissions to " +
                    "read messages, send messages, and use application commands in the " +
                    "unverified channel.\n\nYou can disable the verification command for " +
                    "verified members using command permissions if you don't want them using it " +
                    "in your normal channels. There is another tutorial about command " +
                    "permissions that will show you how.")
                    .WithColor(new DiscordColor(3092790));

                await ctx.CreateResponseAsync(embed, true);
            }
            else if (help == 1)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Reporting banned users")
                    .WithDescription("When trying to report a banned user, they will not appear " +
                    "in the list of server members. You can still report them, but you'll have " +
                    "to access their snowflake ID. Every Discord user has a unique 18/19-digit " +
                    "number associated with their account. You can use this snowflake ID in " +
                    "place of the user's name when reporting them. Here is how to get the " +
                    "snowflake ID of a banned member.\n\nYou'll need to enable developer mode " +
                    "in your Discord settings. (**User Settings > Advanced > Developer Mode**) " +
                    "If you have permissions to ban members in the server, you can go to **" +
                    "Server Settings > Bans**, right-click the user's name, and select \"Copy ID" +
                    "\" to copy the user's ID. Otherwise, you'll need someone with permissions " +
                    "to ban members to get the user's ID for you.")
                    .WithColor(new DiscordColor(3092790));

                await ctx.CreateResponseAsync(embed, true);
            }
            else if (help == 2)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Setting up command permissions")
                    .WithDescription("Do you want to restrict certain commands to a specific " +
                    "channel, require a particular role to use certain commands, or disable " +
                    "some commands entirely? You can do that with command permissions.\n\nYou " +
                    "can find the command permissions menu in **Server Settings > Integrations " +
                    "> RainBOT**. Then you can select the command you want to configure. This " +
                    "menu isn't available on mobile, so if you don't have access to the " +
                    "computer or web app, you'll have to ask someone else to set up command " +
                    "permissions.\n\nSome commands are disabled by default, and some commands " +
                    "have user permission requirements that cannot be changed. People with " +
                    "Administrator permissions can use commands even if they're disabled for " +
                    "@everyone.")
                    .WithColor(new DiscordColor(3092790));

                await ctx.CreateResponseAsync(embed, true);
            }
            else if (help == 3)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Buttons/select menus/modals not working")
                    .WithDescription("Has a button, select menu, or modal suddenly stopped " +
                    "working? This probably means I've been restarted. Every time I am " +
                    "restarted *(such as in the case of an update or crash)*, I will stop " +
                    "listening to existing message components and modals to prevent errors from " +
                    "lost data or changes made to my code.\n\nYou'll have to redo whatever " +
                    "created the component/modal to use it again. Your command cooldown will " +
                    "also have been reset, so you won't have to wait to reuse any limited " +
                    "commands.")
                    .WithColor(new DiscordColor(3092790));

                await ctx.CreateResponseAsync(embed, true);
            }
            else if (help == 4)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("Bugs, typos, and suggestions")
                    .WithDescription("Bug reports, typo reports, and featuer suggestions should " +
                    "all be made [here](https://github.com/BujjuIsDumb/RainBOT/issues).\n\nWe " +
                    "try to get bugs fixed as soon as possible, but typos and suggested " +
                    "features may take a bit longer. Please give as much detail as possible.")
                    .WithColor(new DiscordColor(3092790));

                await ctx.CreateResponseAsync(embed, true);
            }
            else if (help == 5)
            {
                await ctx.CreateResponseAsync($"You can get additional help in the support server. Get the invite with {Core.Utilities.GetCommandMention(ctx.Client, "support")}.", true);
            }
        }

        [SlashCommand("info", "Get information about me.")]
        public async Task InfoAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("RainBOT")
                .WithDescription("A bot designed to help start, manage, and moderate 2SLGBTQIA+ safespace servers.")
                .WithColor(new DiscordColor(3092790))
                .WithThumbnail("https://i.imgur.com/IHdrwiJ.png")
                .AddField("Latency", ctx.Client.Ping + "ms", true)
                .AddField("Shard", "#" + ctx.Client.ShardId, true)
                .AddField("Uptime", $"Started <t:{((DateTimeOffset)Process.GetCurrentProcess().StartTime.ToUniversalTime()).ToUnixTimeSeconds()}:R>", true)
                .AddField("Library", "DSharpPlus", true)
                .AddField("Creator", "[Bujju](https://github.com/BujjuIsDumb)", true)
                .AddField("Version", "v" + Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion, true);

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("source", "View my source code.")]
        public async Task SourceAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to view my source code.")
                .AddComponents(new DiscordLinkButtonComponent(Config.SourceUrl, "Source"))
                .AsEphemeral());
        }

        [SlashCommand("support", "Join the support server.")]
        public async Task SupportAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to join the support server.")
                .AddComponents(new DiscordLinkButtonComponent(Config.SupportUrl, "Server"))
                .AsEphemeral());
        }

        [SlashCommand("invite", "Invite me to your server.")]
        public async Task InviteAsync(InteractionContext ctx)
        {
            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Click the button to invite me to your server.")
                .AddComponents(new DiscordLinkButtonComponent(Config.InviteUrl, "Invite"))
                .AsEphemeral());
        }
    }
}