using Quartz;

namespace Taye.Jobs
{
    [DisallowConcurrentExecution]
    public class TestJob : IJob
    {
        private readonly ILogger<TestJob> _logger;
        public TestJob(ILogger<TestJob> logger)
        {
            _logger = logger;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("test job!");
            return Task.CompletedTask;
        }
    }
}
