using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
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
        // Single reporter instance shared across threads
        private static ExtentReports? _extent;
        private static ExtentSparkReporter? _spark;

        // Map features by name so features are grouped and shared across threads
        private static readonly ConcurrentDictionary<string, ExtentTest> _featureMap = new();

        // Thread-local current feature and scenario to keep operations thread-safe
        private static readonly ThreadLocal<ExtentTest?> _currentFeature = new(() => null);
        private static readonly ThreadLocal<ExtentTest?> _currentScenario = new(() => null);

        // Simple lock for initialization
        private static readonly object _initLock = new();

        public static void InitReport()
        {
            lock (_initLock)
            {
                if (_extent != null)
                    return; // already initialized

                var reportDir = Path.Combine(PathHelper.GetProjectRoot(), "Reports");

                if (!Directory.Exists(reportDir))
                    Directory.CreateDirectory(reportDir);

                var reportPath = Path.Combine(reportDir,
                    $"TestAutomationReport_{DateTime.Now:yyyyMMdd_HHmmss}.html");

                _spark = new ExtentSparkReporter(reportPath);

                _spark.Config.DocumentTitle = "OrangeHRM Automation Report";
                _spark.Config.ReportName = "Reqnroll Selenium Automation";
                _spark.Config.Theme = AventStack.ExtentReports.Reporter.Config.Theme.Dark;

                _extent = new ExtentReports();
                _extent.AttachReporter(_spark);
            }
        }

        // Attach a screenshot to the current scenario if available, otherwise to current feature

        // Create or get a shared feature. This groups scenarios under a feature node.
        /// <summary>
        /// Create or select a feature node in the report. Features group scenarios
        /// for better organization when reviewing test results.
        /// </summary>
        public static void CreateFeature(string featureName)
            => CreateFeature(featureName, Array.Empty<string>());

        /// <summary>
        /// Create or select a feature node in the report and optionally assign tags/categories.
        /// </summary>
        public static void CreateFeature(string featureName, params string[] tags)
        {
            if (_extent == null)
                InitReport();

            var feature = _featureMap.GetOrAdd(featureName, name => _extent!.CreateTest($"Feature: {name}"));

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                        feature.AssignCategory(tag);
                }
            }

            _currentFeature.Value = feature;
        }

        // Maintain original signature for compatibility
        /// <summary>
        /// Create a scenario node under the current feature. Overload keeps compatibility
        /// with code that doesn't supply tags.
        /// </summary>
        public static void CreateScenario(string scenarioName)
            => CreateScenario(scenarioName, Array.Empty<string>());

        // Create a scenario node under the current feature and optionally assign tags/categories
        /// <summary>
        /// Create a scenario node under the current feature and optionally assign tags/categories.
        /// Scenarios are used by the framework to track per-test results and attach artifacts.
        /// </summary>
        public static void CreateScenario(string scenarioName, params string[] tags)
        {
            if (_extent == null)
                InitReport();

            var feature = _currentFeature.Value;
            if (feature == null)
            {
                // If no feature has been created for this thread, use a default global feature to avoid null refs
                feature = _featureMap.GetOrAdd("Global", name => _extent!.CreateTest($"Feature: {name}"));
                _currentFeature.Value = feature;
            }

            var scenario = feature.CreateNode($"Scenario: {scenarioName}");

            if (tags != null)
            {
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                        scenario.AssignCategory(tag);
                }
            }

            _currentScenario.Value = scenario;
        }

        // Write an info/log step to the current scenario (preferred) or feature
        /// <summary>
        /// Log an informational step to the current scenario (preferred) or feature.
        /// Used by hooks to record step-level information in the report.
        /// </summary>
        public static void LogStep(string step)
        {
            var target = _currentScenario.Value ?? _currentFeature.Value;
            target?.Info(step);
        }

        // Mark current scenario as passed (applies to scenario node so counts are correct)
        /// <summary>
        /// Mark the current scenario as passed in the report. This ensures counts and
        /// status are accurate for reporting dashboards.
        /// </summary>
        public static void LogPass(string message)
        {
            var target = _currentScenario.Value ?? _currentFeature.Value;
            target?.Pass(message);
        }

        // Mark current scenario as failed and attach screenshot if provided
        /// <summary>
        /// Mark the current scenario as failed and attach a screenshot if available.
        /// Called by the failure-handling hooks to provide visual context for failures.
        /// </summary>
        public static void LogFail(string message, string screenshot)
        {
            var target = _currentScenario.Value ?? _currentFeature.Value;
            if (target == null)
                return;

            if (!string.IsNullOrEmpty(screenshot))
            {
                try
                {
                    target.Fail(message).AddScreenCaptureFromPath(screenshot);
                }
                catch
                {
                    // If adding screenshot fails, still mark the failure with message
                    target.Fail(message);
                }
            }
            else
            {
                target.Fail(message);
            }
        }

        /// <summary>
        /// Flush in-memory report data to disk. Should be called at the end of the run
        /// so generated HTML contains all logged results and attachments.
        /// </summary>
        public static void FlushReport()
        {
            _extent?.Flush();
        }
    }
}
