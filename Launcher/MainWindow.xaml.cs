using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

namespace Launcher
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private object lockObject = new object();
        private Timer _timer;
        public MainWindow()
        {
            InitializeComponent();
            Directory.CreateDirectory("D:/ launcherResource");

            #region ЗаполнениеОбъектов
            Uri iconUri = new Uri("D:/launcherResource/mainIcon.png", UriKind.RelativeOrAbsolute);
            this.Icon = BitmapFrame.Create(iconUri);

            Uri imageUri = new Uri("D:/launcherResource/mainImage.png", UriKind.RelativeOrAbsolute);
            mainImage.ImageSource = BitmapFrame.Create(imageUri);
            #endregion
            
            File.Copy("D:/launcherResource/mainIcon.png", "D:/launcherResource/currentIcon.png");
            File.Copy("D:/launcherResource/mainImage.png", "D:/launcherResource/currentImage.png");

            _timer = new Timer(CheckNewFiles, null, 0, 15000);
        }

        private void CheckNewFiles(object obj)//загрузка файлов
        {
            #region ПодгрузкаФайлов
            lock (lockObject)
            {
                string addresIcon = "https://drive.google.com/uc?export=download&confirm=no_antivirus&id=1YxCslyRgs9erucFmNDbNvRbH3SjErB3N";
                string fileIcon = "D:/launcherResource/SecondmainIcon.png";

                string addresImage = "https://drive.google.com/uc?export=download&confirm=no_antivirus&id=1SCpl0uk1En7PUxjmCDeX_B7YD5TO_D6I";
                string fileImage = "D:/launcherResource/SecondmainImage.png";
                using (WebClient web = new WebClient())
                {
                    web.Encoding = Encoding.UTF8;
                    web.DownloadFile(addresIcon, fileIcon);

                    web.Encoding = Encoding.UTF8;
                    web.DownloadFile(addresImage, fileImage);
                }
            }
            #endregion
            #region СравнениеФайлов
            bool isUpdate = false;
            if (!FileCompare("D:/launcherResource/currentIcon.png", "D:/launcherResource/SecondmainIcon.png"))
            {
                isUpdate = true;
            }
            if (!FileCompare("D:/launcherResource/currentImage.png", "D:/launcherResource/SecondmainImage.png"))
            {
                isUpdate = true;
            }
            if (isUpdate)
            {
                MessageBoxResult result = MessageBox.Show("Были получены обновления вы хотите перезагруить программу для их применения", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (MessageBoxResult.Yes == result)
                {
                    Reboot();
                }
            }
            #endregion
        }

        private void Reboot()//перезагрузка системы
        {
            Thread updateThread = new Thread(Update);
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void Update() //заменяет основной файл подгруженным если они различны
        {
            lock (lockObject)
            {
                File.Delete("D:/launcherResource/mainIcon.png");
                System.IO.File.Move("D:/launcherResource/SecondmainIcon.png", "D:/launcherResource/mainIcon.png");
            }
            lock (lockObject)
            {
                File.Delete("D:/launcherResource/mainImage.png");
                System.IO.File.Move("D:/launcherResource/SecondmainIamge.png", "D:/launcherResource/mainImage.png");
            }
        }

        private bool FileCompare(string file1, string file2)//проверка файлов на совпадение    //добросовестно украдено из интернета
        {
            lock (lockObject)
            {
                int file1byte;
                int file2byte;
                FileStream fs1;
                FileStream fs2;
                
                if (file1 == file2)
                {
                    return true;
                }

                fs1 = new FileStream(file1, FileMode.Open);
                fs2 = new FileStream(file2, FileMode.Open);

                if (fs1.Length != fs2.Length)
                {
                    fs1.Close();
                    fs2.Close();
                    
                    return false;
                }
                
                do
                {
                    file1byte = fs1.ReadByte();
                    file2byte = fs2.ReadByte();
                }
                while ((file1byte == file2byte) && (file1byte != -1));
                
                fs1.Close();
                fs2.Close();
                
                return ((file1byte - file2byte) == 0);
            }
        }
    }
}