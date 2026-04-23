using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;

namespace web.Services
{
    public class StoryCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StoryCleanupService> _logger;

        public StoryCleanupService(IServiceProvider serviceProvider, ILogger<StoryCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("StoryCleanupService running at: {time}", DateTimeOffset.Now);

                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

                        var expiredStories = await dbContext.Stories
                            .Where(s => s.ExpiresAt < DateTime.UtcNow)
                            .ToListAsync(stoppingToken);

                        if (expiredStories.Any())
                        {
                            foreach (var story in expiredStories)
                            {
                                // Delete file
                                if (!string.IsNullOrEmpty(story.MediaUrl))
                                {
                                    var filePath = Path.Combine(env.WebRootPath, story.MediaUrl.TrimStart('/'));
                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(filePath);
                                    }
                                }
                                dbContext.Stories.Remove(story);
                            }

                            await dbContext.SaveChangesAsync(stoppingToken);
                            _logger.LogInformation($"Cleaned up {expiredStories.Count} expired stories.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up stories.");
                }

                // Run every 1 hour
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
