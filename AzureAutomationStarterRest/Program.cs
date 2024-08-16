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
        static async Task Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration config = builder.Build();

            var tokenCredential = new DefaultAzureCredential(true);
            var accessToken = await tokenCredential.GetTokenAsync(new Azure.Core.TokenRequestContext(["https://management.azure.com/user_impersonation"]));
            Console.WriteLine($"Token: {accessToken.Token}");

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken.Token);
            string accUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}?api-version=2023-11-01";
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Not nesselarily needed here
            var automationAccountResult = await client.GetFromJsonAsync<AutomationAccount>(accUrl);
            Console.WriteLine(automationAccountResult.id);

            string groupAlias = "MMTeamNo32";
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
                        displayName = "MM Team No 32",
                        teamDescription = "MM Team No 32 Descr",
                        teamOwner = config["teamOwner"]
                    },
                    runOn = ""
                }                
            };
            var jsonRequest = JsonSerializer.Serialize(request);

            string jobStartUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}/jobs/Creation{groupAlias}?api-version=2023-11-01";

            var jobStartReq = new HttpRequestMessage(HttpMethod.Put, jobStartUrl);
            jobStartReq.Content = new StringContent(jsonRequest, Encoding.UTF8);
            jobStartReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            var jobStartResult = await client.SendAsync(jobStartReq);
            jobStartResult.EnsureSuccessStatusCode();
            var jobStartContent = await jobStartResult.Content.ReadAsStringAsync();
            var createdAutomationJob = JsonSerializer.Deserialize<AutomationJob>(jobStartContent);
            string jobName = createdAutomationJob.name;
            int count = 0;
            while (count < 10)
            {
                string jobCheckUrl = $"https://management.azure.com/subscriptions/{config["subscriptionID"]}/resourceGroups/{config["resourceGroupID"]}/providers/Microsoft.Automation/automationAccounts/{config["automationAccount"]}/jobs/{jobName}?api-version=2023-11-01";
                var checkReq = new HttpRequestMessage(HttpMethod.Get, jobStartUrl);
                jobStartReq.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var jobCheckResult = await client.SendAsync(checkReq);
                jobCheckResult.EnsureSuccessStatusCode();
                var ceckContent = await jobCheckResult.Content.ReadAsStringAsync();
                var newAutomationJob = JsonSerializer.Deserialize<AutomationJob>(ceckContent);
                if (newAutomationJob.properties.status == "Completed")
                {
                    Console.WriteLine($"Job Ended {newAutomationJob.properties.jobId}");
                    break;
                }
                if (newAutomationJob.properties.status == "Failed" ||
                    newAutomationJob.properties.status == "Stopped")
                {
                    Console.WriteLine($"Job Ended unsuccesfully {newAutomationJob.properties.jobId}");
                    break;
                }
                count++;
                Thread.Sleep(30000);
            }
            Console.ReadLine();
        }
    }
}