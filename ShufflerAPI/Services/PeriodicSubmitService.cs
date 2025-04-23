namespace ShufflerAPI.Services
{
    public class PeriodicSubmitService : BackgroundService
    {
        private int _executionCount = 0;
        private readonly SubmitService _submitService;

        public PeriodicSubmitService(SubmitService submitService)
        {
            _submitService = submitService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new PeriodicTimer(_submitService.SubmitPeriod);
            while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _executionCount++;
                    Console.WriteLine($"Executed PeriodicHostedService - Count: {_executionCount}");
                    await _submitService.SubmitDataToCounter();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Periodic submit failed to execute: {ex.Message}");
                }
            }
        }
    }
}
