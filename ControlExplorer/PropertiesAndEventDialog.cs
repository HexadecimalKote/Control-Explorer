using System.Drawing;
using System.Windows.Forms;

namespace ControlExplorer
{
    class PropertiesAndEventDialog : Form
    {
        PropertyGrid propGrid;
        ReadOnlyPropertyGrid readPropGrid;
        EventLogger eventLogger;

        // SelectedObject распределяет свойства на другие элементы управления
        public object SelectedObject
        {
            get { return propGrid.SelectedObject; }
            set
            {
                propGrid.SelectedObject = value;
                readPropGrid.SelectedObject = value;
                eventLogger.SelectedObject = value;
            }
        }

        // Конструктор
        public PropertiesAndEventDialog()
        {
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(800, 600);

            SplitContainer split1 = new SplitContainer();
            split1.Parent = this;
            split1.Dock = DockStyle.Fill;
            split1.SplitterDistance = ClientSize.Height / 2;
            split1.Orientation = Orientation.Horizontal;

            SplitContainer split2 = new SplitContainer();
            split2.Parent = split1.Panel1;
            split2.Dock = DockStyle.Fill;
            split2.SplitterDistance = ClientSize.Width / 2;

            // Элемент управлени PropertyGrid.
            propGrid = new PropertyGrid();
            propGrid.Parent = split2.Panel1;
            propGrid.Dock = DockStyle.Fill;

            // Элемнет управления ReadOnlyPropertyGrid
            readPropGrid = new ReadOnlyPropertyGrid();
            readPropGrid.Parent = split2.Panel2;
            readPropGrid.Dock = DockStyle.Fill;

            // Элемент управления EventLogger.
            eventLogger = new EventLogger();
            eventLogger.Parent = split1.Panel2;
            eventLogger.Dock = DockStyle.Fill;
        }
    }
}
