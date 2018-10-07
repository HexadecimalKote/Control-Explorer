using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlExplorer
{
    class ReadOnlyPropertyGrid : Control
    {
        object selected;
        ListView props;
        Label labelProp, labelDesc;
        Timer tmr;

        // Открытое свойство
        public object SelectedObject
        {
            get
            {
                return selected;
            }
            set
            {
                selected = value;
                Fullrefresh();
                tmr.Enabled = selected != null;
            }
        }

        // Конструктор 
        public ReadOnlyPropertyGrid()
        {
            ClientSize = new Size(30 * Font.Height, 30 * Font.Height);

            SplitContainer sc = new SplitContainer();
            sc.Parent = this;
            sc.Dock = DockStyle.Fill;
            sc.Orientation = Orientation.Horizontal;
            sc.FixedPanel = FixedPanel.Panel2;
            sc.SplitterDistance = Height - 4 * Font.Height;

            // ListView отображает свойства и значения
            props = new ListView();
            props.Parent = sc.Panel1;
            props.Dock = DockStyle.Fill;
            props.View = View.Details;
            props.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            props.GridLines = true;
            props.FullRowSelect = true;
            props.MultiSelect = false;
            props.HideSelection = false;
            props.Activation = ItemActivation.OneClick;
            props.SelectedIndexChanged += ListViewOnSelectedIndexChanged;

            props.Columns.Add("Property", 12 * Font.Height, HorizontalAlignment.Left);
            props.Columns.Add("Value", 18 * Font.Height, HorizontalAlignment.Left);

            // Метки отображают выбранное свойство и его значение
            labelDesc = new Label();
            labelDesc.Parent = sc.Panel2;
            labelDesc.Dock = DockStyle.Fill;

            labelProp = new Label();
            labelProp.Parent = sc.Panel2;
            labelProp.Dock = DockStyle.Top;
            labelProp.Height = Font.Height;
            labelProp.Font = new Font(labelProp.Font, FontStyle.Bold);

            tmr = new Timer();
            tmr.Interval = 100;
            tmr.Tick += TimerOnClick;
        }

        // По сигналу таймера обновляем все значения свойств
        private void TimerOnClick(object sender, EventArgs e)
        {
            ValueRefresh();
        }

        private void ListViewOnSelectedIndexChanged(object sender, EventArgs e)
        {
            ListView lv = (ListView)sender;

            if (lv.SelectedItems.Count == 0)
            {
                labelProp.Text = labelDesc.Text = "";
                return;
            }

            ListViewItem lvi = lv.SelectedItems[0];
            PropertyInfo pi = (PropertyInfo)lvi.Tag;
            DescriptionAttribute dattr = (DescriptionAttribute)Attribute.GetCustomAttribute(pi, typeof(DescriptionAttribute));

            labelProp.Text = pi.Name;
            labelDesc.Text = dattr == null ? "" : dattr.Description;
        }

        private void Fullrefresh()
        {
            props.Items.Clear();

            if (SelectedObject == null)
                return;

            PropertyInfo[] api = SelectedObject.GetType().GetProperties();

            foreach (PropertyInfo pi in api)
            {
                if (pi.CanRead && !pi.CanWrite)
                {
                    ListViewItem lvi = new ListViewItem(pi.Name);
                    lvi.Tag = pi;
                    object objValue = pi.GetValue(SelectedObject, null);
                    lvi.SubItems.Add(objValue == null ? "" : objValue.ToString());
                    props.Items.Add(lvi);
                    lvi.Selected = pi.Name == "Bottom";
                }
            }
        }

        // Обновляем значения всех изменившихся свойств
        private void ValueRefresh()
        {
            foreach (ListViewItem lvi in props.Items)
            {
                PropertyInfo pi = (PropertyInfo)lvi.Tag;
                object objValue = pi.GetValue(SelectedObject, null);
                string strNew = objValue == null ? "" : objValue.ToString();

                if (strNew != lvi.SubItems[1].Text)
                    lvi.SubItems[1].Text = strNew;
            }
        }
    }
}
