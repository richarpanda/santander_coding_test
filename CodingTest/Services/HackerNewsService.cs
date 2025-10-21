using CodingTest.Models;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;

namespace CodingTest.Services
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly ILogger<HackerNewsService> _logger;
        private readonly SemaphoreSlim _semaphore;

        
        private const string BestStoriesIdsKey = "BestStoriesIds";
        private const int BestStoriesIdsCacheDurationMinutes = 5;
        private const int StoryCacheDurationMinutes = 5;
        private const int MaxConcurrentRequests = 10; 

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache, ILogger<HackerNewsService> logger)
        {
            _logger = logger;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _semaphore = new SemaphoreSlim(MaxConcurrentRequests, MaxConcurrentRequests);
        }

        public async Task<List<StoryResponse>> GetBestStoriesAsync(int count, CancellationToken cancellationToken)
        {
            var storyIds = await GetBestStoryIdsAsync(cancellationToken);

            var topStoryIds = storyIds.Take(count).ToList();

            _logger.LogInformation("Fetching details for {Count} stories from Hacker News API", topStoryIds.Count);

            var storyTasks = topStoryIds.Select(id => GetStoryDetailsAsync(id, cancellationToken));
            var stories = await Task.WhenAll(storyTasks);

            var validStories = stories
                .Where(s => s != null)
                .OrderByDescending(s => s!.Score)
                .Select(s => s!)
                .ToList();

            _logger.LogInformation("Successfully processed {Count} out of {Total} requested stories",
                validStories.Count, topStoryIds.Count);

            return validStories;
        }

        private async Task<List<int>> GetBestStoryIdsAsync(CancellationToken cancellationToken)
        {
            // Try get data from cache
            if (_cache.TryGetValue(BestStoriesIdsKey, out List<int>? cachedIds) && cachedIds != null)
            {
                _logger.LogDebug("Retrieved {Count} story IDs from cache", cachedIds.Count);
                return cachedIds;
            }

            // If not, get data from api
            _logger.LogInformation("Cache miss - Fetching story IDs from Hacker News API");

            var response = await _httpClient.GetAsync("beststories.json", cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var storyIds = JsonSerializer.Deserialize<List<int>>(content) ?? new List<int>();

            _logger.LogInformation("Retrieved {Count} story IDs from Hacker News API", storyIds.Count);

            // Saves cache
            var cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(BestStoriesIdsCacheDurationMinutes)
            };

            _cache.Set(BestStoriesIdsKey, storyIds, cacheOptions);
            _logger.LogDebug("Cached story IDs for {Duration} minutes", BestStoriesIdsCacheDurationMinutes);

            return storyIds;
        }

        private async Task<StoryResponse?> GetStoryDetailsAsync(int storyId, CancellationToken cancellationToken)
        {
            var cacheKey = $"Story_{storyId}";

            if (_cache.TryGetValue(cacheKey, out StoryResponse? cachedStory) && cachedStory != null)
            {
                _logger.LogDebug("Retrieved story {StoryId} from cache", storyId);
                return cachedStory;
            }

            try
            {
                await _semaphore.WaitAsync(cancellationToken);

                try
                {
                    _logger.LogDebug("Fetching story {StoryId} from Hacker News API", storyId);

                    var response = await _httpClient.GetAsync($"item/{storyId}.json", cancellationToken);
                    response.EnsureSuccessStatusCode();

                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    var story = JsonSerializer.Deserialize<HackerNewsStory>(content);

                    if (story == null)
                    {
                        _logger.LogWarning("Story {StoryId} returned null from API", storyId);
                        return null;
                    }

                    var storyResponse = MapToStoryResponse(story);

                    var cacheOptions = new MemoryCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(StoryCacheDurationMinutes)
                    };

                    _cache.Set(cacheKey, storyResponse, cacheOptions);
                    _logger.LogDebug("Cached story {StoryId} for {Duration} minutes",
                        storyId, StoryCacheDurationMinutes);

                    return storyResponse;
                }
                finally
                {
                    _semaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error fetching story {StoryId}", storyId);
                return null;
            }
        }

        private StoryResponse MapToStoryResponse(HackerNewsStory story)
        {
            return new StoryResponse
            {
                Title = story.Title ?? string.Empty,
                Uri = story.Url ?? string.Empty,
                PostedBy = story.By ?? string.Empty,
                Time = DateTimeOffset.FromUnixTimeSeconds(story.Time)
                    .ToString("yyyy-MM-ddTHH:mm:sszzz"),
                Score = story.Score,
                CommentCount = story.Descendants
            };
        }
    }
}
