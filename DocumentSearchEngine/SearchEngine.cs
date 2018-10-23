using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    public class SearchEngine
    {
        private readonly List<Document> documents;
        private readonly HashSet<string> keywords;
        private readonly IDocumentSanitizer sanitizer;
        private readonly PorterStemmer stemmer;
        private readonly ICollection<double> inverseDocumentFrequencies;

        public SearchEngine(string[] keywords, IDocumentSanitizer sanitizer)
        {
            this.sanitizer = sanitizer;
            this.stemmer = new PorterStemmer();
            this.documents = new List<Document>();
            this.keywords = new HashSet<string>();
            this.inverseDocumentFrequencies = new List<double>();
            foreach (var keyword in keywords)
            {
                var stemmed = this.stemmer.StemWord(keyword);
                this.keywords.Add(stemmed);
            }
        }

        public void AddDocument(string rawContent)
        {
            var document = this.sanitizer.PrepareDocument(rawContent, this.keywords);
            this.documents.Add(document);
            this.RecalculateInverseTermFrequency();
        }

        private void RecalculateInverseTermFrequency()
        {
            this.inverseDocumentFrequencies.Clear();
            double[][] vectors = this.documents.Select(d => d.Vector.ToArray()).ToArray();
            for (int i = 0; i < this.keywords.Count; i++)
            {
                int numberOfOccurences = 0;
                for (int j = 0; j < vectors.Length; j++)
                {
                    if (vectors[j][i] > 0)
                    {
                        numberOfOccurences++;
                    }
                }
                var idf = numberOfOccurences > 0 ? Math.Log10(this.documents.Count / (double)numberOfOccurences) : 0;
                this.inverseDocumentFrequencies.Add(idf);
            }
        }

        public SearchResult Search(string query)
        {
            var preparedQuery = this.sanitizer.PrepareDocument(query, this.keywords);
            if (preparedQuery.Length == 0)
            {
                throw new ArgumentException("Query does not contain any known keywords");
            }
            var queryVector = preparedQuery.Vector.Times(this.inverseDocumentFrequencies);
            var results = this.documents.Select(d =>
                {
                    var similarity = d.Vector.Times(this.inverseDocumentFrequencies).Cosine(queryVector);
                    return (d, similarity);
                })
                .OrderByDescending(x => x.similarity)
                .ToList();
            return new SearchResult(preparedQuery, results);
        }
    }
}