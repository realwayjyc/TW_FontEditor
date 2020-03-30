using Common;
using CufParser;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

//21814 继  32487  相差 10673
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

        private ObservableCollection<HeaderPropertyItem> _headerItemTable;
        public MainWindow()
        {
            InitializeComponent();
            _charTable = new ObservableCollection<CharPropertyItem>();
            _headerItemTable = new ObservableCollection<HeaderPropertyItem>();
        }

        private void PackFileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Pack文件|*.pack";
            openFileDialog.FileName = "FontModeWH.pack";
            if (openFileDialog.ShowDialog() == true)
            {
                PackFile pack = new PackFileCodec().Open(openFileDialog.FileName);
                ShowPack(pack);
                Title = pack.Filepath;
                MakeBackup(openFileDialog.FileName);
            }
        }

        private void MakeBackup(string fileName)
        {
            string backupFileName = fileName + ".bak";
            if (File.Exists(backupFileName)) return;
            File.Copy(fileName, backupFileName);
        }

        private void ShowPack(PackFile pack)
        {
            if (pack == null) return;
            treeViewPackedFiles.Tag = pack;
            
            if(treeViewPackedFiles.Items.Count==0)
            {
                TreeViewItem root = new TreeViewItem();
                root.Header = Path.GetFileName(pack.Filepath);
                treeViewPackedFiles.Items.Add(root);

                foreach (PackedFile packedFile in pack.Files)
                {
                    TreeViewItem childItem = new TreeViewItem();
                    childItem.Header = packedFile.Name;
                    childItem.Tag = packedFile;
                    root.Items.Add(childItem);
                    if (packedFile.Name.EndsWith(".ttf"))
                    {
                        UseFontFromFile(packedFile.Data);
                    }
                }
            }
            else
            {
                TreeViewItem root = treeViewPackedFiles.Items[0] as TreeViewItem;
                root.Header = Path.GetFileName(pack.Filepath);
                for(int i=0;i< pack.Files.Count;i++)
                {
                    TreeViewItem childItem = root.Items[i] as TreeViewItem;
                    PackedFile packedFile = pack.Files[i];
                    childItem.Header = packedFile.Name;
                    childItem.Tag = packedFile;
                    if (packedFile.Name.EndsWith(".ttf"))
                    {
                        UseFontFromFile(packedFile.Data);
                    }
                }
            }
        }

        private void UseFontFromFile(byte[] content)
        {

            try
            {
                string fileNameNew = AppDomain.CurrentDomain.BaseDirectory + fontName + ".ttf";
                FileStream fileStream = new FileStream(fileNameNew, FileMode.Create, FileAccess.Write);
                fileStream.Write(content, 0, content.Length);
                fileStream.Close();
                FontFamily = new System.Windows.Media.FontFamily("file:///G:/GitHub/TW_FontEditor/TW_FontEditor/bin/Debug/#" + fontName);
            }
            catch (Exception ex)
            {

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
                CharPropertyItem charPropertyItem = new CharPropertyItem(charProperty);
                _charTable.Add(charPropertyItem);
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

            ShowHeaderItemTable(_currentEditingPackedFile.CufFile);
        }

        private void ShowHeaderItemTable(CufFile cufFile)
        {
            if (cufFile == null) return;
            _headerItemTable.Clear();
            PropertyInfo[] propertyInfos= cufFile.GetType().GetProperties();
            foreach(PropertyInfo propertyInfo in propertyInfos)
            {
                if(propertyInfo.PropertyType==typeof(ushort))
                {
                    HeaderPropertyItem headerPropertyItem = new HeaderPropertyItem(cufFile, propertyInfo.Name);
                    _headerItemTable.Add(headerPropertyItem);
                }
            }
            this.dataGridHeaderTable.ItemsSource = _headerItemTable;

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
                MessageBox.Show("保存成功");
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

        private void MenuItemReplace_Click(object sender, RoutedEventArgs e)
        {
            if (treeViewPackedFiles.SelectedItem == null) return;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "*.*|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                FileStream fileStream = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.Read);
                byte[] content = new byte[fileStream.Length];
                fileStream.Read(content, 0,(int)fileStream.Length);
                fileStream.Close();

                TreeViewItem treeViewItem = treeViewPackedFiles.SelectedItem as TreeViewItem;
                PackedFile packedFile = treeViewItem.Tag as PackedFile;
                packedFile.Data = content;
            }
        }

        private void FillContent_Click(object sender, RoutedEventArgs e)
        {
            FillContent fillContent = new FillContent();
            CharPropertyItem charPropertyItemSelected= this.dataGridCharTable.SelectedItem as CharPropertyItem;
            if(charPropertyItemSelected != null)
            {
                fillContent.ValueWidth = charPropertyItemSelected.Width;
                fillContent.ValueHeight = charPropertyItemSelected.Height;
                fillContent.ValueUnk1 = charPropertyItemSelected.Unknown1;
                fillContent.ValueUnk2 = charPropertyItemSelected.Unknown2;
                fillContent.ValueWidthFull = charPropertyItemSelected.WidthFull;
                fillContent.FromIndex = 0;
                fillContent.ToIndex = _charTable.Count - 1;
            }
            fillContent.ShowDialog();
            if (fillContent.IsOK == false) return;
            if(fillContent.FromIndex<0)
            {
                fillContent.FromIndex = 0;
            }
            for(int i= fillContent.FromIndex; i<= fillContent.ToIndex && i< _charTable.Count; i++)
            {
                CharPropertyItem charPropertyItem = _charTable[i];
                charPropertyItem.Width = fillContent.ValueWidth;
                charPropertyItem.Height = fillContent.ValueHeight;
                charPropertyItem.Unknown1 = fillContent.ValueUnk1;
                charPropertyItem.Unknown2 = fillContent.ValueUnk2;
                charPropertyItem.WidthFull = fillContent.ValueWidthFull;
            }
            TreeViewItem treeViewItem = treeViewPackedFiles.SelectedItem as TreeViewItem;
            ShowPackedFile(treeViewItem.Tag as PackedFile);
        }

        private void Restore_Click(object sender, RoutedEventArgs e)
        {
            PackFile packFile = treeViewPackedFiles.Tag as PackFile;
            if(packFile!=null)
            {
                new PackFileCodec().Save(packFile);
            }

            string bakPackFileName = null;
            if(treeViewPackedFiles.Tag == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "*.bak|*.bak";
                if (openFileDialog.ShowDialog() == true)
                {
                    bakPackFileName = openFileDialog.FileName;
                }
            }
            else
            {
                packFile = treeViewPackedFiles.Tag as PackFile;
                bakPackFileName = packFile.Filepath + ".bak";
            }

            string packFileName = bakPackFileName.Substring(0, bakPackFileName.Length - 4);
            if (File.Exists(packFileName))
            {
                File.Delete(packFileName);
            }
            File.Copy(bakPackFileName, packFileName);
            PackFile pack = new PackFileCodec().Open(packFileName);
            ShowPack(pack);
            Title = pack.Filepath;

            _currentEditingPackedFile = null;
            TreeViewItem treeViewItem = treeViewPackedFiles.SelectedItem as TreeViewItem;
            ShowPackedFile(treeViewItem.Tag as PackedFile);
            MessageBox.Show("恢复成功");
        }

        private void SearchChar_Click(object sender, RoutedEventArgs e)
        {
            string text = txtSearchChar.Text;
            int value = 0;
            ushort unicode = 0;
            if(int.TryParse(text,out value)==true)
            {
                if(value<10)
                {
                    unicode = value.ToString().ToCharArray()[0];
                }
                else
                {
                    unicode = (ushort)value;
                }
            }
            else
            {
                unicode = text.ToCharArray()[0];
            }

            for(int i=0;i< _charTable.Count;i++)
            {
               if(_charTable[i].UnicodeValue== unicode)
                {
                    dataGridCharTable.SelectedIndex = i;
                    dataGridCharTable.ScrollIntoView(dataGridCharTable.SelectedItem);
                    dataGridCharTable.Focus();
                    break;
                }
            }
        }

        private void DataGridHeaderTable_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {

        }
    }
}
