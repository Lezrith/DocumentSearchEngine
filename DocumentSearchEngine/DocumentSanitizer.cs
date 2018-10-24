using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentSearchEngine
{
    public class DocumentSanitizer : IDocumentSanitizer
    {
        public Document PrepareDocument(string contents, IReadOnlyCollection<string> keywords)
        {
            var singleLine = contents.Replace("\r", "").Replace("\n", " ");
            var cleaned = RemoveSpecialCharacters(singleLine);
            var lower = cleaned.ToLower();
            var splitted = lower.Split();
            var stemmer = new PorterStemmer();
            var stemmed = splitted.Select(stemmer.StemWord).ToArray();
            var vector = this.DocumentToVector(stemmed, keywords);
            return new Document(contents, stemmed, vector);
        }

        private double[] DocumentToVector(IReadOnlyCollection<string> contents, IReadOnlyCollection<string> keywords)
        {
            var vector = keywords.Select(k => contents.Count(c => c.Equals(k)));
            var max = vector.Max();
            double[] normalized = max > 0 ? vector.Select(x => x / (double)max).ToArray() : new double[keywords.Count];
            return normalized;
        }

        /// <summary>
        /// Removes any character from string except letters, digits and white spaces.
        /// </summary>
        /// <remarks>https://stackoverflow.com/questions/1120198/most-efficient-way-to-remove-special-characters-from-string</remarks>
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

            var singleSpaces = Regex.Replace(sb.ToString(), @"\s{2,}", " ");
            return singleSpaces;
        }
    }
}
