using System.Collections.Generic;
using System.Data.HashFunction.xxHash;
using System.Text;

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
            var hashFunction = xxHashFactory.Instance.Create();
            this.Hash = hashFunction.ComputeHash(Encoding.UTF8.GetBytes(rawContents)).AsBase64String();
        }

        public string Hash { get; }

        public IEnumerable<string> Contents => this.contents;

        public IEnumerable<double> Vector => this.vector;

        public int Length => this.contents.Length;

        public string RawContents { get; }
    }
}
