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

using DSharpPlus.Entities;
using Newtonsoft.Json;

namespace RainBOT.Core.Entities.Services
{
    public class Config
    {
        [JsonProperty("token")]
        public string Token { get; private set; }

        [JsonProperty("source_url")]
        public string SourceUrl { get; private set; }

        [JsonProperty("support_url")]
        public string SupportUrl { get; private set; }

        [JsonProperty("invite_url")]
        public string InviteUrl { get; private set; }

        [JsonProperty("guild_id")]
        public ulong? GuildId { get; private set; }

        [JsonProperty("status")]
        public string Status { get; private set; }

        [JsonProperty("status_type")]
        public ActivityType StatusType { get; private set; }

        public Config(string fileName) => FileName = fileName;

        [JsonIgnore]
        public string FileName { get; set; }

        public void Initialize()
        {
            // Load the config.
            var loaded = JsonConvert.DeserializeObject<Config>(File.ReadAllText(FileName));

            Token = loaded.Token;
            SourceUrl = loaded.SourceUrl;
            SupportUrl = loaded.SupportUrl;
            InviteUrl = loaded.InviteUrl;
            GuildId = loaded.GuildId;
            Status = loaded.Status;
            StatusType = loaded.StatusType;
        }
    }
}