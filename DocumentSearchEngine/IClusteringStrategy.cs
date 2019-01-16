using System;
using System.Collections.Generic;
using System.Text;

namespace DocumentSearchEngine
{
    internal interface IClusteringStrategy
    {
        IDictionary<string, List<Document>> Cluster(
            IReadOnlyCollection<Document> documents,
            IReadOnlyCollection<double> inverseDocumentFrequencies);
    }
}
