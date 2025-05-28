using LangApp.Application.Auth.Options;

namespace LangApp.Api.Common.Services;

public interface IHtmlTemplateService
{
    string RenderEmailConfirmationSuccess();
    string RenderEmailConfirmationError();
    string RenderDeepLinkRedirect(string deepLink, ClientAppOptions options);
} 