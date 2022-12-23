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

namespace RainBOT.Core.Pagination
{
    /// <summary>
    ///    The extension for pagination.
    /// </summary>
    public static class PaginationExtension
    {
        /// <summary>
        ///      Creates a paginated response to this interaction.
        /// </summary>
        /// <param name="interaction">The interaction.</param>
        /// <param name="client">The Discord client.</param>
        /// <param name="pages">The pages.</param>
        /// <param name="ephemeral">Whether the response should be ephemeral.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static async Task CreatePaginatedResponseAsync(this DiscordInteraction interaction, DiscordClient client, List<Page> pages, bool ephemeral)
        {
            int index = 0;

            if (pages.Count == 0)
                throw new ArgumentException("You must provide at least one page to paginate.", nameof(pages));

            #region Components
            var previous = new DiscordButtonComponent(ButtonStyle.Danger, $"previous-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Previous", pages.Count == 1);

            var next = new DiscordButtonComponent(ButtonStyle.Success, $"next-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Next", pages.Count == 1);
            #endregion

            #region Event Handlers
            client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == previous.CustomId)
                {
                    if (index - 1 >= 0)
                        index -= 1;
                    else
                        index = pages.Count - 1;

                    // Update the message.
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await interaction.EditOriginalResponseAsync(pages[index].ToDiscordWebhookBuilder().AddComponents(previous, next));
                }
                else if (args.Id == next.CustomId)
                {
                    if (index + 1 <= pages.Count - 1)
                        index += 1;
                    else
                        index = 0;

                    // Update the message.
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.DeferredMessageUpdate);
                    await interaction.EditOriginalResponseAsync(pages[index].ToDiscordWebhookBuilder().AddComponents(previous, next));
                }
            };
            #endregion

            await interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, pages.First().ToDiscordInteractionResponseBuilder(ephemeral).AddComponents(previous, next));
        }
    }
}