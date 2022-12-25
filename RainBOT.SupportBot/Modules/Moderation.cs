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
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.SupportBot.Core.Services;
using RainBOT.SupportBot.Core.Services.Models;

namespace RainBOT.SupportBot.Modules
{
    /// <summary>
    ///     The moderation module.
    /// </summary>
    [GuildOnly]
    public class Moderation : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /purge command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="amount">The amount of messages to delete.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("purge", "Delete multiple messages at once.")]
        [SlashCooldown(1, 30, SlashCooldownBucketType.Guild)]
        [SlashCommandPermissions(Permissions.ManageMessages)]
        [SlashRequireBotPermissions(Permissions.ManageMessages)]
        public async Task PurgeAsync(InteractionContext ctx,
            [Option("amount", "The amount of messages to delete.")] long amount)
        {
            #region Components
            var confimButton = new DiscordButtonComponent(ButtonStyle.Success, $"confimButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Yes", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("✅")));
            
            var nevermindButton = new DiscordButtonComponent(ButtonStyle.Danger, $"nevermindButton-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Nevermind", false, new DiscordComponentEmoji(DiscordEmoji.FromUnicode("❌")));
            #endregion

            #region Event Handlers
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == confimButton.CustomId)
                {
                    await ctx.DeleteResponseAsync();

                    try
                    {
                        await ctx.Channel.DeleteMessagesAsync(await ctx.Channel.GetMessagesAsync((int)amount), $"Purged messages (/purge executed by {ctx.User.Username})");
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
            #endregion

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent($"Are you sure you want to delete {amount} messages?")
                .AddComponents(confimButton, nevermindButton)
                .AsEphemeral());
        }

        /// <summary>
        ///     The Add Notes context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.UserContextMenu, "Add Note")]
        [SlashCommandPermissions(Permissions.ModerateMembers)]
        public async Task AddNoteAsync(ContextMenuContext ctx)
        {
            #region Components
            var noteModal = new DiscordInteractionResponseBuilder()
                .WithTitle("Add a mod note")
                .WithCustomId($"noteModal-{DateTimeOffset.Now.ToUnixTimeSeconds()}")
                .AddComponents(new TextInputComponent(label: "Subject", customId: "subject", placeholder: "Describe the note.", style: TextInputStyle.Short, min_length: 5, max_length: 50))
                .AddComponents(new TextInputComponent(label: "Note", customId: "note", placeholder: "What should the note say?", style: TextInputStyle.Paragraph, min_length: 10, max_length: 1200));
            #endregion

            #region Event Handlers
            ctx.Client.ModalSubmitted += async (sender, args) =>
            {
                if (args.Interaction.Data.CustomId == noteModal.CustomId)
                {
                    Data.ModNotes.Add(new ModNoteData()
                    {
                        UserId = ctx.TargetUser.Id,
                        CreatorUserId = ctx.User.Id,
                        Subject = args.Values["subject"],
                        Note = args.Values["note"],
                        CreationTimestamp = DateTimeOffset.Now
                    });
                    Data.Update();

                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .WithContent("✅ Created the note.")
                        .AsEphemeral());
                }
            };
            #endregion

            await ctx.CreateResponseAsync(InteractionResponseType.Modal, noteModal);
        }

        /// <summary>
        ///     The View Notes context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Notes")]
        [SlashCommandPermissions(Permissions.ModerateMembers)]
        public async Task ViewNotesAsync(ContextMenuContext ctx)
        {
            if (!Data.ModNotes.Exists(x => x.UserId == ctx.TargetUser.Id))
            {
                await ctx.CreateResponseAsync($"⚠ There are no notes for **{ctx.TargetUser.Username}**.", true);
                return;
            }

            #region Components
            var noteSelectOptions = Data.ModNotes.FindAll(x => x.UserId == ctx.TargetUser.Id).Select(x => new DiscordSelectComponentOption(x.Subject, x.ModNoteId.ToString()));
            var noteSelect = new DiscordSelectComponent($"noteSelect-{DateTimeOffset.Now.ToUnixTimeSeconds()}", "Select a note to view.", noteSelectOptions);
            #endregion

            #region Event Handlers
            ctx.Client.ComponentInteractionCreated += async (sender, args) =>
            {
                if (args.Id == noteSelect.CustomId)
                {
                    var note = Data.ModNotes.Find(x => x.ModNoteId == ulong.Parse(args.Values[0]));
                    var noteCreator = await ctx.Client.GetUserAsync(note.CreatorUserId);

                    var embed = new DiscordEmbedBuilder()
                        .WithAuthor(name: noteCreator.Username, iconUrl: noteCreator.AvatarUrl)
                        .WithTitle(note.Subject)
                        .WithDescription(note.Note)
                        .WithTimestamp(note.CreationTimestamp)
                        .WithColor(new DiscordColor(3092790));
                    
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder()
                        .AddEmbed(embed)
                        .AsEphemeral());
                }
            };
            #endregion

            await ctx.CreateResponseAsync(new DiscordInteractionResponseBuilder()
                .WithContent("Select a note to view.")
                .AddComponents(noteSelect)
                .AsEphemeral());
        }
    }
}