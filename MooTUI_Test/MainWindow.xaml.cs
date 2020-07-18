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
        LayoutContainer Buttons;
        Button Button_Create;
        Outline O_Button_Destroy;

        public MainWindow()
        {
            InitializeComponent();

            LayoutContainer layoutContainer = new LayoutContainer(new LayoutRect(100, 30), Orientation.Vertical);

            WPFMooTUIBuilder.GenerateViewer(this, layoutContainer, Theme.Solarized);

            Buttons = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(10),
                    new FlexSize(10)
                ),
                Orientation.Horizontal
            );

            Button_Create = 
                new Button(
                    "Create a button!",
                    new LayoutRect(
                        new FlexSize(15),
                        new FlexSize(5)
                    )
                );
            Button_Create.Click += Create_Click;

            Button Button_Destroy =
                new Button(
                    "Destroy this button!",
                    new LayoutRect(
                        new FlexSize(15),
                        new FlexSize(5)
                    )
                );
            Button_Destroy.Text.SetColorInfo(0, new ColorPair(MooTUI.Core.Color.Red, MooTUI.Core.Color.None));
            Button_Destroy.Click += Destroy_Click;

            Outline c_o = 
                new Outline(
                    Button_Create,
                    "This has a lot of text!"
                );
            O_Button_Destroy =
                new Outline(
                    Button_Destroy
                );

            layoutContainer.AddChild(Buttons);

            Buttons.AddChild(c_o);
        }

        private void Destroy_Click(object sender, EventArgs e)
        {
            Buttons.RemoveChild(O_Button_Destroy);
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Buttons.AddChild(O_Button_Destroy);
        }
    }
}
