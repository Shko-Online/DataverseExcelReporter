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

using System;
using System.Collections.Generic;

namespace ShkoOnline.DataverseExcelReporter.Tool.DataModel
{
    public class DataverseTable
    {
        public string Name { get; set; }
        public string LogicalName { get; set; }
        public string DisplayName
        {
            get
            {
                if (LogicalName == null)
                {
                    return Name;
                }
                return $"{Name} ({LogicalName})";
            }
        }

        public static DataverseTable GetOrCreate(List<DataverseTable> tables, string logicalName, string name)
        {
            var table = tables.Find(t => t.LogicalName.Equals(logicalName, StringComparison.OrdinalIgnoreCase));
            if (table == null)
            {
                table = new DataverseTable()
                {
                    LogicalName = logicalName,
                    Name = name
                };
                tables.Add(table);
            }
            return table;
        }
    }
}
