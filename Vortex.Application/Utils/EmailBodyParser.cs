using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Vortex.Contracts;

namespace Vortex.Application.Utils;

public static class EmailBodyParser
{
    private static readonly Dictionary<EmailTemplate, string> TemplateMap = new()
    {
        { EmailTemplate.Invitation, "Vortex.Application.EmailTemplates.InvitationEmail.html" },
        { EmailTemplate.PasswordReset, "Vortex.Application.EmailTemplates.PasswordResetEmail.html" }
    };

    public static string ToHtmlBody(this object data, EmailTemplate template)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = TemplateMap[template];

        using (var stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream == null)
            {
                throw new FileNotFoundException($"Email template not found: {resourceName}");
            }
            using (var reader = new StreamReader(stream))
            {
                string body = reader.ReadToEnd();
                // Use reflection to replace placeholders
                foreach (var prop in data.GetType().GetProperties())
                {
                    body = body.Replace($"{{{{{prop.Name}}}}}", prop.GetValue(data)?.ToString());
                }
                return body;
            }
        }
    }
}
