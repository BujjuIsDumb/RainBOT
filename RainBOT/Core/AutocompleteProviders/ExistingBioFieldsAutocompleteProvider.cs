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
using RainBOT.Core.Services;

namespace RainBOT.Core.AutocompleteProviders
{
    /// <summary>
    ///     An autocomplete provider for existing bio fields.
    /// </summary>
    public class ExistingBioFieldsAutocompleteProvider : IAutocompleteProvider
    {
        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            using var data = new Database("data.json").Initialize();

            var account = data.Users.Find(x => x.UserId == ctx.User.Id);
            var list = new List<DiscordAutoCompleteChoice>();

            if (account is not null)
            {
                foreach (var field in account.BioFields)
                    list.Add(new DiscordAutoCompleteChoice(field.Name, field.Name));
            }

            return Task.FromResult(list.OrderBy(x => AutocompleteHelper.CompareStrings((string)ctx.OptionValue, x.Name)) as IEnumerable<DiscordAutoCompleteChoice>);
        }
    }
}