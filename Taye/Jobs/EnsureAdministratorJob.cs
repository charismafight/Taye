using Quartz;
using Taye.Data;

namespace Taye.Jobs
{
    public class EnsureAdministratorJob : IJob
    {
        private readonly SeedData _seedData;
        public EnsureAdministratorJob(SeedData seedData)
        {
            _seedData = seedData;
        }

        public Task Execute(IJobExecutionContext context)
        {
            _seedData.SeedAdminUser();
            return Task.CompletedTask;
        }
    }
}
