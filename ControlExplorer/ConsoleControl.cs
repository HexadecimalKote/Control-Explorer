using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlExplorer
{
    class ConsoleControl : Control
    {
        TextBox txtBox;

        public ConsoleControl()
        {
            txtBox = new TextBox();
            txtBox.Parent = this;
            txtBox.Multiline = true;
            txtBox.WordWrap = false;
            txtBox.ScrollBars = ScrollBars.Both;
            txtBox.ReadOnly = true;
            txtBox.Dock = DockStyle.Fill;
            txtBox.TabStop = false;
            txtBox.HideSelection = false;
        }

        public void Clear()
        {
            txtBox.Clear();
        }

        public void WriteLine()
        {
            Output("\r\n");
        }

        public void Write(object obj)
        {
            Output(obj.ToString());
        }

        public void WriteLine(object obj)
        {
            Output(obj + "\r\n");
        }

        public void Write(string strFormat, params object[] arrObj)
        {
            Output(String.Format(strFormat, arrObj));
        }

        public void WriteLine(string strFormat, params object[] arrObj)
        {
            Output(String.Format(strFormat, arrObj) + "\r\n");
        }

        private void Output(string str)
        {
            txtBox.SelectionStart = txtBox.TextLength;
            txtBox.AppendText(str);
        }
    }
}
