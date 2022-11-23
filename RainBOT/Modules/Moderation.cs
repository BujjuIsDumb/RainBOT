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
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;

namespace RainBOT.Modules
{
    [GuildOnly]
    public class Moderation : ApplicationCommandModule
    {
        [SlashCommand("purge", "Delete multiple messages at once.")]
        [SlashCooldown(1, 30, SlashCooldownBucketType.Guild)]
        [SlashCommandPermissions(Permissions.ManageMessages)]
        [SlashRequireBotPermissions(Permissions.ManageMessages)]
        public async Task PurgeAsync(InteractionContext ctx,
            [Option("amount", "The amount of messages to delete.")] long amount)
        {
            var confimButton = new DiscordButtonComponent(ButtonStyle.Primary, Core.Utilities.CreateCustomId("confimButton"), "Yes", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));
            var nevermindButton = new DiscordButtonComponent(ButtonStyle.Primary, Core.Utilities.CreateCustomId("nevermindButton"), "Nevermind", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("❌")));

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Are you sure you want to delete {amount} messages?")
                .AddComponents(confimButton, nevermindButton)
                .AsEphemeral());

            // Respond to button input.
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == confimButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();

                    try
                    {
                        await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync((int)amount));
                    }
                    catch (ArgumentException)
                    {
                        await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                            .WithContent($"⚠ There were no messages to delete.")
                            .AsEphemeral());

                        return;
                    }

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent($"✅ Deleted the last {amount} message{(amount == 1 ? string.Empty : "s")}.")
                        .AsEphemeral());
                }
                else if (args.Id == nevermindButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent($"✅ Okay, the message{(amount == 1 ? string.Empty : "s")} will not be deleted.")
                        .AsEphemeral());
                }
            };
        }
    }
}