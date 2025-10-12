using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.Services;
using Microsoft.Extensions.Logging;

namespace Application.Service
{
    public class JobService : IJobService
    {

        private readonly ILogger _logger;
        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
        }



        public void ContinuationJob()
        {
            _logger.LogInformation("ContinuationJob");
        }

        public void DelayJob()
        {
            _logger.LogInformation("Delay Job");
        }

        public void FireAndForgetJob()
        {
            _logger.LogInformation("Fire and Forget Job");
        }

        public void RecurringJob()
        {
            _logger.LogInformation("Recuring Job");
        }

    }
}
