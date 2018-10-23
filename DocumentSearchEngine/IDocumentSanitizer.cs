using System.Collections.Generic;

namespace DocumentSearchEngine
{
    public interface IDocumentSanitizer
    {
        Document PrepareDocument(string contents, IReadOnlyCollection<string> keywords);
    }
}