using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butler.Core.RAG.Interfaces
{
    public interface IIngestService
    {
        Task IngestDocumentAsync(string filePath, CancellationToken ct = default);
        Task IngestFolderAsync(string folderPath, CancellationToken ct = default);
    }
}
