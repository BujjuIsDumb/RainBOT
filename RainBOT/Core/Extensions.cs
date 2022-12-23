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
using RainBOT.Core.Services;
using RainBOT.Core.Services.Models;

namespace RainBOT.Core
{
    /// <summary>
    ///     An extension for DSharpPlus objects.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        ///     Gets the user configuration.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="data">The database object to use.</param>
        /// <returns>The user configuration for the specified user.</returns>
        public static UserData GetUserData(this DiscordUser user, Database data)
        {
            if (!data.Users.Exists(x => x.UserId == user.Id))
            {
                data.Users.Add(new UserData() { UserId = user.Id });
                data.Update();
            }

            return data.Users.Find(x => x.UserId == user.Id);
        }

        /// <summary>
        ///     Gets the guild configuration.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <param name="data">The database object to use.</param>
        /// <returns>The guild configuration for the specified guild.</returns>
        public static GuildData GetGuildData(this DiscordGuild guild, Database data)
        {
            if (!data.Guilds.Exists(x => x.GuildId == guild.Id))
            {
                data.Guilds.Add(new GuildData() { GuildId = guild.Id });
                data.Update();
            }

            return data.Guilds.Find(x => x.GuildId == guild.Id);
        }
    }
}