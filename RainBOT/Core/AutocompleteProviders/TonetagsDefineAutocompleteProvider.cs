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
    public class TonetagsDefineAutocompleteProvider : IAutocompleteProvider
    {
        public Task<IEnumerable<DiscordAutoCompleteChoice>> Provider(AutocompleteContext ctx)
        {
            var list = new List<DiscordAutoCompleteChoice>
            {
                new DiscordAutoCompleteChoice("/j", "/j"),
                new DiscordAutoCompleteChoice("/hj", "/hj"),
                new DiscordAutoCompleteChoice("/s", "/s"),
                new DiscordAutoCompleteChoice("/srs", "/srs"),
                new DiscordAutoCompleteChoice("/gen", "/gen"),
                new DiscordAutoCompleteChoice("/lh", "/lh"),
                new DiscordAutoCompleteChoice("/ref", "/ref"),
                new DiscordAutoCompleteChoice("/nm", "/nm"),
                new DiscordAutoCompleteChoice("/nbh", "/nbh"),
                new DiscordAutoCompleteChoice("/nay", "/nay"),
                new DiscordAutoCompleteChoice("/nbr", "/nbr"),
                new DiscordAutoCompleteChoice("/pos", "/pos"),
                new DiscordAutoCompleteChoice("/neg", "/neg"),
                new DiscordAutoCompleteChoice("/neu", "/neu")
            };

            return Task.FromResult(list.OrderBy(x => Utilities.CompareStrings((string)ctx.OptionValue, x.Name)) as IEnumerable<DiscordAutoCompleteChoice>);
        }
    }
}