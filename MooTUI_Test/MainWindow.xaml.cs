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

            Widget.Style = MooTUI.IO.Style.Light;

            Container = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(30),
                    new FlexSize(3)),
                Orientation.Vertical,
                crossJustification: LayoutContainer.CrossAxisJustification.STRETCH);

            ScrollBox scroll = new ScrollBox(
                new LayoutRect(50, 20),
                Container,
                lineStyle: BoxDrawing.Leaf);

            WPFMooTUIBuilder.GenerateViewer(this, scroll, Theme.Solarized);

            ListBox list = new ListBox(
                new LayoutRect(
                    new FlexSize(20),
                    new FlexSize(10)),
                "List!",
                BoxDrawing.Leaf);

            Container.AddChild(list);

            Button addToList = new Button(
                "{green/}add an item to the list!",
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)));
            addToList.Click += (s, e) =>
            {
                list.Add("This is a {yellow/}listitem!");
            };
            Button removeList = new Button(
                "{red/}remove selected listitem!",
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)));
            removeList.Click += (s, e) =>
            {
                list.RemoveElementUnderCursor();
            };

            Container.AddChild(addToList);
            Container.AddChild(removeList);

            TextInput t = new TextInput(
                new LayoutRect(
                    new Size(80),
                    new Size(2)),
                "Prompt!!!");

            ScrollBox b = new ScrollBox(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(5)),
                t);

            Container.AddChild(b);
        }
    }
}
