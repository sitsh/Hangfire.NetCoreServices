using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Hangfire.NetCoreServices.Common
{
    public class RandomHealthCheck
        : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (DateTime.UtcNow.Minute % 2 == 0)
            {
                return Task.FromResult(HealthCheckResult.Healthy());
            }

            return Task.FromResult(HealthCheckResult.Unhealthy(description: "failed"));
        }
    }
}
