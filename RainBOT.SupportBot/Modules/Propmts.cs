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

using System.Text;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using RainBOT.SupportBot.Core.AutocompleteProviders;
using RainBOT.SupportBot.Core.Entities.Models;
using RainBOT.SupportBot.Core.Entities.Services;

namespace RainBOT.SupportBot.Modules
{
    [SlashCommandGroup("prompt", "A prompt is a reusable text snippet for common questions.")]
    public class Propmts : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("create", "Create a new prompt.")]
        public async Task PromptCreateAsync(InteractionContext ctx)
        {
            // Build modal.
            var promptCreateModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Create the prompt")
                .WithCustomId(Core.Utilities.CreateCustomId("promptCreateModal"))
                .AddComponents(new TextInputComponent(label: "Tags (Comma-separated)", customId: "tags", placeholder: "example, tag-example, tags-example", style: TextInputStyle.Short, min_length: 5, max_length: 50))
                .AddComponents(new TextInputComponent(label: "Prompt", customId: "prompt", style: TextInputStyle.Paragraph, min_length: 5, max_length: 1500));

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, promptCreateModal);

            // Respond to modal submission.
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == promptCreateModal.CustomId)
                {
                    string[] tags = args.Values["tags"].ToLower().Split(", ");

                    // Check for duplicate tags.
                    foreach (string tag in tags)
                    {
                        if (Data.Prompts.Exists(x => x.Tags.Contains(tag)) || tags.ToList().FindAll(x => x == tag).Count > 1)
                        {
                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent($"⚠ `{tag}` already exists.")
                                .AsEphemeral());

                            return;
                        }
                    }

                    Data.Prompts.Add(new PromptData()
                    {
                        Tags = tags,
                        Prompt = args.Values["prompt"],
                        CreatorUserId = ctx.User.Id
                    });
                    Data.Update();

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Created the prompt.")
                        .AsEphemeral());
                }
            };
        }

        [SlashCommand("edit", "Edit a prompt.")]
        public async Task PromptEditAsync(InteractionContext ctx,
            [Autocomplete(typeof(TagAutocompleteProvider))]
            [Option("query", "The tag to search for.", true)] string query)
        {

            if (Data.Prompts.Exists(x => x.Tags.Contains(query)))
            {
                if (Data.Prompts.Find(x => x.Tags.Contains(query)).CreatorUserId == ctx.User.Id)
                {
                    // Build modal.
                    var promptEditModal = new DiscordInteractionResponseBuilder()
                        .WithTitle("Edit the prompt")
                        .WithCustomId(Core.Utilities.CreateCustomId("promptEditModal"))
                        .AddComponents(new TextInputComponent(label: "Prompt", customId: "prompt", value: Data.Prompts.Find(x => x.Tags.Contains(query)).Prompt, style: TextInputStyle.Paragraph, min_length: 5, max_length: 1500));

                    await ctx.CreateResponseAsync(InteractionResponseType.Modal, promptEditModal);

                    // Respond to modal submission.
                    ctx.Client.ModalSubmitted += async (sender, args) =>
                    {
                        if (args.Interaction.Data.CustomId == promptEditModal.CustomId)
                        {
                            Data.Prompts.Find(x => x.Tags.Contains(query)).Prompt = args.Values["prompt"];
                            Data.Update();

                            await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                                .WithContent("✅ Edited the prompt.")
                                .AsEphemeral());
                        }
                    };
                }
                else
                {
                    await ctx.CreateResponseAsync("⚠ Only the creator of a prompt can edit it.", true);
                }
            }
            else
            {
                // Find similar tags.
                var allTags = new List<string>();
                var builder = new StringBuilder();

                foreach (var prompt in Data.Prompts)
                {
                    foreach (string tag in prompt.Tags)
                    {
                        allTags.Add(tag);
                    }
                }

                foreach (string tag in allTags.OrderBy(x => Core.Utilities.CompareStrings(query, x)))
                {
                    if ((Core.Utilities.CompareStrings(query, tag) * -1) > tag.Length - (tag.Length * 0.25))
                    {
                        builder.AppendLine("`" + tag + "`");
                    }
                }

                await ctx.CreateResponseAsync($"⚠ `{query}` doesn't exist. Did you mean:\n\n{builder}", true);
            }
        }

        [SlashCommand("delete", "Delete a prompt.")]
        public async Task PromptDeleteAsync(InteractionContext ctx,
            [Autocomplete(typeof(TagAutocompleteProvider))]
            [Option("query", "The tag to search for.", true)] string query)
        {

            if (Data.Prompts.Exists(x => x.Tags.Contains(query)))
            {
                if (Data.Prompts.Find(x => x.Tags.Contains(query)).CreatorUserId == ctx.User.Id)
                {
                    Data.Prompts.Remove(Data.Prompts.Find(x => x.Tags.Contains(query)));
                    Data.Update();

                    await ctx.CreateResponseAsync("✅ Deleted the prompt.", true);
                }
                else
                {
                    await ctx.CreateResponseAsync("⚠ Only the creator of a prompt can delete it.", true);
                }
            }
            else
            {
                // Find similar tags.
                var allTags = new List<string>();
                var builder = new StringBuilder();

                foreach (var prompt in Data.Prompts)
                {
                    foreach (string tag in prompt.Tags)
                    {
                        allTags.Add(tag);
                    }
                }

                foreach (string tag in allTags.OrderBy(x => Core.Utilities.CompareStrings(query, x)))
                {
                    if ((Core.Utilities.CompareStrings(query, tag) * -1) > tag.Length - (tag.Length * 0.05))
                    {
                        builder.AppendLine("`" + tag + "`");
                    }
                }

                await ctx.CreateResponseAsync($"⚠ `{query}` doesn't exist. Did you mean:\n\n{builder}", true);
            }
        }

        [SlashCommand("get", "Search for a prompt.")]
        public async Task PromptGetAsync(InteractionContext ctx,
            [Autocomplete(typeof(TagAutocompleteProvider))]
            [Option("query", "The tag to search for.", true)] string query)
        {
            foreach (var prompt in Data.Prompts)
            {
                if (prompt.Tags.Contains(query))
                {
                    var creator = await ctx.Client.GetUserAsync(prompt.CreatorUserId);

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: creator.Username, iconUrl: creator.AvatarUrl)
                        .WithTitle("`" + query + "`")
                        .WithDescription(prompt.Prompt)
                        .WithColor(new DiscordColor(3092790));

                    await ctx.CreateResponseAsync(embed, true);
                    return;
                }
            }

            // Find similar tags.
            var allTags = new List<string>();
            var builder = new StringBuilder();

            foreach (var prompt in Data.Prompts)
            {
                foreach (string tag in prompt.Tags)
                {
                    allTags.Add(tag);
                }
            }

            foreach (string tag in allTags.OrderBy(x => Core.Utilities.CompareStrings(query, x)).Take(10))
            {
                if ((Core.Utilities.CompareStrings(query, tag) * -1) > tag.Length - (tag.Length * 0.05))
                {
                    builder.AppendLine("`" + tag + "`");
                }
            }

            await ctx.CreateResponseAsync($"⚠ `{query}` doesn't exist. {(string.IsNullOrEmpty(builder.ToString()) ? string.Empty : $"Did you mean:\n\n{builder}")}", true);
        }

        [SlashCommand("share", "Share a prompt.")]
        public async Task PromptShareAsync(InteractionContext ctx,
            [Autocomplete(typeof(TagAutocompleteProvider))]
            [Option("query", "The tag to search for.", true)] string query)
        {
            foreach (var prompt in Data.Prompts)
            {
                if (prompt.Tags.Contains(query))
                {
                    var creator = await ctx.Client.GetUserAsync(prompt.CreatorUserId);

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: creator.Username, iconUrl: creator.AvatarUrl)
                        .WithTitle("`" + query + "`")
                        .WithDescription(prompt.Prompt)
                        .WithColor(new DiscordColor(3092790));

                    await ctx.CreateResponseAsync(embed);
                    return;
                }
            }

            // Find similar tags.
            var allTags = new List<string>();
            var builder = new StringBuilder();

            foreach (var prompt in Data.Prompts)
            {
                foreach (string tag in prompt.Tags)
                {
                    allTags.Add(tag);
                }
            }

            foreach (string tag in allTags.OrderBy(x => Core.Utilities.CompareStrings(query, x)).Take(10))
            {
                if ((Core.Utilities.CompareStrings(query, tag) * -1) > tag.Length - (tag.Length * 0.05))
                {
                    builder.AppendLine("`" + tag + "`");
                }
            }

            await ctx.CreateResponseAsync($"⚠ `{query}` doesn't exist. Did you mean:\n\n{builder}", true);
        }
    }
}