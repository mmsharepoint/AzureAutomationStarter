using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AzureAutomationStarterRest.Models;
using System.Text;

namespace AzureAutomationStarterRest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var tokenCredential = new DefaultAzureCredential(true);
            var accessToken = tokenCredential.GetToken(new Azure.Core.TokenRequestContext(["https://management.azure.com/user_impersonation"])).Token;
            Console.WriteLine($"Token: {accessToken}");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string accUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}?api-version=2023-11-01";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var automationAccountResult = client.GetFromJsonAsync<AutomationAccount>(accUrl).Result; // Task

            Console.WriteLine(automationAccountResult.id);

            string groupAlias = "MMTeamNo30";
            JobStartRequest request = new JobStartRequest()
            {
                properties = new JobProperties()
                {
                    runbook = new RunbookProperties()
                    {
                        name = config["runbookName"]
                    },
                    parameters = new JobParameters()
                    {
                        groupAlias = groupAlias,
                        displayName = "MM Team No 30",
                        teamDescription = "MM Team No 30S Descr",
                        teamOwner = config["teamOwner"]
                    },
                    runOn = ""
                }                
            };
            var jsonRequest = JsonSerializer.Serialize(request);

            string jobStartUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}/jobs/Creation{groupAlias}?api-version=2023-11-01";

            var req = new HttpRequestMessage(HttpMethod.Put, jobStartUrl);
            req.Content = new StringContent(jsonRequest, Encoding.UTF8);
            req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var jobStartResult = client.SendAsync(req).Result;
            jobStartResult.EnsureSuccessStatusCode();
            var content = jobStartResult.Content.ReadAsStringAsync().Result;

            int count = 0;
            while (count < 10)
            {
                string jobName = $"Creation{groupAlias}";
                string jobCheckUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}/jobs/{jobName}?api-version=2023-11-01";
                var checkReq = new HttpRequestMessage(HttpMethod.Get, jobStartUrl);
                req.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var jobCheckResult = client.SendAsync(checkReq).Result;
                jobCheckResult.EnsureSuccessStatusCode();
                var ceckContent = jobCheckResult.Content.ReadAsStringAsync().Result;
                var newAutomationJob = JsonSerializer.Deserialize<AutomationJob>(ceckContent);
                if (newAutomationJob.properties.status == "Completed")
                {
                    Console.WriteLine($"Job Ended {newAutomationJob.properties.jobId}");
                    break;
                }
                if (newAutomationJob.properties.status == "Failed" ||
                    newAutomationJob.properties.status == "Stopped")
                {
                    Console.WriteLine($"Job Ended unsuccesful {newAutomationJob.properties.jobId}");
                    break;
                }
                count++;
                Thread.Sleep(30000);
            }

            Console.ReadLine();
        }
    }
}