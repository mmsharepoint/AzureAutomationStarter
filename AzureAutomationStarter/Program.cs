using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Automation;
using Azure.ResourceManager.Automation.Models;
using Microsoft.Extensions.Configuration;

namespace AzureAutomationStarter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            // For Dev and local env
            Environment.SetEnvironmentVariable("AZURE_TENANT_ID", config["AZURE_TENANT_ID"]);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_ID", config["AZURE_CLIENT_ID"]);
            Environment.SetEnvironmentVariable("AZURE_CLIENT_SECRET", config["AZURE_CLIENT_SECRET"]);

            ArmClient client = new ArmClient(new DefaultAzureCredential(true)); // Enable interactive as well
            
            var automationAcc = client.GetAutomationAccountResource(new ResourceIdentifier(config["automationAccount"]));
            var automationJobs = automationAcc.GetAutomationJobs();
            
            var jobParameters = new Azure.ResourceManager.Automation.Models.AutomationJobCreateOrUpdateContent()
            {
                RunbookName = config["runbookName"],
                RunOn = ""
            };

            string alias = "MMTeamNo26";
            jobParameters.Parameters.Add("displayName", "MM Team No 26");
            jobParameters.Parameters.Add("alias", alias);
            jobParameters.Parameters.Add("teamDescription", "MM Team No 26 Descr");
            jobParameters.Parameters.Add("teamOwner", config["teamOwner"]);

            var automationJob = automationJobs.CreateOrUpdate(Azure.WaitUntil.Started, $"Creation of {alias}", jobParameters);

            Console.WriteLine($"Job Started ${automationJob.Value.Id}");

            int count = 0;
            while (count < 10)
            {
                var newAutomationJob = automationAcc.GetAutomationJob($"Creation of {alias}");
                if (newAutomationJob.Value.Data.Status == AutomationJobStatus.Completed)
                {
                    Console.WriteLine($"Job Ended {automationJob.Value.Id}");
                    break;   
                }
                if (newAutomationJob.Value.Data.Status == AutomationJobStatus.Failed ||
                    newAutomationJob.Value.Data.Status == AutomationJobStatus.Stopped)
                {
                    Console.WriteLine($"Job Ended unsuccesfully {automationJob.Value.Id}");
                    break;
                }
                count++;
                Thread.Sleep(30000);
            }
            Console.ReadLine();
        }
    }
}
