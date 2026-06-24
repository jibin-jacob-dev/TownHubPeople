using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TownHubPeople.Web.TagHelpers;

/// <summary>
/// Renders the standard page header + breadcrumb, e.g.
/// <c>&lt;erp-page-header module="Administration" item="Users"&gt;...action button...&lt;/erp-page-header&gt;</c>.
/// All segments but the last render muted (the last is the current page). Child content (an action
/// button such as "Add New" / "Back to List") is placed to the right.
/// </summary>
[HtmlTargetElement("erp-page-header")]
public class PageHeaderTagHelper : TagHelper
{
    public string Module { get; set; } = string.Empty;
    public string? Item { get; set; }
    public string? Action { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var child = await output.GetChildContentAsync();

        output.TagName = "div";
        // Force a start+end tag so the self-closing usage `<erp-page-header ... />` renders as
        // `<div>…</div>` rather than an unclosed div that swallows following content.
        output.TagMode = TagMode.StartTagAndEndTag;
        output.Attributes.SetAttribute("class", "page-header");

        var segments = new List<string>();
        if (!string.IsNullOrWhiteSpace(Module)) segments.Add(Module);
        if (!string.IsNullOrWhiteSpace(Item)) segments.Add(Item!);
        if (!string.IsNullOrWhiteSpace(Action)) segments.Add(Action!);

        var enc = HtmlEncoder.Default;
        var h3 = new StringBuilder("<h3>");
        for (var i = 0; i < segments.Count; i++)
        {
            if (i > 0) h3.Append("<span class=\"text-muted mx-1\">›</span> ");
            if (i == segments.Count - 1)
                h3.Append(enc.Encode(segments[i]));
            else
                h3.Append("<span class=\"text-muted fw-normal\">").Append(enc.Encode(segments[i])).Append("</span> ");
        }
        h3.Append("</h3>");

        output.Content.AppendHtml(h3.ToString());
        if (!child.IsEmptyOrWhiteSpace)
            output.Content.AppendHtml(child);
    }
}
