using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace TW_FontEditor
{
    /// <summary>
    /// FillContent.xaml 的交互逻辑
    /// </summary>
    public partial class FillContent : Window
    {
        public FillContent()
        {
            InitializeComponent();
        }

        public bool IsOK { get; set; }
        public byte ValueWidth
        {
            get
            {
                return byte.Parse(txtWidth.Text);
            }
            set
            {
                txtWidth.Text = value.ToString();
            }
        }

        public byte ValueHeight
        {
            get
            {
                return byte.Parse(txtHeight.Text);
            }
            set
            {
                txtHeight.Text = value.ToString();
            }
        }
        public byte ValueUnk1
        {
            get
            {
                return byte.Parse(txtUnk1.Text);
            }
            set
            {
                txtUnk1.Text = value.ToString();
            }
        }
        public byte ValueUnk2
        {
            get
            {
                return byte.Parse(txtUnk2.Text);
            }
            set
            {
                txtUnk2.Text = value.ToString();
            }
        }
        public byte ValueUnk3
        {
            get
            {
                return byte.Parse(txtUnk3.Text);
            }
            set
            {
                txtUnk3.Text = value.ToString();
            }
        }

        public int FromIndex
        {
            get
            {
                return int.Parse(txtFrom.Text);
            }
            set
            {
                txtFrom.Text = value.ToString();
            }
        }

        public int ToIndex
        {
            get
            {
                return int.Parse(txtTo.Text);
            }
            set
            {
                txtTo.Text = value.ToString();
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            IsOK = true;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            IsOK = false;
            this.Close();
        }
    }
}
