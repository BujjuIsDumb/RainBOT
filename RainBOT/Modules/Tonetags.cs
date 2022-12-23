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
using DSharpPlus.SlashCommands;
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

        }

        /// <summary>
        ///     The /tonetags list command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("list", "Get a list of tonetags.")]
        public async Task TonetagsListAsync(InteractionContext ctx)
        {

        }

        /// <summary>
        ///     The Find Tonetags context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.MessageContextMenu, "Find Tonetags")]
        public async Task FindTonetagsAsync(ContextMenuContext ctx)
        {

        }
    }
}