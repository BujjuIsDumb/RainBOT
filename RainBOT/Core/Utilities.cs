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
    public class Utilities
    {
        public static string GetCommandMention(DiscordClient client, string name)
        {
            foreach (var registeredCommand in client.GetSlashCommands().RegisteredCommands)
            {
                // Find the command/command group.
                var command = registeredCommand.Value.ToList().Find(x => x.Name == name.Split(' ')[0]);

                if (command is not null)
                    return $"</{name}:{command.Id}>";
            }

            // Return without ID if the command/command group isn't found
            return $"</{name}:0>";
        }

        public static int CompareStrings(string string1, string string2)
        {
            int similarity = 0;

            foreach (char c in string1)
            {
                // Increase by 1 if the second string contains the character.
                // Increase by 2 if the second string contains the character in the same place.

                if (string2.Contains(c)) similarity++;
                if (string2.IndexOf(c) == string1.IndexOf(c)) similarity++;
            }

            return similarity * -1;
        }

        public static string CreateCustomId(string componentName)
        {
            // Create a custom ID.
            // Uses snowflakes to differentiate between uses.
            // Uses component name to differentiate between different components created at the same time.
            return $"{componentName}-{DateTimeOffset.Now.ToUnixTimeSeconds()}";
        }
    }
}