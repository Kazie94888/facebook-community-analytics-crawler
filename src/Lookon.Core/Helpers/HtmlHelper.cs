using System;
using System.Text;
using LookOn.Core.Extensions;

namespace LookOn.Core.Helpers
{
    public static class MimeTypeHelper
    {
        public static string GetMimeTypeForExtension(string extension)
        {
            switch (extension)
            {
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".ppt":
                    return "application/vnd.ms-powerpoint";
                case ".pptx":
                    return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case ".pdf":
                    return "application/pdf";
                case ".csv":
                    return "text/csv";
                default:
                    return null;
            }
        }
    }
    public static class HtmlHelper
    {
        public class Table : HtmlBase, IDisposable
        {

            public Table(StringBuilder sb, string border = "", string classAttributes = "", string id = "") : base(sb)
            {
                DefineCssTable();
                Append("<table");
                AddOptionalAttributes(classAttributes, id, border: border);
            }

            private void DefineCssTable()
            {
                Append("<style>");
                Append(".main-table {" +
                       "font-family: \"Trebuchet MS\", Arial, Helvetica, sans-serif;" +
                       "border-collapse: collapse;" +
                       "width: 100%;" +
                       "}");

                Append(".main-table td, .main-table th {" +
                       "border: 1px solid #ddd;" +
                       "padding: 8px;" +
                       "}");

                Append(".main-table tr:nth-child(even){background-color: #f2f2f2;}");

                Append(".main-table th {" +
                       "padding-top: 12px;" +
                       "padding-bottom: 12px;" +
                       "text-align: left;" +
                       "background-color: #4CAF50;" +
                       "color: white;" +
                       "}");
                Append("</style>");
            }

            public void StartHead(string classAttributes = "", string id = "")
            {
                Append("<thead");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndHead()
            {
                Append("</thead>");
            }

            public void StartFoot(string classAttributes = "", string id = "")
            {
                Append("<tfoot");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndFoot()
            {
                Append("</tfoot>");
            }

            public void StartBody(string classAttributes = "", string id = "")
            {
                Append("<tbody");
                AddOptionalAttributes(classAttributes, id);
            }

            public void EndBody()
            {
                Append("</tbody>");
            }

            public void Dispose()
            {
                Append("</table>");
            }

            public Row AddRow(string classAttributes = "", string id = "")
            {
                return new Row(GetBuilder(), classAttributes, id);
            }
            public RowTHead AddRowTHead(string classAttributes = "", string id = "")
            {
                return new RowTHead(GetBuilder(), classAttributes, id);
            }
        }

        public class RowTHead : HtmlBase, IDisposable
        {
            public RowTHead(StringBuilder sb, string classAttributes = "", string id = "") : base(sb)
            {
                Append("<tr");
                AddOptionalAttributes(classAttributes, id);
            }
            public void Dispose()
            {
                Append("</tr>");
            }
            public void AddCell(string innerText, string classAttributes = "", string id = "", string colSpan = "", string style = "")
            {
                Append("<th");
                AddOptionalAttributes(classAttributes, id, colSpan, style: style);
                Append(StringExtensions.FormatHtml(innerText));
                Append("</th>");
            }
        }


        public class Row : HtmlBase, IDisposable
        {
            public Row(StringBuilder sb, string classAttributes = "", string id = "") : base(sb)
            {
                Append("<tr");
                AddOptionalAttributes(classAttributes, id);
            }
            public void Dispose()
            {
                Append("</tr>");
            }
            public void AddCell(string innerText, string classAttributes = "", string id = "", string colSpan = "", string style = "")
            {
                Append("<td");
                AddOptionalAttributes(classAttributes, id, colSpan, style: style);
                Append(StringExtensions.FormatHtml(innerText));
                Append("</td>");
            }
        }

        public abstract class HtmlBase
        {
            private StringBuilder _sb;

            protected HtmlBase(StringBuilder sb)
            {
                _sb = sb;
            }

            public StringBuilder GetBuilder()
            {
                return _sb;
            }

            protected void Append(string toAppend)
            {
                _sb.Append(toAppend);
            }

            protected void AddOptionalAttributes(string className = "", string id = "", string colSpan = "", string border = "", string style = "")
            {

                if (id.IsNotNullOrEmpty())
                {
                    _sb.Append($" id=\"{id}\"");
                }
                if (className.IsNotNullOrEmpty())
                {
                    _sb.Append($" class=\"{className}\"");
                }
                if (colSpan.IsNotNullOrEmpty())
                {
                    _sb.Append($" colspan=\"{colSpan}\"");
                }
                if (border.IsNotNullOrEmpty())
                {
                    _sb.Append($" border=\"{border}\"");
                }

                if (style.IsNotNullOrEmpty())
                {
                    _sb.Append($" style=\"{style}\"");
                }
                _sb.Append(">");
            }
        }
    }
}