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
using MooTUI.Text;

namespace MooTUI_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        Button Button_Create;

        public MainWindow()
        {
            InitializeComponent();

            Button_Create = new Button(
                new MultilineTextSpan(
                    "This is a Button with too much text :p",
                    15, 
                    justification: MooTUI.Layout.HJustification.CENTER),
                new MooTUI.Layout.LayoutRect(30, 3)
            );
            Button_Create.Click += A_Click;
            Button_Create.InputReceived += A_Input;

            Outline o = new Outline(
                Button_Create,
                new SingleLineTextSpan("This is a button!"),
                BoxDrawingChars.Rounded
            );

            WPFMooTUIBuilder.GenerateViewer(this, o, Theme.Solarized);
            Button_Create.Render();
        }

        private void A_Input(object sender, MooTUI.Input.InputEventArgs e)
        {
            if (sender == Button_Create)
            {
                switch (e.InputType)
                {
                    case InputTypes.MOUSE_ENTER:
                        //Button_Create.GenerateMessage(Message.MessageType.INFO, "This will create a new button!");
                        break;
                }
            }
        }

        private void A_Click(object sender, EventArgs e)
        {
            //Button_Create.GenerateMessage(Message.MessageType.INFO, "New button created!");
            //Canvas.AddChild(Button_Destroy, 5, 5);
        }
    }
}
