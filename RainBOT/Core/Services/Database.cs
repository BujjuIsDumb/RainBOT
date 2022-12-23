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
using RainBOT.Core.Services.Models;

namespace RainBOT.Core.Services
{
    public class Database : IDisposable
    {
        [JsonProperty("users")]
        public List<UserData> Users { get; set; } = new();

        [JsonProperty("guilds")]
        public List<GuildData> Guilds { get; set; } = new();

        [JsonProperty("reports")]
        public List<ReportData> Reports { get; set; } = new();

        [JsonProperty("bans")]
        public List<BanData> Bans { get; set; } = new();

        public Database(string fileName) => FileName = fileName;

        [JsonIgnore]
        public string FileName { get; set; }

        public Database Initialize()
        {
            // Load the database.
            var loaded = JsonConvert.DeserializeObject<Database>(File.ReadAllText(FileName));

            Users = loaded.Users;
            Guilds = loaded.Guilds;
            Reports = loaded.Reports;
            Bans = loaded.Bans;

            return this;
        }

        public void Update()
            => File.WriteAllText(FileName, JsonConvert.SerializeObject(this, Formatting.Indented));

        void IDisposable.Dispose()
        {
            GC.SuppressFinalize(this);
            Users.Clear();
            Guilds.Clear();
            Reports.Clear();
            Bans.Clear();
        }
    }
}