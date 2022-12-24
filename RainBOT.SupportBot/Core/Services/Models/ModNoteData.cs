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

namespace RainBOT.SupportBot.Core.Services.Models
{
    /// <summary>
    ///     Represents a mod note.
    /// </summary>
    public class ModNoteData
    {
        /// <summary>
        ///     Gets or sets the mod note id.
        /// </summary>
        [JsonProperty("mod_note_id")]
        public ulong ModNoteId { get; set; } = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds();

        /// <summary>
        ///     Gets or sets the ID of the user who this note is for.
        /// </summary>
        [JsonProperty("user_id")]
        public ulong UserId { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the ID of the user who created this note.
        /// </summary>
        [JsonProperty("creator_user_id")]
        public ulong CreatorUserId { get; set; } = 0;

        /// <summary>
        ///     Gets or sets the subject of the note.
        /// </summary>
        [JsonProperty("subject")]
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the note text.
        /// </summary>
        [JsonProperty("note")]
        public string Note { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets when the note was created.
        /// </summary>
        [JsonProperty("creation_timestmap")]
        public DateTimeOffset CreationTimestamp { get; set; } = DateTimeOffset.UnixEpoch;
    }
}