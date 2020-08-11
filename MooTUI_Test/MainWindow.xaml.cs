using MooTUI.Input;
using MooTUI.Core.WPF;
using MooTUI.Core;
using MooTUI.Layout;
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

            Widget.Style = MooTUI.Core.Style.HighContrast.Value;

            TabBox tabs = new TabBox(
                new LayoutRect(
                    new FlexSize(80),
                    new FlexSize(40)),
                BoxDrawing.Leaf);

            WPFMooTUIBuilder.GenerateViewer(this, tabs, Theme.Basic);

            Container = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(70),
                    new FlexSize(35)),
                Orientation.Vertical,
                crossJustification: LayoutContainer.CrossAxisJustification.STRETCH);

            ScrollBox scroll = new ScrollBox(
                new LayoutRect(75, 30),
                Container,
                lineStyle: BoxDrawing.Leaf);

            tabs.AddTab(scroll, TextSpan.Parse("{base03/altyellow}Tab 01"));

            Button bigboi = new Button(new LayoutRect(new FlexSize(30), new FlexSize(10)),
                "This is a beeg boi");

            tabs.AddTab(bigboi, TextSpan.Parse("This tab has a button"));

            ListBox list = new ListBox(
                new LayoutRect(
                    new FlexSize(20),
                    new Size(10)),
                "List!",
                true,
                BoxDrawing.Leaf);

            Container.AddChild(list);

            Button addToList = new Button(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)),
                "{green/}add an item to the list!");
            addToList.Click += (s, e) =>
            {
                list.Add("This is a {yellow/}listitem!");
            };
            Button removeList = new Button(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)),
                "{red/}remove selected listitem!");
            removeList.Click += (s, e) =>
            {
                list.RemoveElementUnderCursor();
            };

            Container.AddChild(addToList);
            Container.AddChild(removeList);

            SimpleTextInput t = new SimpleTextInput(
                new FlexSize(10),
                10,
                false,
                "Prompt!!!");

            ScrollBox b = new ScrollBox(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(3)),
                t,
                vScrollbarVisibility: ScrollBox.ScrollBarVisibility.DISABLED);

            Button setText = new Button(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)),
                "set the text of the textInput");
            setText.Click += (s, e) => t.SetText("TEXT :)");

            Container.AddChild(b);
            Container.AddChild(setText);

            Container.AddChild(new Toggle(TextSpan.Parse("Check?")));
        }
    }
}
