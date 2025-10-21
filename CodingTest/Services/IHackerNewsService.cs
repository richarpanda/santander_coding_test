using CodingTest.Models;

namespace CodingTest.Services
{
    public interface IHackerNewsService
    {
        Task<List<StoryResponse>> GetBestStoriesAsync(int count, CancellationToken cancellationToken);
    }
}
