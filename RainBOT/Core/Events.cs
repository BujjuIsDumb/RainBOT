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

using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using RainBOT.Core.Attributes;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Core
{
    public class Events
    {
        public static async Task SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
        {
            if (args.Exception is SlashExecutionChecksFailedException slashExecutionChecksFailedException)
            {
                var attribute = slashExecutionChecksFailedException.FailedChecks.First();

                if (attribute is SlashCooldownAttribute slashCooldownAttribute)
                {
                    long unix = ((DateTimeOffset)DateTime.Now.Add(slashCooldownAttribute.GetRemainingCooldown(args.Context))).ToUnixTimeSeconds();
                    await args.Context.CreateResponseAsync($"⚠️ This command is on cooldown. (Finished <t:{unix}:R>)", true);
                }
                else if (attribute is SlashRequireBotPermissionsAttribute slashRequireBotPermissionsAttribute)
                {
                    await args.Context.CreateResponseAsync($"⚠️ I need `{slashRequireBotPermissionsAttribute.Permissions}` permissions for this command to work.", true);
                }
                else if (attribute is SlashRequireUserAccountAttribute)
                {
                    await args.Context.CreateResponseAsync($"⚠️ You need a user account to use this commnad. Create one with {Utilities.GetCommandMention(args.Context.Client, "user register")}.", true);
                }
                else if (attribute is SlashRequireGuildAccountAttribute)
                {
                    await args.Context.CreateResponseAsync($"⚠️ The server needs an account to use this command. Create one with {Utilities.GetCommandMention(args.Context.Client, "server register")}.", true);
                }
                else if (attribute is SlashUserBannableAttribute slashUserBannableAttribute)
                {
                    using (var data = new Data("data.json"))
                    {
                        await args.Context.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                            .WithContent($"⚠️ You are banned from RainBOT for \"{data.UserBans.Find(x => x.UserId == args.Context.User.Id).Reason}\".")
                            .AddComponents(new DiscordLinkButtonComponent("https://forms.gle/mBBhmmT9qC57xjkG7", "Appeal"))
                            .AsEphemeral());
                    }
                }
                else if (attribute is SlashGuildBannableAttribute slashGuildBannableAttribute)
                {
                    using (var data = new Data("data.json").Initialize())
                    {
                        await args.Context.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                            .WithContent($"⚠️ This server is banned from RainBOT for \"{data.GuildBans.Find(x => x.GuildId == args.Context.Guild.Id).Reason}\".")
                            .AddComponents(new DiscordLinkButtonComponent("https://forms.gle/mBBhmmT9qC57xjkG7", "Appeal"))
                            .AsEphemeral());
                    }
                }
            }
            else
            {
                await args.Context.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                    .WithContent($"❌ An unexpected error has occurred. If you think this is a bug, please report it.\n\n```{args.Exception.Message}```")
                    .AddComponents(new DiscordLinkButtonComponent("https://github.com/BujjuIsDumb/RainBOT/issues", "Report"))
                    .AsEphemeral());
            }
        }

        public static async Task MessageCreated(DiscordClient sender, MessageCreateEventArgs args)
        {
            if (args.MentionedUsers.Contains(sender.CurrentUser))
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle("👋 Get started")
                    .WithDescription($"Welcome to RainBOT! I use slash commands, so you can view all of my commands by typing a `/` symbol.")
                    .WithImageUrl("https://i.imgur.com/sWoBYi6.png")
                    .WithFooter("Hint: Try /help!")
                    .WithColor(new DiscordColor(3092790));

                await args.Message.RespondAsync(embed);
            }
        }
    }
}