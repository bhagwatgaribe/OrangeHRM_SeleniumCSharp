using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public static class ExtentReportManager
    {
        private static readonly ConcurrentDictionary<string, TestResult> _results = new ConcurrentDictionary<string, TestResult>();
        private static readonly ThreadLocal<string> _currentTest = new ThreadLocal<string>();

        public static string ReportPath { get; }

        static ExtentReportManager()
        {
            try
            {
                var reportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
                Directory.CreateDirectory(reportDir);
                ReportPath = Path.Combine(reportDir, $"TestReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");
            }
            catch
            {
                // ignore report init errors
            }
        }

        public static void CreateTest(string testName)
        {
            try
            {
                _currentTest.Value = testName;
                _results.TryAdd(testName, new TestResult { Name = testName, Status = "Started", Messages = new List<string>() });
            }
            catch { }
        }

        public static void LogPass(string message)
        {
            try
            {
                var name = _currentTest.Value;
                if (string.IsNullOrEmpty(name)) return;
                if (_results.TryGetValue(name, out var r))
                {
                    r.Status = "Passed";
                    r.Messages.Add(message);
                }
            }
            catch { }
        }

        public static void LogFail(string message)
        {
            try
            {
                var name = _currentTest.Value;
                if (string.IsNullOrEmpty(name)) return;
                if (_results.TryGetValue(name, out var r))
                {
                    r.Status = "Failed";
                    r.Messages.Add(message);
                }
            }
            catch { }
        }

        public static void AddScreenCapture(string path)
        {
            try
            {
                var name = _currentTest.Value;
                if (string.IsNullOrEmpty(name)) return;
                if (_results.TryGetValue(name, out var r))
                {
                    r.ScreenshotPath = path;
                }
            }
            catch { }
        }

        public static void Flush()
        {
            try
            {
                // Build simple HTML report
                var sb = new StringBuilder();
                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine("<meta charset=\"utf-8\" />");
                sb.AppendLine("<title>Automation Test Report</title>");
                sb.AppendLine("<style> body{font-family: Arial, Helvetica, sans-serif;} .passed{color:green;} .failed{color:red;} .card{border:1px solid #ddd;padding:10px;margin:10px 0;} img{max-width:800px;display:block;margin-top:10px;} </style>");
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");
                sb.AppendLine($"<h1>Test Execution Report - {DateTime.Now:yyyy-MM-dd HH:mm:ss}</h1>");

                foreach (var kv in _results)
                {
                    var r = kv.Value;
                    var cssClass = string.Equals(r.Status, "Passed", StringComparison.OrdinalIgnoreCase) ? "passed" : (string.Equals(r.Status, "Failed", StringComparison.OrdinalIgnoreCase) ? "failed" : "");
                    sb.AppendLine($"<div class=\"card\"><h2 class=\"{cssClass}\">{r.Name} - {r.Status}</h2>");
                    if (r.Messages != null)
                    {
                        sb.AppendLine("<ul>");
                        foreach (var m in r.Messages)
                        {
                            sb.AppendLine($"<li>{System.Net.WebUtility.HtmlEncode(m)}</li>");
                        }
                        sb.AppendLine("</ul>");
                    }
                    if (!string.IsNullOrEmpty(r.ScreenshotPath) && File.Exists(r.ScreenshotPath))
                    {
                        // Convert screenshot path to relative path from report
                        var rel = MakeRelativePath(Path.GetDirectoryName(ReportPath) ?? string.Empty, r.ScreenshotPath);
                        sb.AppendLine($"<div>Screenshot:<br/><img src=\"{rel}\" alt=\"screenshot\" /></div>");
                    }
                    sb.AppendLine("</div>");
                }

                sb.AppendLine("</body>");
                sb.AppendLine("</html>");

                // Ensure directory exists
                var dir = Path.GetDirectoryName(ReportPath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

                File.WriteAllText(ReportPath, sb.ToString(), Encoding.UTF8);

                // Try to open the report in default browser
                if (!string.IsNullOrEmpty(ReportPath) && File.Exists(ReportPath))
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo { FileName = ReportPath, UseShellExecute = true });
                    }
                    catch { }
                }
            }
            catch { }
        }

        private static string MakeRelativePath(string fromPath, string toPath)
        {
            try
            {
                Uri fromUri = new Uri(AppendDirectorySeparatorChar(fromPath));
                Uri toUri = new Uri(toPath);

                if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

                Uri relativeUri = fromUri.MakeRelativeUri(toUri);
                var relativePath = Uri.UnescapeDataString(relativeUri.ToString());

                if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
                {
                    relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                }

                return relativePath;
            }
            catch
            {
                return toPath;
            }
        }

        private static string AppendDirectorySeparatorChar(string path)
        {
            if (string.IsNullOrEmpty(path)) return path;
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString())) return path;
            return path + Path.DirectorySeparatorChar;
        }

        private class TestResult
        {
            public string Name { get; set; }
            public string Status { get; set; }
            public List<string> Messages { get; set; }
            public string ScreenshotPath { get; set; }
        }
    }
}
