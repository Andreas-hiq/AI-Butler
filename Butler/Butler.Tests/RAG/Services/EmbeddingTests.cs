using Butler.Core.RAG.Services;

namespace Butler.Tests.RAG.Services;

using Xunit;
using FluentAssertions;

public class EmbeddingTests : TestBase {
    
    [Theory]
    [InlineData("dummy text")]
    public async void Test_Embed(string text) {
        //TODO: Create variable "expected" depicted what SHOULD be returned.
        float[] expected = Enumerable.Repeat(0.1f, 1536).ToArray();
        Embedding embedding = new();
        float[] result = await embedding.Embed(text);
        result.Should().BeEquivalentTo(expected);
    }
}