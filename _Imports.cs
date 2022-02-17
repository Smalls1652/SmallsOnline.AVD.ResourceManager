global using System;
global using System.Collections.Generic;
global using System.Net;
global using System.Net.Http;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;

global using Microsoft.Azure.Functions.Worker;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;

global using SmallsOnline.AVD.ResourceManager.Helpers;
global using SmallsOnline.AVD.ResourceManager.Services.Azure;
global using SmallsOnline.AVD.ResourceManager.Services.CosmosDb;