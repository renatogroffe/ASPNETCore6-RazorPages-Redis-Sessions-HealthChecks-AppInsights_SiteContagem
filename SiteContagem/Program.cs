using StackExchange.Redis;
using SiteContagem.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddHealthChecks();

builder.Services.AddSingleton<ConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(
        builder.Configuration.GetConnectionString("Redis"))
);

builder.Services.AddApplicationInsightsTelemetry(
    builder.Configuration);

if (builder.Configuration["Session:Type"]?.ToUpper() == "REDIS")
    builder.Services.AddStackExchangeRedisCache(redisCacheConfig =>
        {
            redisCacheConfig.Configuration =
                builder.Configuration.GetConnectionString("Redis");
            redisCacheConfig.InstanceName = "SiteContagem-";
        });
else
    builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(
        Convert.ToInt32(builder.Configuration["Session:TimeoutInSeconds"]));
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHealthChecks("/status", HealthChecksExtensions.GetJsonReturn());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();