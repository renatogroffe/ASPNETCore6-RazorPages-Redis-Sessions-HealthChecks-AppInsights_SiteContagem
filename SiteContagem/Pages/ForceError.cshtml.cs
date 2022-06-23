using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SiteContagem.Pages;

public class ForceErrorModel : PageModel
{
    public void OnGet([FromServices] ILogger<IndexModel> logger)
    {
        ApplicationStatus.Healthy = false;
        logger.LogWarning("Status da aplicação configurado para simulação de erro do tipo 500!");
    }
}