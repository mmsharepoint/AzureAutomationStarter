
namespace AzureAutomationStarterRest.Models
{
    public class AutomationJob   
    {        
        public string id { get; set; }
        public string name {  get; set; }
        public string type { get; set; }
        public AutomationJobProperties properties { get; set; }
    }
    public class AutomationJobProperties
    {
        public string jobId { get; set; }
        public string provisioningState { get; set; }
        public string status { get; set; }
    }
}
