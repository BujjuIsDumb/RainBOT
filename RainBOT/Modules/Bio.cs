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
using RainBOT.Core.Entities.Models;
using RainBOT.Core.Entities.Services;

namespace RainBOT.Modules
{
    [SlashCommandGroup("bio", "Create a bio with information about yourself.")]
    public class Bio : ApplicationCommandModule
    {
        public Config Config { private get; set; }

        public Data Data { private get; set; }

        [SlashCommand("create", "Create a bio field.")]
        [SlashRequireUserAccount]
        public async Task BioCreateAsync(InteractionContext ctx,
            [Autocomplete(typeof(TemplateBioFieldsAutocompleteProvider))]
            [MaximumLength(256)]
            [Option("field", "What to name the field.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {
            if (ctx.User.GetUserAccount(Data).BioFields.ToList().Exists(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You already have a field with that name. Use {Core.Utilities.GetCommandMention(ctx.Client, "bio edit")}.", true);
                return;
            }
            if (ctx.User.GetUserAccount(Data).BioFields.Length >= 10)
            {
                await ctx.CreateResponseAsync("⚠️ You can only have up to 10 bio fields.", true);
                return;
            }

            var bioFields = ctx.User.GetUserAccount(Data).BioFields.ToList();
            bioFields.Add(new BioFieldData()
            {
                Name = field,
                Value = value
            });

            ctx.User.GetUserAccount(Data).BioFields = bioFields.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync("✅ Created the field.", true);
        }

        [SlashCommand("edit", "Edit a bio field.")]
        [SlashRequireUserAccount]
        public async Task BioEditAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to edit.", true)] string field,
            [MaximumLength(1024)]
            [Option("value", "What to set the field to.")] string value)
        {
            if (!ctx.User.GetUserAccount(Data).BioFields.ToList().Exists(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You don't have a field with that name. Use {Core.Utilities.GetCommandMention(ctx.Client, "bio create")}.", true);
                return;
            }

            var bioFields = ctx.User.GetUserAccount(Data).BioFields.ToList();
            bioFields.Find(x => x.Name == field).Value = value;

            ctx.User.GetUserAccount(Data).BioFields = bioFields.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync("✅ Edited the field.", true);
        }

        [SlashCommand("delete", "Delete a bio field.")]
        [SlashRequireUserAccount]
        public async Task BioClearAsync(InteractionContext ctx,
            [Autocomplete(typeof(ExistingBioFieldsAutocompleteProvider))]
            [Option("field", "The name of the field to delete.", true)] string field)
        {
            if (!ctx.User.GetUserAccount(Data).BioFields.ToList().Exists(x => x.Name == field))
            {
                await ctx.CreateResponseAsync($"⚠️ You don't have a field with that name.", true);
                return;
            }

            var bioFields = ctx.User.GetUserAccount(Data).BioFields.ToList();
            bioFields.Remove(bioFields.Find(x => x.Name == field));

            ctx.User.GetUserAccount(Data).BioFields = bioFields.ToArray();
            Data.Update();

            await ctx.CreateResponseAsync("✅ Deleted the field.", true);
        }

        [SlashCommand("get", "View a user's bio.")]
        public async Task BioGetAsync(InteractionContext ctx,
            [Option("user", "The user to get the bio of.")] DiscordUser user)
        {
            if (Data.UserAccounts.Find(x => x.UserId == user.Id) is not null)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{user.Username}'s Bio")
                    .WithThumbnail(user.AvatarUrl)
                    .WithColor(new DiscordColor(Data.UserAccounts.Find(x => x.UserId == user.Id).BioStyle));

                foreach (var field in Data.UserAccounts.Find(x => x.UserId == user.Id).BioFields) embed.AddField(field.Name, "> " + field.Value);
                if (embed.Fields.Count <= 0) await ctx.CreateResponseAsync($"⚠️ **{user.Username}** doesn't have a bio.", true);
                else await ctx.CreateResponseAsync(embed, true);
            }
            else
            {
                await ctx.CreateResponseAsync($"⚠️ **{user.Username}** doesn't have an account.", true);
            }
        }

        [ContextMenu(ApplicationCommandType.UserContextMenu, "View Bio")]
        public async Task ViewBioAsync(ContextMenuContext ctx)
        {
            if (Data.UserAccounts.Find(x => x.UserId == ctx.TargetUser.Id) is not null)
            {
                var embed = new DiscordEmbedBuilder()
                    .WithTitle($"{ctx.TargetUser.Username}'s Bio")
                    .WithThumbnail(ctx.TargetUser.AvatarUrl)
                    .WithColor(new DiscordColor(Data.UserAccounts.Find(x => x.UserId == ctx.TargetUser.Id).BioStyle));

                foreach (var field in Data.UserAccounts.Find(x => x.UserId == ctx.TargetUser.Id).BioFields) embed.AddField(field.Name, "> " + field.Value);
                if (embed.Fields.Count <= 0) await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** doesn't have a bio.", true);
                else await ctx.CreateResponseAsync(embed, true);
            }
            else
            {
                await ctx.CreateResponseAsync($"⚠️ **{ctx.TargetUser.Username}** doesn't have an account.", true);
            }
        }
    }
}