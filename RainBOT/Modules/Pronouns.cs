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

using DSharpPlus.SlashCommands;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("pronouns", "Try new pronouns to see if they fit.")]
    public class Pronouns : ApplicationCommandModule
    {
        public Database Data { private get; set; }

        [SlashCommand("try", "Try new pronouns.")]
        public async Task PronounsTryAsync(InteractionContext ctx,
            [Option("subjective", "He, she, and they are subjective pronouns.")] string subjective,
            [Option("objective", "Him, her, and them are objective pronouns.")] string objective,
            [Option("possessive-adjective", "His, her, and their are possessive adjectives.")] string possessiveAdjective,
            [Option("possessive-pronoun", "His, hers, and theirs are possessive pronouns.")] string possessivePronoun,
            [Option("reflexive", "Himself, herself, and themself are reflexive pronouns.")] string reflexive,
            [Option("plural", "Whether or not to use plural grammar rules for the pronouns.")] bool plural)
        {

        }

        [SlashCommand("random", "Get a random set of pronouns to try.")]
        public async Task PronounsRandom(InteractionContext ctx)
        {

        }
    }
}