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
            Dictionary<string, double> normalized = new Dictionary<string, double>();
            if (frequencies.Count > 0)
            {
                int max = frequencies.Max(f => f.count);
                normalized = frequencies.ToDictionary(f => f.key, f => f.count > 0 ? f.count / (double)max : 0);
            }
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
                var numberOfOccurences = termFrequencies.Count(tf => tf.ContainsKey(keyword));
                double idf = numberOfOccurences > 0
                    ? Math.Log10((double)numberOfDocuments / numberOfOccurences)
                    : 0;
                result.Add(keyword, idf);
            }
            return result;
        }
    }
}