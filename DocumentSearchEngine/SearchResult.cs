using System.Collections;
using System.Collections.Generic;

namespace DocumentSearchEngine
{
    public class SearchResult
    {
        public Document Query { get; }
        public IReadOnlyCollection<(Document document, double similarity)> Results { get; }

        internal SearchResult(Document query, IReadOnlyCollection<(Document document, double similarity)> results)
        {
            this.Query = query;
            this.Results = results;
        }
    }
}
