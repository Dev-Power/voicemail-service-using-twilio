using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Twilio.AspNet.Core;
using Twilio.TwiML;

namespace VoicemailDirectory.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class IncomingCallController : TwilioController
{
    private readonly ILogger<IncomingCallController> _logger;
    private readonly VoicemailOptions _voicemailOptions;

    public IncomingCallController(
        ILogger<IncomingCallController> logger,
        IOptionsSnapshot<VoicemailOptions> voicemailOptions
    )
    {
        _logger = logger;
        _voicemailOptions = voicemailOptions.Value;
    }

    [HttpPost]
    public TwiMLResult Index([FromForm] string from)
    {
        _logger.LogInformation("Incoming call from {callingNumber}", from);

        var response = new VoiceResponse();

        var redirectUrl = _voicemailOptions.Owners.Contains(from)
            ? Url.Action("Index", "Directory")!
            : Url.Action("Index", "Record")!;

        response.Redirect(
            url: new Uri(redirectUrl, UriKind.Relative),
            method: Twilio.Http.HttpMethod.Post
        );

        return TwiML(response);
    }
}