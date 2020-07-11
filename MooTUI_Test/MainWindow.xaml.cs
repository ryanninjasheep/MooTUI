using MooTUI.Input;
using MooTUI.Core.WPF;
using MooTUI.Core;
using MooTUI.Layout;
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

            LayoutContainer layoutContainer = new LayoutContainer(new LayoutRect(100, 30), Orientation.Vertical);

            WPFMooTUIBuilder.GenerateViewer(this, layoutContainer, Theme.Solarized);

            Button_Create = 
                new Button(
                    new MultilineTextSpan(
                        "This is a Button with too much text :p",
                        15, 
                        justification: HJustification.CENTER),
                    new LayoutRect(
                        new FlexSize(17, 17),
                        new FlexSize(8, 8)
                    )
                );
            Button_Create.Click += A_Click;
            Button_Create.InputReceived += A_Input;

            Outline o = 
                new Outline(
                    Button_Create,
                    new SingleLineTextSpan("This is a button!"),
                    BoxDrawingChars.Rounded
                );

            MultilineTextSpan span = new MultilineTextSpan(
                "This is a ton of text, and I'm gonna have it be different colors!", 15);
            span.SetColorInfo(10, new ColorPair(MooTUI.Core.Color.Red, MooTUI.Core.Color.None));
            span.SetColorInfo(15, new ColorPair(MooTUI.Core.Color.Blue, MooTUI.Core.Color.None));
            span.SetColorInfo(12, new ColorPair(MooTUI.Core.Color.Magenta, MooTUI.Core.Color.None));
            span.SetColorInfo(30, new ColorPair(MooTUI.Core.Color.Yellow, MooTUI.Core.Color.None));
            span.SetColorInfo(35, new ColorPair(MooTUI.Core.Color.Base03, MooTUI.Core.Color.Green));

            TextBlock t = TextBlock.FromSpan(span);

            LayoutContainer h =
                new LayoutContainer(
                    new LayoutRect(
                        new FlexSize(10, 10),
                        new FlexSize(16, 8)
                    ),
                    Orientation.Horizontal
                );

            Button otherButton =
                new Button(
                    new MultilineTextSpan(
                        "This is a completely different button!",
                        15,
                        justification: HJustification.CENTER),
                    new LayoutRect(
                        new FlexSize(10, 10),
                        new FlexSize(16, 8)
                    )
                );
            Button otherOtherButton =
                new Button(
                    new SingleLineTextSpan("This is to the right"),
                    new LayoutRect(
                        new FlexSize(10, 10),
                        new FlexSize(16, 8)
                    )
                );
            Outline h_o =
                new Outline(
                    otherOtherButton,
                    new SingleLineTextSpan("This is a different outline"),
                    BoxDrawingChars.Double
                );

            layoutContainer.AddChild(o);
            layoutContainer.AddChild(t);
            layoutContainer.AddChild(h);

            h.AddChild(otherButton);
            h.AddChild(h_o);
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
