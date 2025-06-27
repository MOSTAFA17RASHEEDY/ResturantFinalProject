using Microsoft.AspNetCore.Razor.TagHelpers;

namespace ResturantFinalProject.TagHelpers
{
    [HtmlTargetElement("td", Attributes = "availability")]
    public class AvailabilityTagHelper : TagHelper
    {
        public bool Availability { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "td";

            // Set the text color based on the Availability property
            string color = Availability ? "green" : "red";
            output.Attributes.SetAttribute("style", $"color: {color};");

            // Set the content to "Yes" or "No" for better readability
            output.Content.SetContent(Availability.ToString());
        }
    }
}
