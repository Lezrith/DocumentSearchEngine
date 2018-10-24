using System.Collections.Generic;
using System.Linq;

namespace DocumentSearchEngine
{
    internal class RocchioFeedbackCalculator : IFeedbackCalculator
    {
        private const double alpha = 1;
        private const double beta = .75;
        private const double gamma = .15;

        public Document CalculateFeedback(
            Document document,
            IEnumerable<Document> releventDocuments,
            IEnumerable<Document> irrelevantDocuments)
        {
            var releventVector = releventDocuments
                .Aggregate(
                    document.Vector.Select(_ => .0),
                    (v, d) => v.Add(d.Vector))
                .ToList();
            var irrelevantVector = irrelevantDocuments
                .Aggregate(
                    document.Vector.Select(_ => .0),
                    (v, d) => v.Add(d.Vector))
                .ToList();
            var vectorWithFeedback = document.Vector
                .Times(alpha)
                .Add(releventVector.Times(beta / releventVector.Count))
                .Add(irrelevantVector.Times(gamma / releventVector.Count))
                .ToArray();
            return new Document(document.RawContents, document.Contents.ToArray(), vectorWithFeedback);
        }
    }
}