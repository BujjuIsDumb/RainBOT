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

using Newtonsoft.Json;

namespace RainBOT.SupportBot.Core
{
    /// <summary>
    ///     The configuration object for the bot.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        ///     Gets the bot token.
        /// </summary>
        [JsonProperty("token")]
        public string Token { get; private set; }

        /// <summary>
        ///     Gets the guild ID for the bot. Null for release versions.
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong? GuildId { get; private set; }

        /// <summary>
        ///     Loads a <see cref="Configuration"/> object from the file at <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The path to the config file.</param>
        /// <returns>A <see cref="Configuration"/> object loaded from the specified file.</returns>
        public static Configuration Load(string fileName)
            => JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(fileName));
    }
}