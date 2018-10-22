using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    public class SearchEngine
    {
        private readonly List<Document> documents;
        private readonly IDictionary<Document, IReadOnlyCollection<double>> documentVectors;
        private readonly HashSet<string> keywords;
        private readonly DocumentSanitizer sanitizer;
        private readonly PorterStemmer stemmer;
        private IReadOnlyDictionary<string, double> inverseDocumentFrequencies;

        public SearchEngine()
        {
            this.sanitizer = new DocumentSanitizer();
            this.stemmer = new PorterStemmer();
            this.documents = new List<Document>();
            this.keywords = new HashSet<string>();
            this.inverseDocumentFrequencies = new Dictionary<string, double>();
            this.documentVectors = new Dictionary<Document, IReadOnlyCollection<double>>();
        }

        public void AddDocument(Document document) => this.documents.Add(document);

        public void AddKeyword(string keyword) => this.keywords.Add(this.stemmer.StemWord(keyword));

        public void RecalculateTermFrequency()
        {
            this.documentVectors.Clear();
            var termFrequencyMap = new List<(Document document, IReadOnlyDictionary<string, double> termFrequencies)>();
            foreach (var document in this.documents)
            {
                var tf = TermFrequencyCalculator.CalculateTermFrequency(document, this.keywords);
                termFrequencyMap.Add((document, tf));
            }
            this.inverseDocumentFrequencies = TermFrequencyCalculator.CalculateInverseTermFrequency(
                this.keywords,
                termFrequencyMap.Select(dtf => dtf.termFrequencies).ToList());
            foreach (var (document, termFrequencies) in termFrequencyMap)
            {
                IEnumerable<double> documentVector = ToVector(termFrequencies);
                this.documentVectors.Add(document, documentVector.ToList());
            }
        }

        public IEnumerable<(Document document, double similarity)> Search(string query)
        {
            var preparedQuery = this.sanitizer.PrepareDocument(query);
            var tf = TermFrequencyCalculator.CalculateTermFrequency(preparedQuery, this.keywords);
            if (tf.Count == 0)
            {
                throw new ArgumentException("Query does not contain any known keywords");
            }
            var queryVector = this.ToVector(tf);
            return this.documents.Select(d =>
                {
                    var documentVector = this.documentVectors[d];
                    var similarity = documentVector.Cosine(queryVector);
                    return (d, similarity);
                })
                .OrderByDescending(x => x.similarity);
        }

        private IEnumerable<double> ToVector(IReadOnlyDictionary<string, double> termFrequencies)
        {
            return this.inverseDocumentFrequencies.Select(idf => termFrequencies.GetValueOrDefault(idf.Key) * idf.Value);
        }
    }
}