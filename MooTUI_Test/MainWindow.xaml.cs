﻿using MooTUI.Core.WPF;
using MooTUI.Drawing;
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
using MooTUI.Core;

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

            Widget.Style = MooTUI.Drawing.Style.Light.Value;
            BoxDrawing.Default = BoxDrawing.Square;

            TabBox tabs = new TabBox(
                new LayoutRect(
                    new FlexSize(80),
                    new FlexSize(40)));

            WPFMooViewer viewer = new WPFFormattedTextViewer(tabs.Width, tabs.Height, 8, 17, 14.725,
                Theme.Basic.Value);
            Content = viewer;
            Background = new SolidColorBrush(Theme.Basic.Value.Palette[MooTUI.Drawing.Color.Base03]);

            new MooInterface(viewer, tabs);

            Container = new LayoutContainer(
                new LayoutRect(
                    new FlexSize(70),
                    new FlexSize(35)),
                Orientation.Vertical,
                crossJustification: LayoutContainer.CrossAxisJustification.STRETCH);

            ScrollBox scroll = new ScrollBox(
                new LayoutRect(75, 30),
                Container);

            tabs.AddTab(scroll, TextSpan.Parse("{base03/altyellow}Tab 01"));

            Button bigboi = new Button(new LayoutRect(new FlexSize(30), new FlexSize(10)),
                new TextSpan("This is a beeg boi"));

            tabs.AddTab(bigboi, TextSpan.Parse("This tab has a button"));

            ListBox list = new ListBox(
                new LayoutRect(
                    new FlexSize(20),
                    new Size(10)),
                new TextSpan("List!"),
                true);

            Container.AddChild(list);

            Button addToList = new Button(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)),
                TextSpan.Parse("{green/}add an item to the list!"));
            addToList.Click += (s, e) =>
            {
                list.Add("This is a {yellow/}listitem!");
            };
            Button removeList = new Button(
                new LayoutRect(
                    new FlexSize(35),
                    new Size(1)),
                TextSpan.Parse("{red/}remove selected listitem!"));
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
                new TextSpan("set the text of the textInput"));
            setText.Click += (s, e) => t.SetText("TEXT :)");

            Container.AddChild(b);
            Container.AddChild(setText);

            Container.AddChild(new Toggle(TextSpan.Parse("Check?")));
        }
    }
}
