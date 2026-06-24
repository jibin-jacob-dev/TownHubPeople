namespace TownHubPeople.Application.Common.Hints;

/// <summary>A single guidance row in a form's hint panel (icon + title + body, tinted by <paramref name="Tone"/>).</summary>
/// <param name="Icon">Font Awesome class without the "fas" prefix, e.g. "fa-barcode".</param>
/// <param name="Tone">Bootstrap text tone: primary | info | success | danger | warning | secondary.</param>
public record FormHint(string Icon, string Title, string Body, string Tone = "primary");

/// <summary>Status-specific guidance for a document form (shown when the form passes its current status).</summary>
/// <param name="Status">Ordinal of a document status (0=Draft … 5=Closed).</param>
/// <param name="CssClass">info-panel status modifier: draft | pending | approved | rejected | cancelled.</param>
/// <param name="Icon">Font Awesome class without the "fas" prefix.</param>
public record DocStatusHint(int Status, string CssClass, string Icon, string Message, IReadOnlyList<FormHint> Actions);

/// <summary>The full hint set for a sub-module's form: a heading, field-level hints, and (optional) document status guidance.</summary>
public record FormHintSet(string Heading, IReadOnlyList<FormHint> Fields, IReadOnlyList<DocStatusHint>? StatusGuide = null);
