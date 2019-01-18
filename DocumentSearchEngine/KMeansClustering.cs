using System;
using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    internal class KMeansClustering : IClusteringStrategy
    {
        private readonly int k;
        private readonly int iterations;
        private readonly Random rng;

        public KMeansClustering(int k, int iterations, Random rng)
        {
            this.k = k > 0 ? k : throw new ArgumentException($"k cannot be less than 1: {k}");
            this.iterations = iterations > 0
                ? iterations
                : throw new ArgumentException($"Number of iterations cannot be less than 1: {iterations}");
            this.rng = rng ?? throw new ArgumentNullException(nameof(rng));
        }

        IDictionary<string, List<Document>> IClusteringStrategy.Cluster(IReadOnlyCollection<Document> documents,
            IReadOnlyCollection<double> inverseDocumentFrequencies)
        {
            var seedDocuments = documents
                .OrderBy(_ => this.rng.NextDouble())
                .Take(this.k);
            var seed = seedDocuments
                .Zip(Enumerable.Range(1, this.k), (d, i) => (document: d, index: i))
                .ToDictionary(x => $"group{x.index}", x => x.document.Vector.Times(inverseDocumentFrequencies).ToList());
            var groups = new Dictionary<string, List<Document>>();
            for (int i = 0; i < this.iterations; i++)
            {
                groups = seed.ToDictionary(kv => kv.Key, _ => new List<Document>());
                foreach (var document in documents)
                {
                    var similarities = seed
                        .Select(kv =>
                        {
                            var similarity = document.Vector.Times(inverseDocumentFrequencies).Cosine(kv.Value);
                            return (kv.Key, similarity);
                        })
                        .ToList();
                    var group = similarities.OrderByDescending(x => x.similarity)
                        .First()
                        .Key;
                    groups[group].Add(document);
                }
                seed = groups.ToDictionary(kv => kv.Key, kv => this.Centroid(kv.Value, inverseDocumentFrequencies));
            }
            return groups;
        }

        private List<Double> Centroid(List<Document> documents, IReadOnlyCollection<double> inverseDocumentFrequencies)
        {
            var sums = documents.Aggregate(
                new double[inverseDocumentFrequencies.Count],
                (acc, d) => acc.Add(d.Vector.Times(inverseDocumentFrequencies)).ToArray());
            return sums.Times(1d / documents.Count).ToList();
        }
    }
}
