using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrangeHRM_SeleniumCSharp.com.orangeHRM.qa.Utilities
{
    public  class PathHelper
    {
        /// <summary>
        /// Resolves the project root directory by walking up from the current working directory
        /// until a marker folder ("Drivers") is found. Tests and helpers use this to locate
        /// resources (reports, screenshots, config) using paths relative to the repository root.
        /// </summary>
        public static string GetProjectRoot()
        {
            var dir = Directory.GetCurrentDirectory();
            while (!Directory.Exists(Path.Combine(dir, "Drivers")))
            {
                dir = Directory.GetParent(dir)?.FullName ?? throw new InvalidOperationException("Project root not found.");
            }
            return dir;
        }
    }
}
