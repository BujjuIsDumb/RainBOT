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
using RainBOT.SupportBot.Core.Services.Models;

namespace RainBOT.SupportBot.Core.Services
{
    public class Database : IDisposable
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Database"/> class.
        /// </summary>
        /// <param name="fileName">The database file.</param>
        public Database(string fileName) => FileName = fileName;

        /// <summary>
        ///     Gets or sets the list of prompts.
        /// </summary>
        [JsonProperty("prompts")]
        public List<PromptData> Prompts { get; set; } = new();

        /// <summary>
        ///     Gets or sets the database file.
        /// </summary>
        [JsonIgnore]
        public string FileName { get; set; }

        /// <summary>
        ///     Initializes the database.
        /// </summary>
        /// <returns>The initialized database.</returns>
        public Database Initialize()
        {
            // Load the database.
            var loaded = JsonConvert.DeserializeObject<Database>(File.ReadAllText(FileName));

            Prompts = loaded.Prompts;

            return this;
        }

        /// <summary>
        ///     Updates the database.
        /// </summary>
        public void Update()
            => File.WriteAllText(FileName, JsonConvert.SerializeObject(this, Formatting.Indented));

        /// <summary>
        ///     Disposes of the database.
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Prompts.Clear();
        }
    }
}