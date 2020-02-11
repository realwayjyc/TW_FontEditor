using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TW_FontEditor
{
    /// <summary>
    /// CharEditPanel.xaml 的交互逻辑
    /// </summary>
    public partial class CharEditPanel : UserControl
    {
        public CharEditPanel()
        {
            InitializeComponent();
            TextBlock tb = new TextBlock();

            tb.Text = "A";
            canvasFont.Children.Add(tb);
        }
    }
}
