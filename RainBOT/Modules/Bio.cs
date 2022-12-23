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
using RainBOT.Core;
using RainBOT.Core.Attributes;
using RainBOT.Core.AutocompleteProviders;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The bio module.
    /// </summary>
    [SlashCommandGroup("bio", "Create a bio with information about yourself.")]
    public class Bio : ApplicationCommandModule
    {
        /// <summary>
        ///     The database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /bio create command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="field">The field option.</param>
        /// <param name="value">The value option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("create", "Create a bio field.")]
        [SlashBannable]
        public async Task BioCreateAsync(InteractionContext ctx,
            [Autocomplete(typeof(TemplateBioFieldsAutocompleteProvider))]
            [MaximumLength(256)]
            [Option("field", "What to name the field.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {
            if (ctx.User.GetUserData(Data).BioFields.Any(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You already have a field with that name.Use {Core.Utilities.GetCommandMention(ctx.Client, "bio edit")}.", true);
                return;
            }

            if (ctx.User.GetUserData(Data).BioFields.Length >= 25)
            {
                await ctx.CreateResponseAsync("⚠️ You can only have up to 25 bio fields.", true);
                return;
            }

            // Create a new array with the new field.
            var bioFields = ctx.User.GetUserData(Data).BioFields.Append(new Core.Services.Models.UserData.BioField()
            {
                Name = field,
                Value = value
            }).ToArray();

            ctx.User.GetUserData(Data).BioFields = bioFields;
            Data.Update();

            await ctx.CreateResponseAsync("✅ Created the field.", true);
        }

        /// <summary>
        ///     The /bio edit command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="field">The field option.</param>
        /// <param name="value">The value option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("edit", "Edit a bio field.")]
        [SlashBannable]
        public async Task BioEditAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to edit.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {
            if (!ctx.User.GetUserData(Data).BioFields.Any(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You don't have a field with that name. Use {Core.Utilities.GetCommandMention(ctx.Client, "bio create")}.", true);
                return;
            }

            // Create a new array with the edited field.
            var bioFields = ctx.User.GetUserData(Data).BioFields.ToList();
            bioFields.Find(x => x.Name == field).Value = value;

            ctx.User.GetUserData(Data).BioFields = bioFields.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync("✅ Created the field.", true);
        }

        /// <summary>
        ///     The /bio delete command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="field">The field option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("delete", "Delete a bio field.")]
        [SlashBannable]
        public async Task BioClearAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to delete.", true)] string field)
        {
            if (!ctx.User.GetUserData(Data).BioFields.Any(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You don't have a field with that name. Use {Core.Utilities.GetCommandMention(ctx.Client, "bio create")}.", true);
                return;
            }

            // Create a new array with the edited field.
            var bioFields = ctx.User.GetUserData(Data).BioFields.ToList();
            bioFields.Remove(bioFields.Find(x => x.Name == field));

            ctx.User.GetUserData(Data).BioFields = bioFields.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync("✅ Created the field.", true);
        }

        /// <summary>
        ///     The /bio get command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="user">The user option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("get", "View a user's bio.")]
        public async Task BioGetAsync(InteractionContext ctx,
            [Option("user", "The user to get the bio of.")] DiscordUser user)
        {
            if (!Data.Users.Exists(x => x.UserId == user.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ **{user.Username}** doesn't have a bio.", true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{user.Username}'s Bio")
                .WithThumbnail(user.AvatarUrl)
                .WithColor(new DiscordColor(user.GetUserData(Data).BioStyle));

            foreach (var field in user.GetUserData(Data).BioFields) embed.AddField(field.Name, "> " + field.Value);
            if (embed.Fields.Count <= 0) await ctx.CreateResponseAsync($"⚠️ **{user.Username}** doesn't have a bio.", true);
            else await ctx.CreateResponseAsync(embed, true);
        }

        /// <summary>
        ///     The View Bio context menu.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Bio")]
        public async Task ViewBioAsync(ContextMenuContext ctx)
        {
            if (!Data.Users.Exists(x => x.UserId == ctx.TargetUser.Id))
            {
                await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** doesn't have a bio.", true);
                return;
            }

            var embed = new DiscordEmbedBuilder()
                .WithTitle($"{ctx.TargetUser.Username}'s Bio")
                .WithThumbnail(ctx.TargetUser.AvatarUrl)
                .WithColor(new DiscordColor(ctx.TargetUser.GetUserData(Data).BioStyle));

            foreach (var field in ctx.TargetUser.GetUserData(Data).BioFields) embed.AddField(field.Name, "> " + field.Value);
            if (embed.Fields.Count <= 0) await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** doesn't have a bio.", true);
            else await ctx.CreateResponseAsync(embed, true);
        }
    }
}