using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.Services
{
     public interface IJobService
    {
        void FireAndForgetJob();
        void RecurringJob();
        void DelayJob();
        void ContinuationJob();
    }
}
