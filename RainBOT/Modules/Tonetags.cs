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

using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core;
using RainBOT.Core.AutocompleteProviders;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The tonetags module.
    /// </summary>
    [SlashCommandGroup("tonetags", "Tonetags convey tone to people who struggle to identify on their own.")]
    public class Tonetags : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///    The /tonetags info command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("info", "Learn more about tonetags.")]
        public async Task TonetagsInfoAsync(InteractionContext ctx)
        {
            string cmd = (await ctx.Client.GetGlobalApplicationCommandAsync("tonetags")).GetSubcommandMention("define");
            await ctx.CreateResponseAsync(new DiscordEmbedBuilder()
                .WithTitle("Tonetags")
                .WithDescription($"Tonetags are indicators of a message's tone, which is important for people who struggle to identify tone by themselves (For example, some neuro-divergent people). You can use a tonetag if you think the tone of your message may be unclear. If you don't know what a tonetag means, you can look it up with {cmd} or define all the tonetags with \"Find Tonetags\".")
                .WithColor(new DiscordColor(3092790)), true);
        }

        /// <summary>
        ///     The /tonetags define command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="tonetag">The tonetag option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("define", "Define a tonetag.")]
        public async Task TonetagsDefineAsync(InteractionContext ctx,
            [Autocomplete(typeof(TonetagsDefineAutocompleteProvider))]
            [Option("tonetag", "The tonetag to define.", true)] string tonetag)
        {
            if (Definitions.Tonetags.TryGetValue($"{(tonetag.StartsWith("/") ? "" : "/")}{tonetag.ToLower()}", out string definition))
                await ctx.CreateResponseAsync($"`{tonetag}` {definition}", true);
            else
                await ctx.CreateResponseAsync("⚠️ That tonetag isn't defined.", true);
        }

        /// <summary>
        ///     The /tonetags list command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("list", "Get a list of tonetags.")]
        public async Task TonetagsListAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Tonetag List")
                .WithDescription(string.Join("\n", Definitions.Tonetags.Keys.Select(x => $"`{x}` {Definitions.Tonetags[x]}")))
                .WithColor(new DiscordColor(3092790));

            await ctx.CreateResponseAsync(embed, true);
        }

        /// <summary>
        ///     The Find Tonetags context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Find Tonetags")]
        public async Task FindTonetagsAsync(ContextMenuContext ctx)
        {
            var builder = new StringBuilder();

            foreach (string section in ctx.TargetMessage.Content.Split(' ', '\n'))
            {
                if (section.Contains('/'))
                {
                    string tonetag = section[section.IndexOf('/')..].ToLower();

                    if (Definitions.Tonetags.TryGetValue($"{(tonetag.StartsWith("/") ? "" : "/")}{tonetag.ToLower()}", out string definition))
                        builder.AppendLine($"`{tonetag}` {definition}");
                }
            }

            if (!string.IsNullOrEmpty(builder.ToString()))
                await ctx.CreateResponseAsync(builder.ToString(), true);
            else
                await ctx.CreateResponseAsync("⚠️ No tonetag was found.", true);
        }
    }
}