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

using System.Text;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.Core;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules.MentalHealth
{
    [SlashCommandGroup("cw", "Content warnings are warnings that messages contain potentially triggering content.")]
    public class ContentWarnings : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("info", "Learn more about content warnings.")]
        public async Task CwInfoAsync(InteractionContext ctx)
        {
            var embed = new DiscordEmbedBuilder()
                .WithTitle("Content warnings")
                .WithDescription($"A content warning is a warning that a message contains " +
                $"content that may be triggering to some people. For example, a message that " +
                $"covers heavy subjects, a video/gif that contains flashing colors, or an image " +
                $"that shows an object that can trigger people who have experienced traumatic " +
                $"events will all need content warnings. Anything can be triggering to someone, " +
                $"so it's important to respect people's triggers even if you don't understand " +
                $"them. Content warnings should look like this:\n\n> CW Example\n\nYou can hide " +
                $"content in many ways, but I suggest using Discord's ||spoiler feature||. " +
                $"Discord will only show spoilered content after it's clicked, so the person " +
                $"can determine whether or not the content might trigger them. You can make " +
                $"spoilered text by adding two \"|\" symbols before and after the text you want " +
                $"to make spoilered. `||Spoiler||` becomes ||Spoiler||.\n\nYou can also spoiler " +
                $"images, gifs, videos, and files. You can spoiler an attachment on a computer " +
                $"by hovering over it and selecting the eye icon in the top right. On mobile, " +
                $"you can spoiler an attachment by tapping it and checking the \"Mark as spoiler" +
                $"\" checkbox. For some topics, even reading the warning can be triggering. " +
                $"I suggest censoring the warning and replacing the vowels with \"/\" symbols " +
                $"for these topics. A censored content warning should look like this:\n\n> CW " +
                $"||/x/mpl/ c/nt/nt w/rn/ng|| (||eae, oe, ai||)\n\nYou can also have me create " +
                $"a censored content warning with the " +
                $"{Utilities.GetCommandMention(ctx.Client, "cw create")} command.")
                .WithColor(new DiscordColor(3092790))
                .AddField("What needs a content warning?", "Well, it depends on the server and " +
                "the members' triggers. If you're unsure whether something needs a content " +
                "warning, you can always ask the staff or just use one anyway. Some things that " +
                "usually will need a content warning include:\n\n• Flashing colors/" +
                "photosensitivity triggers/eye strain\n• ||Suicide/sh||\n• ||Abuse/violence/" +
                "murder||\n• ||Death/injury/blood||\n• ||Sexual harassment/rape/sexual content" +
                "||\n• ||Food/eating/disordered eating/weight||\n• ||Weapons||\n• ||Bigotry/" +
                "genocide/hate speech/slurs/hate crimes||");

            await ctx.CreateResponseAsync(embed, true);
        }

        [SlashCommand("create", "Create a censored content warning.")]
        public async Task CwCreateAsync(InteractionContext ctx,
            [Option("warning", "The topic(s) (comma separated for more than one) to make a content warning for.")] string warning)
        {
            var sb1 = new StringBuilder(); // For the censored section.
            var sb2 = new StringBuilder(); // For the vowel key.

            foreach (char c in warning)
            {
                if ("aeiou".Contains(c.ToString().ToLower()))
                {
                    sb1.Append('/');
                    sb2.Append(c);
                }
                else if (c == ' ')
                {
                    sb1.Append(c);
                    sb2.Append(", ");
                }
                else sb1.Append(c);
            }

            string vowelKey = sb2.ToString().Replace(", , ", ", ").TrimEnd(',', ' ');
            await ctx.CreateResponseAsync($"Here is your censored content warning:\n\n```CW ||{sb1}|| {(string.IsNullOrEmpty(vowelKey) ? "" : $"(||{vowelKey}||)")}```", true);
        }
    }
}