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

namespace RainBOT.Core.Pagination
{
    /// <summary>
    ///     Represents a page in a paginated message.
    /// </summary>
    public class Page
    {
        /// <summary>
        ///     Gets or sets the content of the page message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        ///     Gets or sets the embed of the page message.
        /// </summary>
        public DiscordEmbed Embed { get; set; }

        /// <summary>
        ///     Sets the content of the message.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>This <see cref="Page"/>.</returns>
        public Page WithContent(string content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        ///     Sets the embed of the message.
        /// </summary>
        /// <param name="embed">The embed.</param>
        /// <returns>This <see cref="Page"/>.</returns>
        public Page WithEmbed(DiscordEmbed embed)
        {
            Embed = embed;
            return this;
        }

        /// <summary>
        ///     Converts this <see cref="Page"/> to a <see cref="DiscordInteractionResponseBuilder"/>.
        /// </summary>
        /// <param name="ephemeral">Whether the response should be ephemeral.</param>
        /// <returns>A <see cref="DiscordInteractionResponseBuilder"/> from this <see cref="Page"/>.</returns>
        internal DiscordInteractionResponseBuilder ToDiscordInteractionResponseBuilder(bool ephemeral)
        {
            var builder = new DiscordInteractionResponseBuilder()
                .WithContent(Content)
                .AddEmbed(Embed)
                .AsEphemeral(ephemeral);

            return builder;
        }

        /// <summary>
        ///     Converts this <see cref="Page"/> to a <see cref="DiscordWebhookBuilder"/>.
        /// </summary>
        /// <returns>A <see cref="DiscordWebhookBuilder"/> from this <see cref="Page"/>.</returns>
        internal DiscordWebhookBuilder ToDiscordWebhookBuilder()
        {
            var builder = new DiscordWebhookBuilder()
                .WithContent(Content)
                .AddEmbed(Embed);

            return builder;
        }
    }
}