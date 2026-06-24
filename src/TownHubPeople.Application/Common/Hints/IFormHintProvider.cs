namespace TownHubPeople.Application.Common.Hints;

/// <summary>
/// Single source of truth for the contextual hints shown on each sub-module's form. Keyed by
/// (module, feature); returns a generic fallback set so every form always has guidance even
/// before bespoke copy is authored.
/// </summary>
public interface IFormHintProvider
{
    FormHintSet Get(string module, string feature);
}
