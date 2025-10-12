using Application.Interface.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace CleanArct_WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobTestController : ControllerBase
    {
        private readonly IJobService _jobService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;

        public JobTestController(IJobService jobService, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _jobService = jobService;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }


        [HttpGet("FireAndForget")]
        public ActionResult FireAndForgetJob()
        {
            _backgroundJobClient.Enqueue(() => _jobService.FireAndForgetJob());
            return Ok();
        }

        [HttpGet("DelayJob")]
        public ActionResult CreateDelayJob()
        {
            _backgroundJobClient.Schedule(() => _jobService.DelayJob(), TimeSpan.FromSeconds(30));
            return Ok();
        }

        [HttpGet("RecurringJob")]
        public ActionResult RecurringJob()
        {
            _recurringJobManager.AddOrUpdate("jobId", () => _jobService.RecurringJob(), Cron.Monthly);
            return Ok();
        }


        [HttpGet("ContinuationJob")]
        public ActionResult CreateContinuationJob()
        {
            var ParentJobId = _backgroundJobClient.Enqueue(() => _jobService.FireAndForgetJob());
            _backgroundJobClient.ContinueJobWith(ParentJobId, () => _jobService.ContinuationJob());

            return Ok();
        }


    }
}
