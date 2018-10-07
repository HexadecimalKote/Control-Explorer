using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlExplorer
{
    public class ControlMenuItem : ToolStripMenuItem
    {

        public ControlMenuItem(EventHandler evtClick)
        {
            // Узнаем сборку, в которой определен класс
            Assembly assembly = Assembly.GetAssembly(typeof(Control));

            // Это массив всех типов этого класса.
            Type[] arrType = assembly.GetTypes();

            // Мы будем хранить наследников Control в сортированном списке
            SortedList<string, ToolStripMenuItem> sortList = new SortedList<string, ToolStripMenuItem>();
            Text = "Control";
            Tag = typeof(Control);
            sortList.Add("Control", this);

            // Перечисляем все типы в массиве
            // для Control и его наследников создаем команды меню и
            // добавляем их в объект SortedList
            // Обратите внимание: свойство Tag команды меню связано с объектом Type
            foreach (Type type in arrType)
            {
                if (type.IsPublic && (type.IsSubclassOf(typeof(Control))))
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(type.Name);
                    item.Click += evtClick;
                    item.Tag = type;
                    sortList.Add(type.Name, item);
                }
            }

            // Перечисляем сортированный список и задаем родителей команд меню.
            foreach (KeyValuePair<string, ToolStripMenuItem> kvp in sortList)
            {
                if (kvp.Key != "Control")
                {
                    string strParent = ((Type)kvp.Value.Tag).BaseType.Name;
                    ToolStripMenuItem itemParent = sortList[strParent];
                    itemParent.DropDownItems.Add(kvp.Value);

                    //У itemParent не должно быть обработчика события
                    itemParent.Click -= evtClick;
                }
            }

            // Еще раз перечисляем список:
            // Если элемент абстрактный и доступен для выбора, отключаем его.
            // Если элемент не абстрактный и не доступен для выбора, добавляем новый элемент.
            foreach (KeyValuePair<string, ToolStripMenuItem> kvp in sortList)
            {
                Type type = (Type)kvp.Value.Tag;

                if (type.IsAbstract && kvp.Value.DropDownItems.Count == 0)
                    kvp.Value.Enabled = false;

                if (!type.IsAbstract && kvp.Value.DropDownItems.Count > 0)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(kvp.Value.Text);
                    item.Click += evtClick;
                    item.Tag = type;
                    kvp.Value.DropDownItems.Insert(0, item);
                }

            }
        }
    }
}
