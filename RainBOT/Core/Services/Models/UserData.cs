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
    ///     Represents a user.
    /// </summary>
    public class UserData
    {
        /// <summary>
        ///     Gets or sets the ID of the user.
        /// </summary>
        [JsonProperty("user_id")]
        public ulong UserId { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the fields in the user bio.
        /// </summary>
        [JsonProperty("bio_fields")]
        public BioField[] BioFields { get; set; } = Array.Empty<BioField>();

        /// <summary>
        ///     Gets or sets the color of the user's bio.
        /// </summary>
        [JsonProperty("bio_style")]
        public string BioStyle { get; set; } = "2F3136";

        /// <summary>
        ///     Gets or sets whether the user has opted out of vent responses.
        /// </summary>
        [JsonProperty("allow_vent_responses")]
        public bool AllowVentResponses { get; set; } = true;

        /// <summary>
        ///     Represents a field in a user's bio.
        /// </summary>
        public class BioField
        {
            /// <summary>
            ///     Gets or sets the name of the field.
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; } = string.Empty;

            /// <summary>
            ///     Gets or sets the value of the field.
            /// </summary>
            [JsonProperty("value")]
            public string Value { get; set; } = string.Empty;
        }
    }
}