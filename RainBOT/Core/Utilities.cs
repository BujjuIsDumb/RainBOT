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

namespace RainBOT.Core
{
    // This file can be deleted once my PR (#1420) is merged into DSharpPlus.

    /// <summary>
    ///     Utilities used for the bot.
    /// </summary>
    public class Utilities
    {
        /// <summary>
        ///     Gets the command mention string.
        /// </summary>
        /// <param name="client">The client to fetch the mention from.</param>
        /// <param name="name">The command name.</param>
        /// <returns>The mention string for the specified command.</returns>
        public static string GetCommandMention(DiscordClient client, string name)
        {
            foreach (var registeredCommand in client.GetSlashCommands().RegisteredCommands)
            {
                // Find the command with the specified name.
                var command = registeredCommand.Value.ToList().Find(x => x.Name == name.Split(' ')[0]);

                if (command is not null)
                    return $"</{name}:{command.Id}>";
            }

            // Return without ID if the command with the specified name is not found.
            return $"</{name}:0>";
        }
    }
}