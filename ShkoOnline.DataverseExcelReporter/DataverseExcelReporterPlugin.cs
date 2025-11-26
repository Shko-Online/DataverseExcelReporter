/*
   Copyright 2025 Shko Online LLC <sales@shko.online>

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using ShkoOnline.DataverseExcelReporter.Tool;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using XrmToolBox.Extensibility;
using XrmToolBox.Extensibility.Interfaces;

namespace ShkoOnline.DataverseExcelReporter
{
    [Export(typeof(IXrmToolBoxPlugin)),
     ExportMetadata("Name", "Dataverse Excel Reporter"),
     ExportMetadata("Company", "Shko Online"),
     ExportMetadata("Description", "This tool uses personal or system excel templates to generate reports. It doesn't have the same limitations as the out of the box reports as it can properly paginate retrieval and doesn't timeout."),
     ExportMetadata("SmallImageBase64", Resources.ShkoOnline32x32),
     ExportMetadata("BigImageBase64", Resources.ShkoOnline80x80),
     ExportMetadata("BackgroundColor", "White"),
     ExportMetadata("PrimaryFontColor", "#E40046"),
     ExportMetadata("SecondaryFontColor", "#C9003C")]
    public class DataverseExcelReporterPlugin : PluginBase
    {
        static readonly string toolAssignedFolder;
        static readonly HashSet<string> referencedAssemblies = new HashSet<string>();

        public DataverseExcelReporterPlugin()
        {
            // Subscribe to assembly resolve event to load dependent assemblies from a subfolder
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(AssemblyResolveEventHandler);
        }

        public override IXrmToolBoxPluginControl GetControl()
        {
            return new DataverseExcelReporterControl();
        }

        /// <summary>
        /// Event fired by CLR when an assembly reference fails to load
        /// Assumes that related assemblies will be loaded from a subfolder named the same as the Plugin
        /// For example, a folder named DataverseExcelReporter containing all dependent assemblies 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        private static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            Assembly loadAssembly = null;

            // base name of the assembly that failed to resolve
            var argName = args.Name.Substring(0, args.Name.IndexOf(",", StringComparison.InvariantCulture));
            var cultureFolder = "";
            if (argName.EndsWith(".resources", StringComparison.InvariantCultureIgnoreCase))
            {
                argName = argName.Substring(0, argName.IndexOf(".resources", StringComparison.InvariantCulture));
                cultureFolder = args.Name.Substring(args.Name.IndexOf("Culture=", StringComparison.InvariantCulture) + 8);
                cultureFolder = cultureFolder.Substring(0, cultureFolder.IndexOf(",", StringComparison.InvariantCulture));
            }

            var referencedAssembly = referencedAssemblies.Contains(argName) ? argName : null;

            // if the current unresolved assembly is referenced by our plugin, attempt to load
            if (referencedAssembly != null)
            {
                // load from the path to this plugin assembly, not host executable
                var assmbPath = Path.Combine(toolAssignedFolder, cultureFolder, $"{argName}.dll");

                if (File.Exists(assmbPath))
                {
                    loadAssembly = Assembly.LoadFrom(assmbPath);
                    GetReferencedAssemblies(loadAssembly);
                }
                else if (string.IsNullOrEmpty(cultureFolder))
                {
                    throw new FileNotFoundException($"Unable to locate dependency: {args.Name}", assmbPath);
                }
            }

            return loadAssembly;
        }

        static DataverseExcelReporterPlugin()
        {
            var currentAssembly = typeof(DataverseExcelReporterPlugin).Assembly;
            toolAssignedFolder = Path.Combine(Path.GetDirectoryName(currentAssembly.Location), Path.GetFileNameWithoutExtension(currentAssembly.Location));
            GetReferencedAssemblies(currentAssembly);
        }

        private static void GetReferencedAssemblies(Assembly assembly)
        {
            foreach (var reference in assembly.GetReferencedAssemblies())
            {
                if (!referencedAssemblies.Contains(reference.Name))
                {
                    referencedAssemblies.Add(reference.Name);
                }
            }
        }
    }
}
