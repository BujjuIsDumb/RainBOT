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

using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace RainBOT.Core.AutocompleteProviders
{
    /// <summary>
    ///     An autocomplete provider that sorts the choices.
    /// </summary>
    public abstract class SortedAutocompleteProvider : IAutocompleteProvider
    {
        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            string userInput = (string)ctx.OptionValue;
            var choices = GetChoices(ctx);
            IEnumerable<DiscordAutoCompleteChoice> filteredChoices = choices.Where(choice => choice.Name.ToLower().Contains(userInput)).OrderBy(choice => choice.Name);

            return Task.FromResult(filteredChoices);
        }

        /// <summary>
        ///     Gets the choices for the autocomplete.
        /// </summary>
        /// <param name="ctx">The autocomplete context.</param>
        /// <returns>The choices for the autocomplete.</returns>
        public abstract List<DiscordAutoCompleteChoice> GetChoices(AutocompleteContext ctx);
    }
}