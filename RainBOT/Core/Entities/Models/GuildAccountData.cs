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

using Newtonsoft.Json;

namespace RainBOT.Core.Entities.Models
{
    public class GuildAccountData
    {
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; } = 0;

        [JsonProperty("vent_moderators")]
        public ulong[] VentModerators { get; set; } = new ulong[0];

        [JsonProperty("anonymous_venting")]
        public bool AnonymousVenting { get; set; } = true;

        [JsonProperty("delete_verification_requests")]
        public bool DeleteVerificationRequests { get; set; } = false;

        [JsonProperty("create_vetting_thread")]
        public bool CreateVettingThread { get; set; } = true;

        [JsonProperty("verification_form_questions")]
        public Dictionary<string, string> VerificationFormQuestions { get; set; } = new();

        [JsonProperty("warnings")]
        public WarnData[] Warnings { get; set; } = new WarnData[0];
    }
}