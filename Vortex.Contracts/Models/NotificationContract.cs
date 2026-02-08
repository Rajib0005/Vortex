using System;
using System.Collections.Generic;

namespace Vortex.Contracts.Models;

public record NotificationContract(
    Guid NotificationId,
    string Destination, // The recipient's email address
    string TemplateId, // e.g., "user-invitation", "password-reset"
    Dictionary<string, string> TemplateData, // For personalizing the template
    DateTime Timestamp
);
