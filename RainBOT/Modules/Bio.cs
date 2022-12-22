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
using DSharpPlus.SlashCommands;
using RainBOT.Core.AutocompleteProviders;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("bio", "Create a bio with information about yourself.")]
    public class Bio : ApplicationCommandModule
    {
        public Database Data { private get; set; }

        [SlashCommand("create", "Create a bio field.")]
        public async Task BioCreateAsync(InteractionContext ctx,
            [Autocomplete(typeof(TemplateBioFieldsAutocompleteProvider))]
            [MaximumLength(256)]
            [Option("field", "What to name the field.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {

        }

        [SlashCommand("edit", "Edit a bio field.")]
        public async Task BioEditAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to edit.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {

        }

        [SlashCommand("delete", "Delete a bio field.")]
        public async Task BioClearAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to delete.", true)] string field)
        {

        }

        [SlashCommand("get", "View a user's bio.")]
        public async Task BioGetAsync(InteractionContext ctx,
            [Option("user", "The user to get the bio of.")] DiscordUser user)
        {

        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Bio")]
        public async Task ViewBioAsync(ContextMenuContext ctx)
        {

        }
    }
}