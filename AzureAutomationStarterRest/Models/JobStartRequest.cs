using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAutomationStarterRest.Models
{
    public class JobStartRequest
    {
        public JobProperties properties { get; set; }
        
    }
    public class JobProperties
    {
        public RunbookProperties runbook { get; set; }
        public JobParameters parameters { get; set; }
        public string runOn { get; set; }

    }
    public class JobParameters
    {
        public string displayName { get; set; }
        public string groupAlias { get; set; }
        public string teamDescription { get; set; }
        public string teamOwner { get; set; }
    }

    public class RunbookProperties
    {
        public string name { get; set; }

    }
}
