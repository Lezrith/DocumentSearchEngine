using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentSearchEngine
{
    public class DocumentWithGroupSanitizer : DocumentSanitizer
    {
        public override Document PrepareDocument(string contents, IReadOnlyCollection<string> keywords)
        {
            var lineEnd = contents.IndexOf("\n", StringComparison.Ordinal);
            var group = contents.Substring(0, lineEnd);
            var document = base.PrepareDocument(contents.Substring(lineEnd + 1), keywords);
            return new Document(group, document.RawContents, document.Contents.ToArray(), document.Vector.ToArray());
        }
    }
}
