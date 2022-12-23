﻿// This file is from RainBOT.
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
using DSharpPlus.SlashCommands.Attributes;
using RainBOT.Core.Services;

namespace RainBOT.Modules
{
    /// <summary>
    ///     The mental health module.
    /// </summary>
    public class MentalHealth : ApplicationCommandModule
    {
        /// <summary>
        ///     Sets the database service.
        /// </summary>
        public Database Data { private get; set; }

        /// <summary>
        ///     The /vent command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <param name="anonymous">The anonymous option.</param>
        /// <param name="tw">The tw option.</param>
        /// <returns></returns>
        [SlashCommand("vent", "Create a vent.")]
        [GuildOnly]
        [SlashCooldown(5, 300, SlashCooldownBucketType.User)]
        [SlashRequireBotPermissions(Permissions.AccessChannels | Permissions.SendMessages | Permissions.EmbedLinks)]
        public async Task VentAsync(InteractionContext ctx,
            [Option("anonymous", "Whether or not the vent will show you name to non-moderators.")] bool anonymous,
            [Option("tw", "Whether or not the vent contains potentially triggering content.")] bool tw)
        {

        }

        /// <summary>
        ///     The /hotlines command.
        /// </summary>
        /// <param name="ctx">Context for the interaction.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        [SlashCommand("hotlines", "Get a list of hotlines.")]
        public async Task HotlinesAsync(InteractionContext ctx)
        {
           
        }
    }
}