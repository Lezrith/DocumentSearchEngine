using System.Collections.Generic;

namespace DocumentSearchEngine
{
    public class Document
    {
        private readonly string[] contents;

        internal Document(string rawContents, string[] contents)
        {
            this.RawContents = rawContents;
            this.contents = contents;
        }

        public IEnumerable<string> Contents
        {
            get { return contents; }
        }

        public int Length => this.contents.Length;
        public string RawContents { get; }
    }
}