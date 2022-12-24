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

using DSharpPlus.SlashCommands;
using RainBOT.Core.Services;

namespace RainBOT.Core.Attributes
{
    /// <summary>
    ///     A slash command check that checks if the user is banned.
    /// </summary>
    public class SlashUserBannable : SlashCheckBaseAttribute
    {
        public override async Task<bool> ExecuteChecksAsync(InteractionContext ctx)
        {
            using var data = new Database("data.json").Initialize();

            // Return false if the user is banned; return true if otherwise.
            return await Task.FromResult(!data.UserBans.Exists(x => x.UserId == ctx.User.Id));
        }
    }
}