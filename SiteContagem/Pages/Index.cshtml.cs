using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StackExchange.Redis;
using SiteContagem.Logging;

namespace SiteContagem.Pages;

public class IndexModel : PageModel
{
    public void OnGet([FromServices] ILogger<IndexModel> logger,
        [FromServices] IConfiguration configuration,
        [FromServices] ConnectionMultiplexer connectionRedis,
        [FromServices] TelemetryConfiguration telemetryConfig)
    {
        DateTimeOffset inicio = DateTime.Now;
        var watch = new Stopwatch();
        watch.Start();
        var valorAtual =
            (int)connectionRedis.GetDatabase().StringIncrement("APIContagem");;
        logger.LogValorAtual(valorAtual);
        watch.Stop();
        TelemetryClient client = new (telemetryConfig);
        client.TrackDependency(
            "Redis", "Get", $"valor = {valorAtual}", inicio, watch.Elapsed, true);

        if (String.IsNullOrWhiteSpace(HttpContext.Session.GetString("UserSession")))
            HttpContext.Session.SetString("UserSession", Guid.NewGuid().ToString());
        
        TempData["Contador"] = valorAtual;
        TempData["UserSession"] = HttpContext.Session.GetString("UserSession");
        TempData["SessionType"] = configuration["Session:Type"];
        TempData["Local"] = ApplicationStatus.Local;
        TempData["Kernel"] = ApplicationStatus.Kernel;
        TempData["Framework"] = ApplicationStatus.Framework;
        TempData["Saudavel"] = ApplicationStatus.Healthy ? "Sim" : "Não";
        TempData["MensagemFixa"] = "Teste";
        TempData["MensagemVariavel"] = configuration["MensagemVariavel"];
    }
}