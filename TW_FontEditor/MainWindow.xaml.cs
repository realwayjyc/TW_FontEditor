using Common;
using CufParser;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();
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
            }
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
            _currentEditingPackedFile = new PackedFileEx()
            {
                PackedFile = packedFile,
                CufFile = new CufFile(packedFile.Data)
            };
            _currentEditingPackedFile.CufFile.MaxGlyphHeight = 100;
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
            PackFile packFile = treeViewPackedFiles.Tag as PackFile;
            if(packFile!=null)
            {
                bool isDirty = false;
                foreach(PackedFile packedFile in packFile.Files)
                {
                    isDirty = true;
                }
            }
        }
    }
}
