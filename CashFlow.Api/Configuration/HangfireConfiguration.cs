using Hangfire;
using CashFlow.Application.Interfaces;

namespace CashFlow.Api.Configuration
{
    public static class HangfireConfiguration
    {
        public static void ConfigureRecurringJobs(this IApplicationBuilder app)
        {
            RecurringJob.AddOrUpdate<IRecTransactionService>(
                "process-recurring-transactions",
                service => service.ProcessPendingTransactionsAsync(),
                Cron.Daily(5, 0)
                );

            RecurringJob.AddOrUpdate<ICurrencyService>(
                "fetch-nbp-rates",
                service => service.SyncRatesAsync(),
                Cron.Daily(12, 0)
                );
        }
    }
}
