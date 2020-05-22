using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Input;
using System.Diagnostics;

namespace MiniTC.ViewModel
{

    class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
        }

        public string FirstPath { get; set; }
        string _secondPath = null;
        public string SecondPath 
        {
            get => _secondPath; 
            set
            {
                _secondPath = value;
                onPropertyChanged(nameof(SecondPath));
            }
        }
        string[] _secondContent = null;
        public string[] SecondContent
        {
            get => _secondContent;
            set
            {
                _secondContent = value;
                onPropertyChanged(nameof(SecondContent));
            }
        }

        private ICommand _copy = null;
        public ICommand Copy
        {
            get
            {
                if (_copy == null)
                {
                    _copy = new RelayCommand(
                        arg =>
                        {
                            try
                            {
                                if (SecondPath.Last().Equals((char)92))
                                {
                                    CopyFile(FirstPath, SecondPath);
                                }
                                else
                                {
                                    string temp = Path.GetDirectoryName(SecondPath);
                                    CopyFile(FirstPath, temp);
                                }
                                SecondContent = PathContentRefreshed(SecondContent, Path.GetFileName(FirstPath));
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        },
                        arg =>
                        {
                            if (!String.IsNullOrEmpty(FirstPath) && !String.IsNullOrEmpty(SecondPath))
                                if (!FirstPath.Last().Equals((char)92))
                                    return true;
                            return false;
                        });
                }

                return _copy;
            }
        }

        private void CopyFile(string file, string dir)
        {
            string destination = Path.Combine(dir, Path.GetFileName(file));
            File.Copy(file, destination, true);
        }

        private string[] PathContentRefreshed(string[] content, string file)
        {
            List<string> c = content.ToList();
            c.Add(file);

            return c.ToArray();
        }
    }
}
