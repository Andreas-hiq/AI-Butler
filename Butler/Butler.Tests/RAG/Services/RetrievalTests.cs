using Butler.Core.RAG.Models;
using Butler.Core.RAG.Services;
using Moq;
using MockFactory = Butler.Tests.TestHelpers.MockFactory;

namespace Butler.Tests.RAG.Services;

public class RetrievalTests {
    [Fact]
    public async Task TestGetRelevantChunks() {
        //Setup
        const string query = "dummy text";
        float[] fakeEmbedding = [0.1f, 0.2f, 0.3f];

        List<DocumentChunk> expectedChunks = [
            new("0", "0st content", fakeEmbedding),
            new("1", "1st content", fakeEmbedding)
        ];

        Mock<IEmbedding> embeddingMock = MockFactory.CreateReturningAsync<IEmbedding, float[]>(
            embedding => embedding.Embed(query),
            fakeEmbedding
        );
        
        Mock<IVectorStore> vectorStoreMock = MockFactory.CreateReturning<IVectorStore, IReadOnlyList<RetrievalResult>>(
                vectorStore => vectorStore.Search(fakeEmbedding),
                expectedChunks.Select(chunk => new RetrievalResult(chunk, 0.9)).ToList()
        );
        
        Retrieval retrieval = new(embeddingMock.Object, vectorStoreMock.Object);
        
        //Call
        IReadOnlyList<DocumentChunk> result = await retrieval.GetRelevantChunks(query);
        
        //Assert
        Assert.Equal(expectedChunks, result);
    }
}