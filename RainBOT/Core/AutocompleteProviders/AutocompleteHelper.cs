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

namespace RainBOT.Core.AutocompleteProviders
{
    /// <summary>
    ///     A helper for autocomplete providers.
    /// </summary>
    public static class AutocompleteHelper
    {
        /// <summary>
        ///     Compares two strings and returns a value indicating how similar they are.
        /// </summary>
        /// <param name="string1">The first string.</param>
        /// <param name="string2">The second string.</param>
        /// <returns>A value indicating how similar the specified strings are</returns>
        public static int CompareStrings(string string1, string string2)
        {
            int similarity = 0;

            foreach (char c in string1)
            {
                if (string2.Contains(c)) similarity++;
                if (string2.IndexOf(c) == string1.IndexOf(c)) similarity++;
            }

            return similarity * -1;
        }
    }
}