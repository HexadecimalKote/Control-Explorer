using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ControlExplorer
{
    class EventLogger : SplitContainer
    {
        object objSelected;
        CheckBox chkBox;
        CheckedListBox lstbox;
        ConsoleControl cons;
        Dictionary<string, Delegate> deleDict = new Dictionary<string, Delegate>();
        static Dictionary<object, ConsoleControl> consDict = new Dictionary<object, ConsoleControl>();

        // Открытое свойство для SelectedObject
        public object SelectedObject
        {
            get { return objSelected; }
            set
            {
                if (objSelected != null)
                    consDict.Remove(objSelected);

                objSelected = value;

                if (objSelected != null)
                    consDict.Add(objSelected, cons);

                FullRefresh();
            }
        }

        // Конструктор
        public EventLogger()
        {
            // В CheckedListBox отображаются все поддерживаемые
            // элементом управления события.
            lstbox = new CheckedListBox();
            lstbox.Parent = Panel1;
            lstbox.Dock = DockStyle.Fill;
            lstbox.CheckOnClick = true;
            lstbox.Sorted = true;
            lstbox.ItemCheck += ListBoxOnItemCheck;

            // Флажок для выбора или сброса всех событий
            chkBox = new CheckBox();
            chkBox.Parent = Panel1;
            chkBox.AutoSize = true;
            chkBox.AutoCheck = false;
            chkBox.ThreeState = true;
            chkBox.Text = "All";
            chkBox.Dock = DockStyle.Top;
            chkBox.Click += CheckBoxOnClick;

            // ConsoleControl для регистрации событий
            cons = new ConsoleControl();
            cons.Parent = Panel2;
            cons.Dock = DockStyle.Fill;
        }

        // Когда установлен или сброшен флажок,
        // отразить изменения в списке.
        private void CheckBoxOnClick(object sender, EventArgs e)
        {
            CheckBox chkBox = (CheckBox)sender;

            // Предоставляем ListBoxOnItemChecked возможность изменения состояния флажка
            if (chkBox.CheckState == CheckState.Unchecked)
            {
                for (int i = 0; i < lstbox.Items.Count; i++)
                    lstbox.SetItemChecked(i, true);
            }
            else
            {
                for (int i = 0; i < lstbox.Items.Count; i++)
                    lstbox.SetItemChecked(i, false);
            }
        }

        // Подключение и отключение обработчиков событий
        private void ListBoxOnItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.CurrentValue == e.NewValue)
                return;

            CheckedListBox listBox = (CheckedListBox)sender;
            string strEvent = lstbox.Items[e.Index].ToString();

            // Подключить обработчик, если элемент отмечен флажком
            if (e.NewValue == CheckState.Checked)
            {
                AttachHandler(strEvent);

                if (lstbox.CheckedIndices.Count + 1 == lstbox.Items.Count)
                    chkBox.CheckState = CheckState.Checked;
                else
                    chkBox.CheckState = CheckState.Indeterminate;
            }
            // В противном случае отключить обработчик
            else
            {
                RemoveHandler(strEvent);

                if (lstbox.CheckedIndices.Count == 1)
                    chkBox.CheckState = CheckState.Unchecked;
                else
                    chkBox.CheckState = CheckState.Indeterminate;
            }
        }

        // FullRefresh вызывается при изменениях SelectedProperty
        private void FullRefresh()
        {
            cons.Clear();
            lstbox.Items.Clear();

            if (SelectedObject == null)
                return;

            // Заполняем массив всемы событиями
            EventInfo[] eventInfo = SelectedObject.GetType().GetEvents();

            lstbox.BeginUpdate();

            // В цикле перичисляем все события
            foreach (EventInfo evtInfo in eventInfo)
            {
                bool bChecked = false;

                // Проверяем событие на предмет того,
                // наследует ли SelectedObject класс Control
                if (SelectedObject.GetType() == typeof(Control))
                    bChecked = true;
                // ... В противном случае проверяем, реализовано ли событие в Control
                else if (SelectedObject is Control)
                    bChecked = typeof(Control).GetEvent(evtInfo.Name) == null;

                lstbox.Items.Add(evtInfo.Name, bChecked);
            }

            lstbox.EndUpdate();
        }

        private void AttachHandler(string strEvent)
        {
            // Информация о конкретном событии
            EventInfo evtInfo = SelectedObject.GetType().GetEvent(strEvent);

            // Тип обработчкыа события
            Type ehType = evtInfo.EventHandlerType;

            // Информация о методе, обработчике события.
            MethodInfo[] methInfo = ehType.GetMethods();

            // Аргументы обработчика события
            ParameterInfo[] parmInfo = methInfo[0].GetParameters();

            // Создаем метод с окончанием "ClickEventHandler"
            // конкретным типом и аргументами.
            DynamicMethod dynaMeth = new DynamicMethod(strEvent + "EventHandler", typeof(void),
                                                       new Type[] { typeof(object), parmInfo[1].ParameterType },
                                                       GetType());

            // ILGenerator обеспечивает генерацию кода метода
            ILGenerator ilg = dynaMeth.GetILGenerator();

            // Обработчик события вызывает стаический метод EventDump.
            MethodInfo eventDump = GetType().GetMethod("EventDump");

            // Генерация кода, заталкивающего имя события b два аргумента в стек,
            // а зетем вызываещего метод EventDump
            ilg.Emit(OpCodes.Ldstr, strEvent);
            ilg.Emit(OpCodes.Ldarg_0);
            ilg.Emit(OpCodes.Ldarg_1);
            ilg.EmitCall(OpCodes.Call, eventDump, null);
            ilg.Emit(OpCodes.Ret);

            // Создание делегата, по типу соответсвующего обработчику события
            Delegate dynaDel = dynaMeth.CreateDelegate(ehType);

            // Наконец устанавливаем обработчик события
            evtInfo.AddEventHandler(SelectedObject, dynaDel);

            // Добавление делегата в словарь для удаления позднее
            deleDict.Add(strEvent, dynaDel);
        }

        // Удаления обработчика события
        private void RemoveHandler(string strEvent)
        {
            EventInfo evtInfo = SelectedObject.GetType().GetEvent(strEvent);
            evtInfo.RemoveEventHandler(SelectedObject, deleDict[strEvent]);
            deleDict.Remove(strEvent);
        }

        // Это статический метод отображает информацию о событии.
        // поскольку метод статический, от должен получать корректный объект ConsoleControl
        // из словаря ConsoleControl (consDict);
        // хотя последний аргумент определен как EventArgs,
        // для большинства событий он будет потомком EventArgs
        public static void EventDump(string strEvent, object sender, EventArgs e)
        {
            // Выбираем корректный объект ConsoleControl из статического словаря
            ConsoleControl cons = consDict[sender];

            // Отображаем информацию о событии вместе с секундами и миллисекундами.
            DateTime dt = DateTime.Now;
            cons.Write("{0}, {1:D3}, {2}", dt.Second % 10, dt.Millisecond, strEvent);

            // Отображаем информацию о всех свойствах EventArgs (или его потомках).
            PropertyInfo[] api = e.GetType().GetProperties();

            foreach (PropertyInfo pi in api)
                cons.Write(" {0}={1}", pi.Name, pi.GetValue(e, null));

            // Если имя события заканчивает на "Changed", отображаем новое свойство.
            if (strEvent.EndsWith("Changed"))
            {
                string strProperty = strEvent.Substring(0, strEvent.Length - 7);
                PropertyInfo pi = sender.GetType().GetProperty(strProperty);
                cons.Write(" [{0} = {1}]", strProperty, pi.GetValue(sender, null));
            }

            // Новая строка
            cons.WriteLine();
        }

    }
}
