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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using MiniTC.ViewModel;

namespace MiniTC
{
    /// <summary>
    /// Logika interakcji dla klasy PanelTC.xaml
    /// </summary>
    public partial class PanelTC : UserControl
    {
        public PanelTC()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty PathProperty =
        DependencyProperty.Register(
        "Path",
        typeof(string),
        typeof(PanelTC),
        new FrameworkPropertyMetadata(null));

        public string Path
        {
            get { return (string)GetValue(PathProperty); }
            set
            {
                SetValue(PathProperty, value);
                //Aktualizacja zawartosci listy plikow ze wzgledu na zmieniona sciezke
                if (Directory.Exists(Path))
                    RefreshContent(value);
            }
        }

        public static readonly DependencyProperty DrivesProperty =
        DependencyProperty.Register(
        "Drives",
        typeof(string[]),
        typeof(PanelTC),
        new FrameworkPropertyMetadata(null));

        public string[] Drives
        {
            get { return (string[])GetValue(DrivesProperty); }
            set { SetValue(DrivesProperty, value); }
        }

        public static readonly DependencyProperty PathContentProperty =
        DependencyProperty.Register(
        "PathContent",
        typeof(string[]),
        typeof(PanelTC),
        new FrameworkPropertyMetadata(null));

        public string[] PathContent
        {
            get { return (string[])GetValue(PathContentProperty); }
            set { SetValue(PathContentProperty, value); }
        }

        //Wyznaczenie dostepnych napedow
        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            Drives = Directory.GetLogicalDrives();
        }

        //Wypelnienie listy zawartoscia danej sciezki
        private void RefreshContent(string p)
        {
            string[] dirs = Directory.GetDirectories(p);
            string[] files = Directory.GetFiles(p);
            List<string> filesAndDirs = new List<string>();

            if (!Drives.Contains(p))
                filesAndDirs.Add("..");

            foreach (var v in dirs)
            {
                //Pominiecie folderow odmawiajacych dostepu
                try
                {
                    Directory.GetDirectories(v);
                }
                catch (Exception)
                {
                    continue;
                }
                filesAndDirs.Add("<D> " + v.Substring(p.Length));
            }
            foreach (var v in files)
                filesAndDirs.Add(v.Substring(p.Length));

            PathContent = filesAndDirs.ToArray();
        }
        //Poruszanie sie po dysku
        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListBox).SelectedItem != null)
            {
                string temp = Path;
                if (!Path.Last().Equals((char) 92))
                {
                    int index = temp.LastIndexOf((char)92);
                    temp = temp.Substring(0, index + 1);
                }
                string item = (sender as ListBox).SelectedItem.ToString();
                if (item == "..")
                {
                    temp = System.IO.Path.GetDirectoryName(temp);
                    temp = System.IO.Path.GetDirectoryName(temp);
                    if (!Drives.Contains(temp))
                        temp += "\\";
                    Path = temp;
                    (sender as ListBox).SelectedItem = null;
                    return;
                }
                if (item.Contains("<D> "))
                {
                    temp = temp + item.Substring(4) + "\\";
                    Path = temp;
                    return;
                }

                Path = temp + item;
            }
        }
        //Ustawienie sciezki po wyborze napedu
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Path = ((ComboBox)sender).SelectedItem.ToString();
        }
    }
}
