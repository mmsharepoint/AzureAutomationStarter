# Azure Automation Starter
This small demo repository shows how to Start an azure runbook programmatically. While the example scenario contacts and rates on provisioning a Microsoft Team the code can be re-used for any scenario. 
It's kept quite simple

## Summary

The repository consists of three parts: 
AzureAutomationStarter realizes The start of a runbook using the .Net SDK
AzureAutomationStarterRest realizes The start of a runbook using the rest api following the same scenario

For further details see the author's [blog post](https://mmsharepoint.wordpress.com/2024/)

## Version history

Version|Date|Author|Comments
-------|----|--------|--------
1.0|Aug 16, 2024|[Markus Moeller](http://www.twitter.com/moeller2_0)|Initial release


## Disclaimer

**THIS CODE IS PROVIDED _AS IS_ WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

## Minimal Path to Awesome

- Clone this repository
    ```bash
    git clone https://github.com/mmsharepoint/AzureAutomationStarter.git
    ```
- You will might want to register an apps in Entra ID 
  - Platform Desktop and Redirect http://localhost
  - with **delegated** Azure permission:
    - user_impersonation
- Copy AzureAutomationStarter\appsettings.sample.json to AzureAutomationStarter\appsettings.json and fill in your data
- Copy AzureAutomationStarterRest\appsettings.sample.json to AzureAutomationStarterRest\appsettings.json and fill in your data
- Fill in Team data to provovision inside your code
  - (This scenario assumes having a runbook structure for provisioning Microsoft teams but you can simply reuse it for any case or mock it for testing purposes. Simply the job should be started and take the right parameters.)
- In AzureAutomationStarter press F5 to debug and assign permissions
- In AzureAutomationStarterRest press F5 to debug and test permissions

## Features

This small demo illustrates the following concepts:

- [Microsoft Azure Automation management client library for .NET](https://learn.microsoft.com/en-us/dotnet/api/overview/azure/resourcemanager.automation-readme?view=azure-dotnet&WT.mc_id=M365-MVP-5004617)
- [Azure Resource Manager Rest APÌ](https://learn.microsoft.com/en-us/rest/api/automation/operation-groups?view=rest-automation-2023-11-01&WT.mc_id=M365-MVP-5004617)


    
