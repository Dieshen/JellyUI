using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Text.RegularExpressions;

namespace Jelly.UI
{
    public partial class JellyMarkdown
    {
        #region Fields and Constants
        private const string CODE_HIGHLIGHTING_LINE_SEPERATOR = " $$CHLS$$ ";
        private const string PATTERN_ORDERED_LIST = @"^\s*\d+\.\s";
        private const string PATTERN_UNORDERED_LIST = @"^\s*\-\s";
        private const string PATTERN_HORIZONTAL_RULES = @"^\-{3,}$";
        private const string PATTERN_BLOCKQUOTES = @"^>{1,}\s(.*)";
        private string html = string.Empty;
        #endregion

        #region Properties, Indexers

        /// <summary>
        /// Gets or sets the CSS class for blockquotes.
        /// </summary>
        [Parameter]
        public string? BlockQuotesCSSClass { get; set; } = "blockquote";

        /// <summary>
        /// Gets or sets the content to be rendered within the component.
        /// </summary>
        /// <remarks>
        /// Default value is <see langword="null" />.
        /// </remarks>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Gets or sets the CSS class for table.
        /// </summary>
        [Parameter]
        public string? TableCSSClass { get; set; } = "table";

        #endregion

        protected override void OnInitialized()
        {
            ParseMarkdown();

            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            if (IsRenderComplete)
                ParseMarkdown();

            base.OnParametersSet();
        }

        // Headers
        private string ConvertMakdownHeadersToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> parsedLines = [];

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    parsedLines.Add(line);
                    parsedLines.Add("\n");

                    continue;
                }

