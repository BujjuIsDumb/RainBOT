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

namespace RainBOT.Core.Services.Models
{
    /// <summary>
    ///     Represents a guild.
    /// </summary>
    public class GuildData
    {
        /// <summary>
        ///     Gets or sets the ID of the guild.
        /// </summary>
        [JsonProperty("guild_id")]
        public ulong GuildId { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the vent moderators of the guild.
        /// </summary>
        [JsonProperty("vent_moderators")]
        public ulong[] VentModerators { get; set; } = Array.Empty<ulong>();

        /// <summary>
        ///     Gets or sets whether users can vent anonymously.
        /// </summary>
        [JsonProperty("anonymous_venting")]
        public bool AnonymousVenting { get; set; } = true;

        /// <summary>
        ///     Gets or sets whether to delete verification requests.
        /// </summary>
        [JsonProperty("delete_verification_requests")]
        public bool DeleteVerificationRequests { get; set; } = false;

        /// <summary>
        ///     Gets or sets whether to create a vetting thread.
        /// </summary>
        [JsonProperty("create_vetting_thread")]
        public bool CreateVettingThread { get; set; } = true;

        /// <summary>
        ///     Gets or sets the verification form questions.
        /// </summary>
        [JsonProperty("verification_form_questions")]
        public string[] VerificationFormQuestions { get; set; } = Array.Empty<string>();
    }
}