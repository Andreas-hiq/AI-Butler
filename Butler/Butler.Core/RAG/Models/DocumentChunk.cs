using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core.RAG.Models
{
    //The smallest domain object we need to represent a chunk of a document
    public record DocumentChunk(string Source, string Content);
}
