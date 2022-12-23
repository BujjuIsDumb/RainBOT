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
using DSharpPlus.SlashCommands;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The user configuration module.
    /// </summary>
    [SlashCommandGroup("user", "Configure user-side settings.")]
    public class User : ApplicationCommandModule
    {
        /// <summary>
        ///     The database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /user config command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="setting">The setting option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("configure", "Configure user-side settings.")]
        public async Task UserConfigureAsync(InteractionContext ctx,
            [Choice("Allow Vent Responses", 0)]
            [Choice("Bio Style", 1)]
            [Option("setting", "The setting to manage.")] long setting)
        {
        }
        
    }

    /// <summary>
    ///     The server configuration module.
    /// </summary>
    [SlashCommandGroup("server", "Configure server-side settings.")]
    [SlashCommandPermissions(Permissions.ManageGuild)]
    [GuildOnly]
    public class Server : ApplicationCommandModule
    {
        /// <summary>
        ///     The database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /server config command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="setting">The setting option.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("config", "Configure server-side settings.")]
        public async Task ServerConfigureAsync(InteractionContext ctx,
            [Choice("Vent Moderators", 0)]
            [Choice("Anonymous Venting", 1)]
            [Choice("Delete Verification Requests", 2)]
            [Choice("Create Vetting Thread", 3)]
            [Choice("Verification Form", 4)]
            [Option("setting", "The setting to manage.")] long setting)
        {

        }
    }
}