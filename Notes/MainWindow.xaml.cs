using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sys = System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MooTUI.Text;
using MooTUI.Core.WPF;
using MooTUI.Drawing;
using MooTUI.Widgets;
using MooTUI.Widgets.Primitives;
using MooTUI.Layout;

namespace Notes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Sys.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GenerateDisplay();
        }

        private void GenerateDisplay()
        {
            BoxDrawing.Default = BoxDrawing.Square;
            Widget.Style = MooTUI.Drawing.Style.Light.Value;
            Box.Enclosure = new TextAreaEnclosure("< ", " >");

            Box box = new Box(GenerateMain(), TextSpan.Parse("NOTES"));

            WPFMooTUIBuilder.GenerateViewer(this, box, Theme.Basic.Value);
        }

        private Widget GenerateMain()
        {
            return new Button(new LayoutRect(30, 5), TextSpan.Parse("This is a button!"));
        }
    }
}
