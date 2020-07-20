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
        LayoutContainer Container;

        public MainWindow()
        {
            InitializeComponent();

            Container = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(100),
                    new FlexSize(3)),
                Orientation.Vertical,
                LayoutContainer.MainAxisJustification.FIT);

            ScrollBox scroll = new ScrollBox(
                new LayoutRect(200, 50),
                Container);

            WPFMooTUIBuilder.GenerateViewer(this, scroll, Theme.Solarized);

            Button create = new Button(
                "{altyellow/}create a new button!",
                new LayoutRect(
                    new FlexSize(30),
                    new Size(3)));
            create.Click += Create_Click;

            Container.AddChild(create);
        }

        private void Create_Click(object sender, EventArgs e)
        {
            Button b = new Button(
                "delete this button",
                new LayoutRect(
                    new FlexSize(30),
                    new Size(3)));
            b.Click += (b, e) => Container.RemoveChild(b as Widget);

            Container.AddChild(b);
        }
    }
}
