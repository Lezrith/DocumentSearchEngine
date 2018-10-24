using System.Collections.Generic;

namespace DocumentSearchEngine
{
    public interface ISearchEngine
    {
        SearchResult Search(string query);
        SearchResult SearchWithFeedback(string query, IReadOnlyCollection<string> positiveFeedback, IReadOnlyCollection<string> negativeFeedback);
    }
}