# HttpFunctionGenerator

## To Use
1. Create new .NET 6.0 EXE
2. Add ```<AzureFunctionsVersion>V4</AzureFunctionsVersion>``` to csproj file
3. Add ```HttpFunctionGenerator``` as dependency with ```OutputItemType``` set to ```Analyzer``` and ```ReferenceOutputAssembly``` set to ```false```
4. Add NuGet references to ```Microsoft.Azure.Functions.Worker```, ```Microsoft.Azure.Functions.Worker.Extensions.Http```, and ```Microsoft.Azure.Functions.Worker.Sdk```
5. Add ```host.json``` file containing at least ```{
  "version": "2.0"
}``` and ensure it is set to copy to output directory.
6. Add ```local.settings.json``` file containing at least ```{
  "IsEncrypted": false,
  "Values": {
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "AzureWebJobsStorage": "UseDevelopmentStorage=true"
  }
}``` and ensure it is set to copy to output directory.
7. In Program class, call ```ConfigureGeneratedHttpFunctions``` on the ```HostBuilder```.