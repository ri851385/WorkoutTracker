using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using WorkoutTracker;
using WorkoutTracker.Services;
using WorkoutTracker.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped<LocalStorageService>();
builder.Services.AddScoped<ExerciseCatalogService>();
builder.Services.AddScoped<WorkoutHistoryService>();
builder.Services.AddScoped<WorkoutSessionState>();
builder.Services.AddScoped<CsvExportService>();

await builder.Build().RunAsync();
