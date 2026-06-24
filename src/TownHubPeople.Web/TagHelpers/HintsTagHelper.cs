using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;
using TownHubPeople.Application.Common.Hints;

namespace TownHubPeople.Web.TagHelpers;

/// <summary>
/// Renders the contextual hint panel for a sub-module's form from <see cref="IFormHintProvider"/>:
/// <c>&lt;erp-hints module="Admin" feature="User" /&gt;</c>. Emits the standard <c>info-panel</c> card.
/// </summary>
[HtmlTargetElement("erp-hints", Attributes = "feature")]
public class HintsTagHelper : TagHelper
{
    private readonly IFormHintProvider _hints;

    public HintsTagHelper(IFormHintProvider hints) => _hints = hints;

    public string Module { get; set; } = string.Empty;
    public string Feature { get; set; } = string.Empty;

    /// <summary>Optional document status ordinal to show status-specific guidance.</summary>
    public int? Status { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var set = _hints.Get(Module, Feature);
        var enc = HtmlEncoder.Default;
        var sb = new StringBuilder();

        sb.Append("<div class=\"card-body\">");
        sb.Append("<h6 class=\"info-panel-heading\"><i class=\"fas fa-lightbulb me-2\"></i>").Append(enc.Encode(set.Heading)).Append("</h6>");

        var statusHint = Status.HasValue ? set.StatusGuide?.FirstOrDefault(s => s.Status == Status.Value) : null;
        if (statusHint is not null)
        {
            sb.Append("<div class=\"info-panel-status status-").Append(statusHint.CssClass).Append("\">")
              .Append("<i class=\"fas ").Append(statusHint.Icon).Append("\"></i><span>").Append(enc.Encode(statusHint.Message)).Append("</span></div>");
            foreach (var a in statusHint.Actions) AppendItem(sb, enc, a);
            sb.Append("<div class=\"info-panel-divider\"></div>");
        }

        foreach (var f in set.Fields) AppendItem(sb, enc, f);

        sb.Append("</div>");

        output.TagName = "div";
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "card info-panel");
        output.Content.AppendHtml(sb.ToString());
    }

    private static void AppendItem(StringBuilder sb, HtmlEncoder enc, FormHint h)
    {
        sb.Append("<div class=\"info-panel-item\"><i class=\"fas ").Append(h.Icon).Append(" text-").Append(h.Tone).Append("\"></i>")
          .Append("<div><strong>").Append(enc.Encode(h.Title)).Append("</strong><p>").Append(enc.Encode(h.Body)).Append("</p></div></div>");
    }
}
