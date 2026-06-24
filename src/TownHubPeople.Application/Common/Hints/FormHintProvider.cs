namespace TownHubPeople.Application.Common.Hints;

/// <summary>
/// In-code registry of form hints. Add a sub-module by adding one entry to <see cref="Build"/>;
/// anything not authored yet falls back to <see cref="Generic"/> so no form renders empty.
/// </summary>
public class FormHintProvider : IFormHintProvider
{
    private static readonly FormHintSet Generic = new(
        "Field Guide",
        [
            new FormHint("fa-asterisk", "Required fields", "Fields marked with validation must be filled before saving.", "danger"),
            new FormHint("fa-pen", "Editable later", "You can revise these details from the record's Edit screen.", "primary"),
            new FormHint("fa-history", "Audit-tracked", "Every change is recorded in the audit trail with who and when.", "secondary"),
        ]);

    private readonly Dictionary<string, FormHintSet> _sets = Build();

    public FormHintSet Get(string module, string feature)
        => _sets.GetValueOrDefault(Key(module, feature)) ?? Generic;

    private static string Key(string module, string feature) => $"{module}/{feature}".ToLowerInvariant();

    private static Dictionary<string, FormHintSet> Build()
    {
        var d = new Dictionary<string, FormHintSet>();

        d[Key("Admin", "User")] = new FormHintSet("Field Guide",
        [
            new FormHint("fa-id-card", "Name & Email", "The email is also the sign-in username and must be unique."),
            new FormHint("fa-user-shield", "Roles", "Roles drive what the user sees and can do. Manage per-role access under Menu Config.", "info"),
            new FormHint("fa-toggle-on", "Active", "Inactive users keep their record but are blocked from signing in.", "warning"),
        ]);

        d[Key("Admin", "Organization")] = new FormHintSet("Field Guide",
        [
            new FormHint("fa-building", "Organization profile", "Shown on reports and documents across the system."),
            new FormHint("fa-coins", "Currency", "Code and symbol used when formatting monetary values.", "info"),
        ]);

        return d;
    }
}
