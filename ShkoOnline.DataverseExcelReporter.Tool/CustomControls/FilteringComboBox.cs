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
using System.ComponentModel;
using System.Windows.Forms;

namespace ShkoOnline.DataverseExcelReporter.Tool.CustomControls
{
    public partial class FilteringComboBox<T> : ComboBox
    {
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<T> BindingValues { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<T, string, bool> FilterPredicate { get; set; }

        protected override void OnTextUpdate(EventArgs e)
        {
            var values = BindingValues;
            var filterPredicate = FilterPredicate;

            if (values != null && filterPredicate != null)
            {
                string filter = Text;

                var filteredDocuments = string.IsNullOrWhiteSpace(filter) ?
                    values :
                    values.FindAll(t => filterPredicate(t, filter));
          
                DataSource = filteredDocuments;
                SelectedIndex = -1;
                DroppedDown = true;
                SelectedItem = null;
                Text = filter;
                SelectionStart = filter.Length;
                SelectionLength = 0;
            }

            base.OnTextUpdate(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.SelectionStart += this.SelectionLength;
                this.SelectionLength = 0;
                return;
            }
            if (this.SelectionLength == 0 ||
                e.KeyChar == (char)Keys.Add ||
                e.KeyChar == (char)Keys.LineFeed ||
                e.KeyChar == (char)Keys.Enter)
            {
                return;
            }
            var text = this.Text;
            string charUpdate = "";
            if (e.KeyChar != (char)Keys.Back && e.KeyChar != (char)Keys.Delete)
            {
                charUpdate += e.KeyChar;
            }
            var selectionStart = this.SelectionStart + charUpdate.Length;
            text = text.Substring(0, this.SelectionStart) + charUpdate + text.Substring(this.SelectionStart + this.SelectionLength);
            this.Text = text;
            this.SelectionStart = selectionStart;
            this.SelectionLength = 0;

            base.OnKeyPress(e);
        }
    }
}
