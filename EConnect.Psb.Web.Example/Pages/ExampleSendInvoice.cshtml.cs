using System.ComponentModel.DataAnnotations;
using EConnect.Psb.Api;
using EConnect.Psb.Config;
using EConnect.Psb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace EConnect.Psb.Web.Example.Pages;

public class IndexModel : PageModel
{
    [Required]
    [BindProperty(SupportsGet = true)]
    public string? Username
    {
        get => _options.Value.Username;
        set => _options.Value.Username = value;
    }

    [Required]
    [BindProperty(SupportsGet = true)]
    public string? Password
    {
        get => _options.Value.Password;
        set => _options.Value.Password = value;
    }

    [Required]
    [BindProperty]
    public string? SenderPartyId { get; set; }

    [Required]
    [BindProperty]
    public string? ReceiverPartyId { get; set; }
        
    [BindProperty]
    public IFormFile? MyFile { get; set; }

    private readonly IOptions<PsbOptions> _options;
    private readonly IPsbMeApi _meApi;
    private readonly IPsbSalesInvoiceApi _salesInvoice;

    public IndexModel(IOptions<PsbOptions> options, IPsbMeApi meApi, IPsbSalesInvoiceApi salesInvoice)
    {
        _options = options;
        _meApi = meApi;
        _salesInvoice = salesInvoice;
    }

    public async Task OnGet()
    {
        try
        {
            if (!string.IsNullOrEmpty(Username))
                ViewData["meResponse"] = await _meApi.Me().ConfigureAwait(false);
        }
        catch (EConnectException ex)
        {
            ViewData["meResponse"] = ex.ToString();
        }
    }

    public async Task OnPost()
    {
        if (!ModelState.IsValid)
            return;

        try
        {
            var res = await _salesInvoice.QueryRecipientParty(
                SenderPartyId!,
                new[] { ReceiverPartyId! },
                cancellation: HttpContext.RequestAborted
            ).ConfigureAwait(false);

            ViewData["preflightResponse"] = res;
        }
        catch (EConnectException ex)
        {
            ViewData["preflightResponse"] = ex.ToString();
        }

        if(MyFile == null)
            return;

        try
        {
            var res = await _salesInvoice.Send(
                SenderPartyId!,
                new FileContent(MyFile.OpenReadStream(), MyFile.FileName),
                ReceiverPartyId,
                cancellation: HttpContext.RequestAborted
            ).ConfigureAwait(false);
            
            ViewData["sendResponse"] = res;
        }
        catch (EConnectException ex)
        {
            ViewData["sendResponse"] = ex.ToString();
        }
    }
}