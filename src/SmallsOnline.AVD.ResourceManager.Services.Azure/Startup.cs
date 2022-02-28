namespace SmallsOnline.AVD.ResourceManager.Services.Azure;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddHttpClient()
            .AddControllers();

        services.AddSingleton<IAzureApiService, AzureApiService>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app
            .UseDefaultFiles()
            .UseStaticFiles()
            .UseWebSockets()
            .UseRouting()
            .UseAuthorization()
            .UseEndpoints(
                configure: (endpoints) =>
                {
                    endpoints.MapControllers();
                }
            );
    }
}