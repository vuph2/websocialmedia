using Microsoft.AspNetCore.Html;
using System.Net;
using System.Text.RegularExpressions;

namespace web.Helpers
{
    public static class HtmlFormatter
    {
        private static readonly Regex UrlRegex = new Regex(
            @"(http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&amp;=]*)?)",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static IHtmlContent FormatPostContent(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new HtmlString(string.Empty);

            // Safely HTML encode everything to prevent XSS
            var encodedContent = WebUtility.HtmlEncode(content);

            var embeds = new List<string>();

            // Replace raw URLs with clickable anchor tags and extract embeds
            var htmlWithLinks = UrlRegex.Replace(encodedContent, match =>
            {
                var url = match.Groups[0].Value;
                var rawUrl = WebUtility.HtmlDecode(url); 
                var lowerUrl = rawUrl.ToLower();

                // 1. Image Embed Check
                if (lowerUrl.EndsWith(".jpg") || lowerUrl.EndsWith(".jpeg") || 
                    lowerUrl.EndsWith(".png") || lowerUrl.EndsWith(".gif") || lowerUrl.EndsWith(".webp"))
                {
                    embeds.Add($"<div style=\"margin-top:0.8rem;\"><img src=\"{rawUrl}\" style=\"max-width:100%; max-height:400px; display:block; border-radius:12px;\" alt=\"Embedded media\" /></div>");
                }
                // 2. Raw Video File Check (.mp4, .webm, .mov)
                else if (lowerUrl.EndsWith(".mp4") || lowerUrl.EndsWith(".webm") || lowerUrl.EndsWith(".mov"))
                {
                    embeds.Add($"<div style=\"margin-top:0.8rem;\"><video src=\"{rawUrl}\" controls style=\"max-width:100%; max-height:400px; display:block; border-radius:12px;\"></video></div>");
                }
                // 2.5. Raw Audio File Check (.mp3, .wav, .ogg)
                else if (lowerUrl.EndsWith(".mp3") || lowerUrl.EndsWith(".wav") || lowerUrl.EndsWith(".ogg"))
                {
                    embeds.Add($"<div style=\"margin-top:0.8rem;\"><audio src=\"{rawUrl}\" controls style=\"width:100%; max-width:400px; display:block;\"></audio></div>");
                }
                // 3. YouTube Embed Check
                else if (Regex.IsMatch(rawUrl, @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})", RegexOptions.IgnoreCase))
                {
                    var ytMatch = Regex.Match(rawUrl, @"(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^""&?\/\s]{11})", RegexOptions.IgnoreCase);
                    if (ytMatch.Success && ytMatch.Groups.Count > 1)
                    {
                        var videoId = ytMatch.Groups[1].Value;
                        embeds.Add($"<div style=\"margin-top:0.8rem; border-radius:12px; overflow:hidden; position:relative; padding-bottom:56.25%; height:0;\"><iframe src=\"https://www.youtube.com/embed/{videoId}\" style=\"position:absolute; top:0; left:0; width:100%; height:100%; border:none; border-radius:12px;\" allowfullscreen></iframe></div>");
                    }
                }
                // 4. Medal.tv Embed Check
                else if (lowerUrl.Contains("medal.tv") && lowerUrl.Contains("/clips/"))
                {
                    var medalMatch = Regex.Match(rawUrl, @"/clips/([^/?\s]+)", RegexOptions.IgnoreCase);
                    if (medalMatch.Success && medalMatch.Groups.Count > 1)
                    {
                        var clipId = medalMatch.Groups[1].Value;
                        embeds.Add($"<div style=\"margin-top:0.8rem; border-radius:12px; overflow:hidden; position:relative; padding-bottom:56.25%; height:0;\"><iframe src=\"https://medal.tv/clip/{clipId}?autoplay=0&muted=0&loop=0\" style=\"position:absolute; top:0; left:0; width:100%; height:100%; border:none; border-radius:12px;\" allowfullscreen></iframe></div>");
                    }
                }
                // 5. Generic Link Card (Nexus Premium Style)
                else
                {
                    var domain = "External Link";
                    try { domain = new Uri(rawUrl).Host; } catch { }
                    
                    embeds.Add($@"<a href=""{rawUrl}"" target=""_blank"" rel=""noopener noreferrer"" class=""link-card-anchor"">
                        <div class=""link-card-container"">
                            <div class=""link-card-icon"">
                                <i class=""bi bi-link-45deg""></i>
                            </div>
                            <div class=""link-card-info"">
                                <div class=""link-card-domain"">{domain}</div>
                                <div class=""link-card-url"">{url}</div>
                            </div>
                            <div class=""link-card-arrow"">
                                <i class=""bi bi-box-arrow-up-right""></i>
                            </div>
                        </div>
                    </a>");
                }

                // Return the functional link inline
                return $"<a href=\"{rawUrl}\" target=\"_blank\" rel=\"noopener noreferrer\" class=\"inline-link\">{url}</a>";
            });

            // Preserve whitespace and newlines
            htmlWithLinks = htmlWithLinks.Replace("\n", "<br/>");

            // Append embeds at the bottom of the post/comment
            if (embeds.Any())
            {
                // De-duplicate generic link embeds if multiple identical URLs in same post
                htmlWithLinks += string.Join("", embeds.Distinct());
            }

            return new HtmlString(htmlWithLinks);
        }
    }
}
