using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    internal static class TermFrequencyCalculator
    {
        public static IReadOnlyDictionary<string, double> CalculateTermFrequency(Document document, IReadOnlyCollection<string> keywords)
        {
            var presentKeywords = from word in document.Contents
                                  join keyword in keywords
                                  on word equals keyword
                                  select keyword;
            List<(string key, int count)> frequencies = presentKeywords
                .GroupBy(w => w)
                .Select(g => (g.Key, g.Count()))
                .ToList();
            int max = frequencies.Max(f => f.count);
            Dictionary<string, double> normalized = frequencies.ToDictionary(f => f.key, f => f.count > 0 ? f.count / (double)max : 0);
            return normalized;
        }

        internal static IReadOnlyDictionary<string, double> CalculateInverseTermFrequency(
            IEnumerable<string> keywords,
            IReadOnlyCollection<IReadOnlyDictionary<string, double>> termFrequencies)
        {
            var result = new Dictionary<string, double>();
            int numberOfDocuments = termFrequencies.Count;
            foreach (var keyword in keywords)
            {
                var occurences = termFrequencies.Where(tf => tf.ContainsKey(keyword));
                double idf = Math.Log10(occurences.Count() / numberOfDocuments);
                result.Add(keyword, idf);
            }
            return result;
        }
    }
}