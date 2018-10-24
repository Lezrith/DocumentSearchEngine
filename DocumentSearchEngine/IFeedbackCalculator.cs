using System.Collections.Generic;

namespace DocumentSearchEngine
{
    internal interface IFeedbackCalculator
    {
        Document CalculateFeedback(Document document, IEnumerable<Document> releventDocuments, IEnumerable<Document> irrelevantDocuments);
    }
}