using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Vortex.Notification.Service.Utils;

public static class EmailBodyParser
{
    public static string Parse(string templateName, Dictionary<string, string> data)
    {
        var templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates", $"{templateName}.html");

        if (!File.Exists(templatePath))
        {
            throw new FileNotFoundException($"Email template not found at {templatePath}");
        }

        var body = File.ReadAllText(templatePath);

        foreach (var entry in data)
        {
            body = body.Replace($"{{{{{entry.Key}}}}}", WebUtility.HtmlEncode(entry.Value));
        }

        return body;
    }
}
