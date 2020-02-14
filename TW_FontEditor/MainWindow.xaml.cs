using Common;
using CufParser;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace TW_FontEditor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private PackedFileEx _currentEditingPackedFile;

        private const string fontName = "Noto Sans CJK SC Regular";
        private ushort _lastSelectedUnicode = 0;

        private ObservableCollection<CharPropertyItem> _charTable;
        public MainWindow()
        {
            InitializeComponent();
            _charTable = new ObservableCollection<CharPropertyItem>();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PackFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pack文件|*.pack";
            if (openFileDialog.ShowDialog() == true)
            {
                PackFile pack = new PackFileCodec().Open(openFileDialog.FileName);
                ShowPack(pack);
                tbFileName.Text = pack.Filepath;
            }
        }

        private void ShowPack(PackFile pack)
        {
            if (pack == null) return;
            treeViewPackedFiles.Tag = pack;
            
            TreeViewItem root = new TreeViewItem();
            root.Header= Path.GetFileName(pack.Filepath);
            treeViewPackedFiles.Items.Add(root);

            foreach(PackedFile packedFile in pack.Files)
            {
                TreeViewItem childItem = new TreeViewItem();
                childItem.Header = packedFile.Name;
                childItem.Tag = packedFile;
                root.Items.Add(childItem);
                if(packedFile.Name.EndsWith(".ttf"))
                {
                    UseFontFromFile(packedFile.Data);
                }
            }
        }

        private void UseFontFromFile(byte[] content)
        {
            string fileNameNew = AppDomain.CurrentDomain.BaseDirectory + fontName + ".ttf";
            FileStream fileStream = new FileStream(fileNameNew, FileMode.Create, FileAccess.Write);
            fileStream.Write(content, 0, content.Length);
            fileStream.Close();
            FontFamily= new System.Windows.Media.FontFamily("file:///G:/GitHub/TW_FontEditor/TW_FontEditor/bin/Debug/#" + fontName);
            //dataGridCharTable.FontFamily = 
        }

        private void TreeViewPackedFiles_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TreeViewItem treeViewItem = treeViewPackedFiles.SelectedItem as TreeViewItem;
            if (treeViewItem == null) return;
            
            PackedFile packedFile = treeViewItem.Tag as PackedFile;
            if(packedFile!=null)
            {
                ShowPackedFile(packedFile);
            }
        }

        private void ShowPackedFile(PackedFile packedFile)
        {
            if(_currentEditingPackedFile!=null && _currentEditingPackedFile.PackedFile== packedFile)
            {
                return;
            }
            SaveCurrentEditingPackedFile();
            if (packedFile.Name.EndsWith(".cuf") == false) return;
            _currentEditingPackedFile = new PackedFileEx()
            {
                PackedFile = packedFile,
                CufFile = new CufFile(packedFile.Data)
            };
            _charTable.Clear();
            int selectedIndex = -1;
            for(int i=0;i< _currentEditingPackedFile.CufFile.CharTable.Count;i++)
            {
                CharProperty charProperty = _currentEditingPackedFile.CufFile.CharTable[i];
                _charTable.Add(new CharPropertyItem(charProperty));
                if (_lastSelectedUnicode != 0 && _lastSelectedUnicode == charProperty.Unicode)
                {
                    selectedIndex = i;
                }
            }
            dataGridCharTable.ItemsSource = _charTable;
            if(selectedIndex!=-1)
            {
                dataGridCharTable.SelectedIndex = selectedIndex;
            }
            dataGridCharTable.Focus();
        }

        private void SaveCurrentEditingPackedFile()
        {
            if (_currentEditingPackedFile == null) return;
            if(Enumerable.SequenceEqual(_currentEditingPackedFile.PackedFile.Data, _currentEditingPackedFile.CufFile.GetData())==false)
            {
                _currentEditingPackedFile.PackedFile.Data = _currentEditingPackedFile.CufFile.GetData();
            }
        }

        private void SavePackFile()
        {
            SaveCurrentEditingPackedFile();
            PackFile packFile = treeViewPackedFiles.Tag as PackFile;
            bool isDirty = false;
            if (packFile!=null)
            {
                foreach(PackedFile packedFile in packFile.Files)
                {
                    if(packedFile.Modified)
                    {
                        isDirty = true;
                    }
                }
            }
            if(isDirty==false)
            {
                MessageBox.Show("未做更改");
            }
            else
            {
                new PackFileCodec().Save(packFile);
                MessageBox.Show("已经保存");
            }
        }

        private void MenuItemAdd_Click(object sender, RoutedEventArgs e)
        {
            this.FontSize += 1;
        }

        private void MenuItemMinus_Click(object sender, RoutedEventArgs e)
        {
            this.FontSize -= 1;
        }

        private void DataGridCharTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (this.dataGridCharTable.SelectedItem == null) return;
            CharPropertyItem charPropertyItem = this.dataGridCharTable.SelectedItem as CharPropertyItem;
            _lastSelectedUnicode = charPropertyItem.UnicodeValue;
        }

        private void MenuItemSave_Click(object sender, RoutedEventArgs e)
        {
            SavePackFile();
        }
    }
}