                if (Regex.IsMatch(line.Trim(), @"^#{6}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{6}\s?([^\n]+)", "<h6>$1</h6>"));
                }
                else if (Regex.IsMatch(line.Trim(), @"^#{5}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{5}\s?([^\n]+)", "<h5>$1</h5>"));
                }
                else if (Regex.IsMatch(line.Trim(), @"^#{4}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{4}\s?([^\n]+)", "<h4>$1</h4>"));
                }
                else if (Regex.IsMatch(line.Trim(), @"^#{3}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{3}\s?([^\n]+)", "<h3>$1</h3>"));
                }
                else if (Regex.IsMatch(line.Trim(), @"^#{2}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{2}\s?([^\n]+)", "<h2>$1</h2>"));
                }
                else if (Regex.IsMatch(line.Trim(), @"^#{1}\s?([^\n]+)"))
                {
                    parsedLines.Add(Regex.Replace(line.Trim(), @"^#{1}\s?([^\n]+)", "<h1>$1</h1>"));
                }
                else
                {
                    parsedLines.Add(line);
                    parsedLines.Add("\n");
                }
            }

            RemoveLastLineBreak(parsedLines);

            return string.Join("", parsedLines);
        }

        // BlockQuotes
        private string ConvertMarkdownBlockquotesToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> htmlLines = [];
            Stack<string> listStack = new();
            Stack<int> indentStack = new();

            foreach (string line in lines)
            {
                string trimmerLine = line.Trim();
                int indentLevel = trimmerLine.TakeWhile(c => c == '>').Count();

                if (Regex.IsMatch(trimmerLine, PATTERN_BLOCKQUOTES))
                {
                    if (listStack.Count == 0 || indentStack.Peek() < indentLevel)
                    {
                        if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
                        {
                            // close the `p` tag
                            if (listStack.Peek() == "p")
                                htmlLines.Add($"</{listStack.Pop()}>");

                            htmlLines.Add($"<blockquote class=\"{BlockQuotesCSSClass}\">");
                            listStack.Push("blockquote");
                            indentStack.Push(indentLevel);
                        }
                        else
                        {
                            while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                            {
                                htmlLines.Add($"</{listStack.Pop()}>");
                                indentStack.Pop();
                            }

                            if (listStack.Count == 0 || listStack.Peek() != "blockquote")
                            {
                                htmlLines.Add($"<blockquote class=\"{BlockQuotesCSSClass}\">");
                                listStack.Push("blockquote");
                                indentStack.Push(indentLevel);
                            }
                        }

                        htmlLines.Add($"<p>{Regex.Replace(trimmerLine, PATTERN_BLOCKQUOTES, "$1")}");
                        listStack.Push("p");
                    }
                    else if (indentStack.Peek() >= indentLevel)
                    {
                        htmlLines.Add($"<br />{Regex.Replace(trimmerLine, PATTERN_BLOCKQUOTES, "$1")}");
                    }
                }
                else
                {
                    // Close any open lists
                    while (listStack.Count > 0)
                    {
                        htmlLines.Add($"</{listStack.Pop()}>");

                        if (indentStack.Count > 0)
                            indentStack.Pop();
                    }

                    htmlLines.Add(line);
                    htmlLines.Add("\n");
                }
            }

            // Close any remaining open blockquotes
            while (listStack.Count > 0)
            {
                htmlLines.Add($"</{listStack.Pop()}>");

                if (indentStack.Count > 0)
                    indentStack.Pop();
            }

            return string.Join("", htmlLines);
        }

        // Code Highlighting
        private string ConvertMarkdownCodeHighlightingToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> parsedLines = [];
            bool isCodeBlockInprogress = false;

            for (int i = 0; i < lines.Count(); i++)
                if (Regex.IsMatch(lines[i].Trim(), @"\```(\w+)"))
                {
                    if (!isCodeBlockInprogress)
                        isCodeBlockInprogress = true;

                    lines[i] = Regex.Replace(lines[i].Trim(), @"\```(\w+)", "<pre><code class=\"lang-$1\">");
                    parsedLines.Add(lines[i]);
                }
                else if (Regex.IsMatch(lines[i].Trim().Trim(), @"```"))
                {
                    if (isCodeBlockInprogress)
                        isCodeBlockInprogress = false;

                    lines[i] = Regex.Replace(lines[i].Trim(), @"```", "</code></pre>");
                    parsedLines.Add(lines[i]);
                }
                else if (isCodeBlockInprogress)
                {
                    parsedLines.Add(lines[i]);
                    parsedLines.Add($"{CODE_HIGHLIGHTING_LINE_SEPERATOR}");
                }
                else
                {
                    parsedLines.Add(lines[i]);
                    parsedLines.Add("\n");
                }

            RemoveLastLineBreak(parsedLines);

            return string.Join("", parsedLines);
        }

        // Emphasis
        private string ConvertMarkdownEmphasisToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> parsedLines = [];

            for (int i = 0; i < lines.Count(); i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    parsedLines.Add(lines[i]);

                    continue;
                }

                if (Regex.IsMatch(lines[i].Trim(), @"\*\*(.*?)\*\*")
                    || Regex.IsMatch(lines[i].Trim().Trim(), @"__(.*?)__")
                    || Regex.IsMatch(lines[i].Trim(), @"\*(.*?)\*")
                    || Regex.IsMatch(lines[i].Trim(), @"_(.*?)_")
                    || Regex.IsMatch(lines[i].Trim(), @"~~(.*?)~~"))
                {
                    lines[i] = Regex.Replace(lines[i].Trim(), @"\*\*(.*?)\*\*", "<b>$1</b>");
                    lines[i] = Regex.Replace(lines[i].Trim(), @"__(.*?)__", "<b>$1</b>");
                    lines[i] = Regex.Replace(lines[i].Trim(), @"\*(.*?)\*", "<i>$1</i>");
                    lines[i] = Regex.Replace(lines[i].Trim(), @"_(.*?)_", "<i>$1</i>");
                    lines[i] = Regex.Replace(lines[i].Trim(), @"~~(.*?)~~", "<s>$1</s>");
                }

                parsedLines.Add(lines[i]);
            }

            return string.Join("\n", parsedLines);
        }

        // HorizontalRules
        private string ConvertMarkdownHorizontalRulesToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> parsedLines = [];

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    parsedLines.Add(line);
                    parsedLines.Add("\n");

                    continue;
                }

                if (Regex.IsMatch(line.Trim(), PATTERN_HORIZONTAL_RULES))
                {
                    RemoveLastLineBreak(parsedLines);
                    parsedLines.Add(Regex.Replace(line.Trim(), PATTERN_HORIZONTAL_RULES, "<hr />"));
                }
                else
                {
                    parsedLines.Add(line);
                    parsedLines.Add("\n");
                }
            }

            RemoveLastLineBreak(parsedLines);

            return string.Join("", parsedLines);
        }

        // Image
        private string ConvertMarkdownImageToHtml(string markup)
        {
            // Pattern to match Markdown image syntax: ![alt text](url "optional title" =WIDTHxHEIGHT)
            string pattern = @"!\[(.*?)\]\((.*?)(\s*""[^""]*"")?(\s*=\s*(\d*)x?(\d*))?\)";

            // Replace Markdown image syntax with HTML <img> tag
            string html = Regex.Replace(
                markup,
                pattern,
                match =>
                {
                    string altText = match.Groups[1].Value;
                    string url = match.Groups[2].Value;
                    string title = match.Groups[3].Value;
                    string width = match.Groups[5].Value;
                    string height = match.Groups[6].Value;

                    string imgTag = $"<img src=\"{url}\" alt=\"{altText}\"";

                    if (!string.IsNullOrEmpty(title)) imgTag += $" title={title}";

                    if (!string.IsNullOrEmpty(width)) imgTag += $" width=\"{width}\"";

                    if (!string.IsNullOrEmpty(height)) imgTag += $" height=\"{height}\"";

                    imgTag += " />";

                    return imgTag;
                }
            );

            return html;
        }

        // In-line code
        private string ConvertMarkdownInlineCodeToHtml(string markup)
        {
            // Pattern to match inline Markdown code syntax: `code`
            string pattern = @"`([^`]+)`";

            // Replace inline Markdown code syntax with HTML <code> tag
            string html = Regex.Replace(
                markup,
                pattern,
                match =>
                {
                    string codeText = match.Groups[1].Value;

                    return $"<code>{codeText}</code>";
                }
            );

            return html;
        }

        // Line breaks
        private string ConvertMarkdownLineBreaksToHtml(string markup) => markup.Replace("\n", "<br />");

        // Links
        private string ConvertMarkdownLinksToHtml(string markup)
        {
            // Pattern to match Markdown link syntax: [Link Text](Link URL)
            string pattern = @"\[(.*?)\]\((.*?)\)";

            // Replace Markdown link syntax with HTML <a> tag
            string html = Regex.Replace(
                markup,
                pattern,
                match =>
                {
                    string linkText = match.Groups[1].Value;
                    string linkUrl = match.Groups[2].Value;

                    return $"<a href=\"{linkUrl}\">{linkText}</a>";
                }
            );

            return html;
        }

        // Lists
        private string ConvertMarkdownListToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> htmlLines = [];
            Stack<string> listStack = new();
            Stack<int> indentStack = new();

            foreach (string line in lines)
            {
                int indentLevel = line.TakeWhile(char.IsWhiteSpace).Count();

                if (Regex.IsMatch(line, PATTERN_ORDERED_LIST))
                {
                    // Ordered list
                    if (listStack.Count == 0 || listStack.Peek() != "ol" || indentStack.Peek() < indentLevel)
                    {
                        if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
                        {
                            htmlLines.Add("<ol>");
                            listStack.Push("ol");
                            indentStack.Push(indentLevel);
                        }
                        else
                        {
                            while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                            {
                                htmlLines.Add($"</{listStack.Pop()}>");
                                indentStack.Pop();
                            }

                            if (listStack.Count == 0 || listStack.Peek() != "ol")
                            {
                                htmlLines.Add("<ol>");
                                listStack.Push("ol");
                                indentStack.Push(indentLevel);
                            }
                        }

                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_ORDERED_LIST, "")}");
                    }
                    else if (indentStack.Peek() > indentLevel)
                    {
                        htmlLines.Add($"</{listStack.Pop()}>");
                        indentStack.Pop();

                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_ORDERED_LIST, "")}");
                    }
                    else if (indentStack.Peek() == indentLevel)
                    {
                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_ORDERED_LIST, "")}");
                    }
                }
                else if (Regex.IsMatch(line, PATTERN_UNORDERED_LIST))
                {
                    // Unordered list
                    if (listStack.Count == 0 || listStack.Peek() != "ul" || indentStack.Peek() < indentLevel)
                    {
                        if (listStack.Count > 0 && indentStack.Peek() < indentLevel)
                        {
                            htmlLines.Add("<ul>");
                            listStack.Push("ul");
                            indentStack.Push(indentLevel);
                        }
                        else
                        {
                            while (listStack.Count > 0 && indentStack.Peek() > indentLevel)
                            {
                                htmlLines.Add($"</{listStack.Pop()}>");
                                indentStack.Pop();
                            }

                            if (listStack.Count == 0 || listStack.Peek() != "ul")
                            {
                                htmlLines.Add("<ul>");
                                listStack.Push("ul");
                                indentStack.Push(indentLevel);
                            }
                        }

                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_UNORDERED_LIST, "")}");
                    }
                    else if (indentStack.Peek() > indentLevel)
                    {
                        htmlLines.Add($"</{listStack.Pop()}>");
                        indentStack.Pop();

                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_UNORDERED_LIST, "")}");
                    }
                    else if (indentStack.Peek() == indentLevel)
                    {
                        htmlLines.Add($"<li>{Regex.Replace(line, PATTERN_UNORDERED_LIST, "")}");
                    }
                }
                else
                {
                    // Close any open lists
                    while (listStack.Count > 0)
                    {
                        htmlLines.Add($"</{listStack.Pop()}>");
                        indentStack.Pop();
                    }

                    htmlLines.Add(line);
                    htmlLines.Add("\n");
                }
            }

            // Close any remaining open lists
            while (listStack.Count > 0)
            {
                htmlLines.Add($"</{listStack.Pop()}>");
                indentStack.Pop();
            }

            // Close any open list items
            for (int i = 0; i < htmlLines.Count; i++)
                if (htmlLines[i].StartsWith("<li>") && (i == htmlLines.Count - 1 || htmlLines[i + 1].StartsWith("<li>") || htmlLines[i + 1].StartsWith("</")))
                    htmlLines[i] += "</li>";

            RemoveLastLineBreak(htmlLines);

            return string.Join("", htmlLines);
        }

        // Paragraphs
        private string ConvertMarkdownParagraphsToHtml(string markup)
        {
            string[] lines = markup.Split("\n\n\n");
            List<string> parsedLines = [];

            if (lines.Length == 1)
                return markup;

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    parsedLines.Add(line);

                    continue;
                }

                parsedLines.Add($"<p>{line}</p>");
            }

            return string.Join("", parsedLines);
        }

        // Tables
        private string ConvertMarkdownTableToHtml(string markup)
        {
            string[] lines = markup.Split("\n");
            List<string> parsedLines = [];
            List<string> htmlLines = [];

            bool isTableStart = false;
            bool isTableHeadingAdded = false;

            // Read lines starting with '|'
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    parsedLines.Add(line);

                    continue;
                }

                // Trim row with spaces
                string trimmedLine = line.Trim();

                if (trimmedLine.StartsWith("| "))
                {
                    if (!isTableStart)
                        isTableStart = true;

                    // Remove '|' from the start and end of the line
                    trimmedLine = trimmedLine.TrimStart('|').TrimEnd('|');

                    // Convert markdown syntax to HTML
                    string[] cells = trimmedLine.Split("|", StringSplitOptions.RemoveEmptyEntries);
                    string tableRow = "<tr>";

                    foreach (string cell in cells)
                    {
                        string tableCellTagName = !isTableHeadingAdded ? "th" : "td";
                        string tableCell = $"<{tableCellTagName}>{cell.Trim()}</{tableCellTagName}>";
                        tableRow += tableCell;
                    }

                    tableRow += "</tr>";
                    htmlLines.Add(tableRow);
                }
                else if (trimmedLine.StartsWith("|--") || trimmedLine.StartsWith("|:--"))
                {
                    // Table heading row is over
                    if (!isTableHeadingAdded)
                    {
                        isTableHeadingAdded = true;
                        htmlLines.Add("</thead>");
                        htmlLines.Add("<tbody>");
                    }
                }
                else
                {
                    if (isTableStart)
                    {
                        isTableStart = false;
                        parsedLines.Add($"<table class=\"{TableCSSClass}\"><thead>{string.Join("", htmlLines)}</tbody></table>");
                        htmlLines.Clear();
                    }
                    else
                    {
                        parsedLines.Add(line);
                    }
                }
            }

            if (isTableStart && htmlLines.Any())
            {
                isTableStart = false;
                parsedLines.Add($"<table class=\"{TableCSSClass}\"><thead>{string.Join("", htmlLines)}</tbody></table>");
                htmlLines.Clear();
            }

            return string.Join("\n", parsedLines);
        }

        private List<string> GetLines()
        {
            List<string> inputs = [];

            if (ChildContent is not null)
            {
                RenderTreeBuilder builder = new();
                ChildContent.Invoke(builder);

#pragma warning disable BL0006 // Do not use RenderTree types
                Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrame[] frames = builder.GetFrames().Array;

                foreach (Microsoft.AspNetCore.Components.RenderTree.RenderTreeFrame frame in frames)
                    if (frame.MarkupContent is not null)
                    {
                        List<string> lines = frame.MarkupContent.Split("\r\n").ToList();

                        if (lines.Any())
                            inputs.AddRange(lines);
                    }
#pragma warning restore BL0006 // Do not use RenderTree types
            }

            if (inputs.Any())
            {
                // remove first blank line
                if (string.IsNullOrWhiteSpace(inputs[0]))
                    inputs.RemoveAt(0);

                // remove last blank line
                if (inputs.Count > 0 && string.IsNullOrWhiteSpace(inputs[^1]))
                    inputs.RemoveAt(inputs.Count - 1);
            }

            return inputs;
        }

        private void ParseMarkdown()
        {
            List<string>? lines = GetLines();

            if (lines is null)
                return;

            // NOTE: do not change the sequence of these two lines
            string markup = string.Join("\n", lines);
            markup = ConvertMakdownHeadersToHtml(markup);
            markup = ConvertMarkdownBlockquotesToHtml(markup);
            markup = ConvertMarkdownHorizontalRulesToHtml(markup);
            markup = ConvertMarkdownEmphasisToHtml(markup);
            markup = ConvertMarkdownCodeHighlightingToHtml(markup);
            markup = ConvertMarkdownListToHtml(markup);
            markup = ConvertMarkdownTableToHtml(markup);
            markup = ConvertMarkdownParagraphsToHtml(markup);
            markup = ConvertMarkdownLineBreaksToHtml(markup);
            markup = ConvertMarkdownImageToHtml(markup);
            markup = ConvertMarkdownLinksToHtml(markup);
            markup = ConvertMarkdownInlineCodeToHtml(markup);
            html = markup.Replace(CODE_HIGHLIGHTING_LINE_SEPERATOR, "\n");
        }

        private static void RemoveLastLineBreak(List<string> htmlLines)
        {
            // remove last line break
            if (htmlLines.Any() && htmlLines[^1] == "\n")
                htmlLines.RemoveAt(htmlLines.Count - 1);
        }
    }
}