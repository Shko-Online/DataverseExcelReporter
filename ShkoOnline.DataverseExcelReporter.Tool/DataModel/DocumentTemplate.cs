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

namespace ShkoOnline.DataverseExcelReporter.Tool.DataModel
{
    public class DocumentTemplate
    {
        public bool IsPersonal { get; set; }

        public DataverseTable Table { get; set; }

        public string ClientData { get; set; }

        public Guid TemplateId { get; set; }

        public int LanguageCode { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Status { get; set; }

        public string DisplayName
        {
            get
            {
                return $"{(IsPersonal? "👤": "🏢")} {Name}";
            }
        }
    }
}
