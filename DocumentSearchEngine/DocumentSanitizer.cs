using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentSearchEngine
{
    public class DocumentSanitizer
    {
        public Document PrepareDocument(string contents)
        {
            var cleaned = RemoveSpecialCharacters(contents);
            var lower = cleaned.ToLower();
            var splitted = lower.Split();
            var stemmer = new PorterStemmer();
            var stemmed = splitted.Select(stemmer.StemWord).ToArray();
            return new Document(contents, stemmed);
        }

        /// <summary>
        /// Removes any character from string except letters, digits and white spaces.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string
        /// </remarks>
        /// <param name="contents"></param>
        /// <returns></returns>
        private string RemoveSpecialCharacters(string contents)
        {
            var sb = new StringBuilder();
            foreach (char c in contents)
            {
                if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
                {
                    sb.Append(c);
                }
            }

            var singleSpaces = Regex.Replace(sb.ToString(), @"\s{2,}", "");
            return singleSpaces;
        }
    }
}