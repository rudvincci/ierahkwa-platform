namespace Mamey.Portal.Cms.Application.Services;

public interface ICmsHtmlSanitizer
{
    string SanitizeNewsBody(string html);
    string SanitizePageBody(string html);
}


