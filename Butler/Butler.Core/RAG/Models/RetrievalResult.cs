using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core.RAG.Models
{
    //Result of a vector search: a document chunk and its similarity score(0..1)(the higher the better)
    public record RetrievalResult(DocumentChunk DocumentChunk, float Score);
}
