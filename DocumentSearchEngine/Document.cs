using System.Collections.Generic;

namespace DocumentSearchEngine
{
    public class Document
    {
        private readonly string[] contents;
        private readonly double[] vector;

        internal Document(string rawContents, string[] contents, double[] vector)
        {
            this.RawContents = rawContents;
            this.contents = contents;
            this.vector = vector;
        }

        public IEnumerable<string> Contents => this.contents;

        public IEnumerable<double> Vector => this.vector;

        public int Length => this.contents.Length;

        public string RawContents { get; }
    }
}
