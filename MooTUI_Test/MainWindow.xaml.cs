using MooTUI.Core;
using MooTUI.IO;
using MooTUI.Widgets;
using MooTUI.Widgets.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MooTUI_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Button Button_Create;
        Button Button_Destroy;
        Canvas Canvas;
        ListBox ListBox;

        public MainWindow()
        {
            InitializeComponent();

            MooViewer viewer = new MooViewer();
            Content = viewer;
            Window window = new Window(viewer.MaxContentWidth, viewer.MaxContentHeight);
            // window.SetStyle(MooTUI.IO.Style.Test, true);
            MooInterface @interface = new MooInterface(viewer, window);

            Canvas = new Canvas(100, 50);
            window.SetContent(Canvas);

            Button_Create = new Button("[ Create Button ]");
            Button_Create.Click += A_Click;
            Button_Create.InputEventHandler += A_Input;
            Canvas.AddChild(Button_Create, 10, 3);

            Button_Destroy = new Button("[ Destroy This Button ]");
            Button_Destroy.Click += B_Click;
            Button_Destroy.InputEventHandler += B_Input;

            ListBox = new ListBox(20, 4);
            ListBox.Add("Item 1");
            ListBox.Add("Item 2");
            ListBox.Add("Item 3");
            ListBox.Add("Item 4");
            ListBox.Add("Item 5");
            ListBox.Add("Item 6");
            ListBox.Add("Item 7");
            Canvas.AddChild(ListBox, 10, 10);
            ListBox.SelectionChanged += Box_SelectionChanged;

            Button Button_Remove = new Button("[ Remove selected ListBox Item ]");
            Button_Remove.Click += Button_Remove_Click;
            Canvas.AddChild(Button_Remove, 30, 5);

            Canvas.AddChild(
                new Outline(
                    new ScrollViewer(10, 2,
                        new ExpandingTextBox(5), ScrollViewer.ScrollBarVisibility.AUTO, ScrollViewer.ScrollBarVisibility.DISABLED),
                    "Text"), 30, 15);
        }

        private void Button_Remove_Click(object sender, EventArgs e)
        {
            if (ListBox.SelectedIndex != -1)
            {
                ListBox.RemoveAt(ListBox.SelectedIndex);
                ListBox.BubbleMessage(new Message(Message.MessageType.WARNING, "Item removed from ListBox", ListBox));
            }
        }

        private void Box_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is ListBox b)
            {
                b.BubbleMessage(new Message(Message.MessageType.MESSAGE,
                    "The selected index is " + b.SelectedIndex.ToString(), b));
            }
        }

        private void A_Input(object sender, MooTUI.Core.InputEventArgs e)
        {
            if (sender == Button_Create)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        Button_Create.BubbleMessage(new Message(Message.MessageType.INFO, "This will create a new button!", Button_Create));
                        break;
                }
            }
        }

        private void A_Click(object sender, EventArgs e)
        {
            Button_Create.BubbleMessage(new Message(Message.MessageType.INFO, "New button created!", Button_Create));
            Canvas.AddChild(Button_Destroy, 5, 5);
        }

        private void B_Input(object sender, MooTUI.Core.InputEventArgs e)
        {
            if (sender == Button_Destroy)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        Button_Destroy.BubbleMessage(new Message(Message.MessageType.WARNING, 
                            "Are you sure you want to destroy this button?", Button_Destroy));
                        break;
                }
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            Button_Destroy.BubbleMessage(new Message(Message.MessageType.ERROR, "BUTTON DESTROYED", Button_Destroy));
            Canvas.RemoveChild(Button_Destroy);
        }
    }
}
