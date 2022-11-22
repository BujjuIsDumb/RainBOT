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

using DSharpPlus.SlashCommands;
using RainBOT.Core.Entities.Models;

namespace RainBOT.Core.Entities.Services
{
    public class RbService : IDisposable
    {
        public Config Config;

        public Data Data;

        public RbService()
        {
            Config = new Config("config.json");
            Config.Initialize();

            Data = new Data("data.json");
            Data.Initialize();
        }

        public UserAccountData GetUserAccount(InteractionContext ctx)
        {
            return Data.UserAccounts.Find(x => x.UserId == ctx.User.Id);
        }

        public UserAccountData GetUserAccount(ContextMenuContext ctx)
        {
            return Data.UserAccounts.Find(x => x.UserId == ctx.User.Id);
        }

        public GuildAccountData GetGuildAccount(InteractionContext ctx)
        {
            return Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id);
        }

        public GuildAccountData GetGuildAccount(ContextMenuContext ctx)
        {
            return Data.GuildAccounts.Find(x => x.GuildId == ctx.Guild.Id);
        }

        public void Dispose()
        {
            Config = null;
            Data = null;
        }
    }
}