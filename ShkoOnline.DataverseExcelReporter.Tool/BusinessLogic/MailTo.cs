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

using System.Diagnostics;

namespace ShkoOnline.DataverseExcelReporter.Tool.BusinessLogic
{
    internal class MailTo
    {
        private const string EMAIL = "sales@shko.online";
        public static void PrepareSendEmail(string subject, string body)
        {
            Process.Start($"mailto:{EMAIL}?subject={EncodeForMailto(subject)}&body={EncodeForMailto(body)}");
        }

        private static string EncodeForMailto(string input)
        {
            return input.Replace(" ", "%20")
                        .Replace("\n", "%0A")
                        .Replace("\r", "")
                        .Replace("&", "%26")
                        .Replace("?", "%3F");
        }
    }
}
