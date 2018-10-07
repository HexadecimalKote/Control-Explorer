using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlExplorer
{
    class ControlExplorer : Form
    {
        const string strResource = "Resources";
        Panel pnl;

        public ControlExplorer()
        {
            Text = "Control Explorer";
            Icon = new Icon(GetType(), strResource + ".ControlExplorer.ico");

            pnl = new Panel();
            pnl.Parent = this;
            pnl.Dock = DockStyle.Fill;

            MenuStrip menu = new MenuStrip();
            menu.Parent = this;
            menu.Items.Add(new ControlMenuItem(MenuItemOnClick));

            ToolStripMenuItem itemHelp = new ToolStripMenuItem("&Help");
            menu.Items.Add(itemHelp);

            ToolStripMenuItem itemAbout = new ToolStripMenuItem();
            itemAbout.Text = "&About Control Explorer...";
            itemAbout.Click += AboutOnClick;
            itemHelp.DropDownItems.Add(itemAbout);

        }

        private void MenuItemOnClick(object sender, EventArgs e)
        {
            // Получаем информацию о меню Item и о классе, который в нем указан
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            Type type = (Type)item.Tag;

            // Подготовка к созданию объекта заданного типа
            ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
            Control ctrl;

            // Попытка создания объекта заданного типа
            try
            {
                ctrl = (Control)ci.Invoke(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Text);
                return;
            }

            // Создаем диалоговое окно с элементом управления PropertyGrid
            PropertiesAndEventDialog dlg = new PropertiesAndEventDialog();
            dlg.Owner = this;
            dlg.Text = item.Text + " Property Grid";
            dlg.SelectedObject = ctrl;
            dlg.Closed += new EventHandler(DialogOnClosed);
            dlg.Show();

            // Если свойству Parent не удается задать значение,
            // это, скорее всего, форма, для которой нужно вызвать Show.
            try
            {
                ctrl.Parent = pnl;
            }
            catch
            {
                ctrl.Show();
            }
        }

        // При закрытии диалогового окна Properties
        // удаляем элемент управления
        private void DialogOnClosed(object sender, EventArgs e)
        {
            PropertiesAndEventDialog dlg = (PropertiesAndEventDialog)sender;
            Control ctrl = (Control)dlg.SelectedObject;
            ctrl.Dispose();
        }

        private void AboutOnClick(object sender, EventArgs e)
        {
            new AboutDialog(strResource).ShowDialog();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // ControlExplorer
            // 
            this.ClientSize = new System.Drawing.Size(604, 304);
            this.Name = "ControlExplorer";
            this.ResumeLayout(false);

        }
    }
}
