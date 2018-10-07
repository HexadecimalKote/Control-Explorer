using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace ControlExplorer
{
    class AboutDialog : Form
    {
        protected FlowLayoutPanel flow;
        protected Button btnOk;

        public AboutDialog(string strResource)
        {
            // Получаем информацию о текущей сборке
            Assembly a = GetType().Assembly;

            // Получаем название программы
            AssemblyTitleAttribute assemblyTitle = (AssemblyTitleAttribute)a.GetCustomAttributes(typeof(AssemblyTitleAttribute), false)[0];
            string strTitle = assemblyTitle.Title;

            // Получаем версию программы
            AssemblyFileVersionAttribute assemblyFileVersion = (AssemblyFileVersionAttribute)a.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), false)[0];
            string strVersion = assemblyFileVersion.Version.Substring(0, 3);

            // Получаем информацию о правообладателе
            AssemblyCopyrightAttribute assemblyCopyright = (AssemblyCopyrightAttribute)a.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)[0];
            string strCopyright = assemblyCopyright.Copyright;

            // Инициализируем атрибуты
            Text = "About " + strTitle;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            ControlBox = false;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowInTaskbar = false;
            Icon = ActiveForm.Icon;
            StartPosition = FormStartPosition.Manual;
            Location = ActiveForm.Location + SystemInformation.CaptionButtonSize + SystemInformation.FrameBorderSize;

            // Создаем элементы управления
            flow = new FlowLayoutPanel();
            flow.Parent = this;
            flow.AutoSize = true;
            flow.FlowDirection = FlowDirection.TopDown;

            FlowLayoutPanel flow2 = new FlowLayoutPanel();
            flow2.Parent = flow;
            flow2.AutoSize = true;
            flow2.Margin = new Padding(Font.Height);

            PictureBox picBox = new PictureBox();
            picBox.Parent = flow2;
            picBox.Image = Icon.ToBitmap();
            picBox.SizeMode = PictureBoxSizeMode.AutoSize;
            picBox.Anchor = AnchorStyles.None;

            Label lbl = new Label();
            lbl.Parent = flow2;
            lbl.AutoSize = true;
            lbl.Anchor = AnchorStyles.None;
            lbl.Text = strTitle + " Version " + strVersion;
            lbl.Font = new Font(FontFamily.GenericSerif, 24, FontStyle.Italic);

            lbl = new Label();
            lbl.Parent = flow;
            lbl.Text = "From the Microsoft Press book: ";
            lbl.AutoSize = true;
            lbl.Anchor = AnchorStyles.None;
            lbl.Margin = new Padding(Font.Height);
            lbl.Font = new Font(FontFamily.GenericSerif, 16);

            picBox = new PictureBox();
            picBox.Parent = flow;
            picBox.Image = new Bitmap(GetType(), strResource + ".BookCover.png");
            picBox.SizeMode = PictureBoxSizeMode.AutoSize;
            picBox.Anchor = AnchorStyles.None;

            LinkLabel lnk = new LinkLabel();
            lnk.Parent = flow;
            lnk.AutoSize = true;
            lnk.Anchor = AnchorStyles.None;
            lnk.Margin = new Padding(Font.Height);
            lnk.Text = "\x00A9 2018 by DeepNetwork";
            lnk.Font = lbl.Font;
            lnk.LinkArea = new LinkArea(10, 21);
            lnk.LinkClicked += delegate { Process.Start("https://google.com.ua"); };

            btnOk = new Button();
            btnOk.Text = "Ok";
            btnOk.Parent = flow;
            btnOk.AutoSize = true;
            btnOk.Anchor = AnchorStyles.None;
            btnOk.Margin = new Padding(Font.Height);
            btnOk.DialogResult = DialogResult.OK;
        }
    }
}