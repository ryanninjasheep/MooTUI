using MooTUI.Input;
using MooTUI.Core.WPF;
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

            WPFMooViewer viewer = new WPFMooViewer(100, 30, Theme.Hybrid);
            Content = viewer;

            Window window = new Window(100, 30);
            window.SetStyle(MooTUI.IO.Style.SimpleLight, true);

            MooInterface @interface = new MooInterface(viewer, window);

            Canvas = new Canvas(150, 30);
            window.SetContent(Canvas);

            Button_Create = new Button("[ Create Button ]");
            Button_Create.Click += A_Click;
            Button_Create.InputReceived += A_Input;
            Canvas.AddChild(Button_Create, 10, 3);

            Button_Destroy = new Button("[ Destroy This Button ]");
            Button_Destroy.Click += B_Click;
            Button_Destroy.InputReceived += B_Input;

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
                ListBox.GenerateMessage(Message.MessageType.WARNING, "Item removed from ListBox");
            }
        }

        private void Box_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is ListBox b)
            {
                b.GenerateMessage(Message.MessageType.MESSAGE, "The selected index is " + b.SelectedIndex.ToString());
            }
        }

        private void A_Input(object sender, MooTUI.Input.InputEventArgs e)
        {
            if (sender == Button_Create)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        Button_Create.GenerateMessage(Message.MessageType.INFO, "This will create a new button!");
                        break;
                }
            }
        }

        private void A_Click(object sender, EventArgs e)
        {
            Button_Create.GenerateMessage(Message.MessageType.INFO, "New button created!");
            Canvas.AddChild(Button_Destroy, 5, 5);
        }

        private void B_Input(object sender, MooTUI.Input.InputEventArgs e)
        {
            if (sender == Button_Destroy)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        Button_Destroy.GenerateMessage(Message.MessageType.WARNING, "Are you sure you want to destroy this button?");
                        break;
                }
            }
        }

        private void B_Click(object sender, EventArgs e)
        {
            Button_Destroy.GenerateMessage(Message.MessageType.ERROR, "BUTTON DESTROYED");
            Canvas.RemoveChild(Button_Destroy);
        }
    }
}
