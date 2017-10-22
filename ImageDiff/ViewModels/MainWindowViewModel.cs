using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Win32;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageDiff.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        #region Properties
        private string _inputImageName1;
        /// <summary>
        /// 入力画像1のパス名
        /// </summary>
        public string InputImageName1
        {
            get { return _inputImageName1; }
            set
            {
                SetProperty(ref _inputImageName1, value);
                DiffCommand.RaiseCanExecuteChanged();
            }
        }

        private BitmapImage _inputImage1;
        /// <summary>
        /// 入力画像1
        /// </summary>
        public BitmapImage InputImage1
        {
            get { return _inputImage1; }
            set
            {
                SetProperty(ref _inputImage1, value);
                DiffCommand.RaiseCanExecuteChanged();
            }
        }

        private string _inputImageName2;
        /// <summary>
        /// 入力画像2のパス名
        /// </summary>
        public string InputImageName2
        {
            get { return _inputImageName2; }
            set
            {
                SetProperty(ref _inputImageName2, value);
                DiffCommand.RaiseCanExecuteChanged();
            }
        }
        private BitmapImage _inputImage2;
        /// <summary>
        /// 入力画像2
        /// </summary>
        public BitmapImage InputImage2
        {
            get { return _inputImage2; }
            set
            {
                SetProperty(ref _inputImage2, value);
                DiffCommand.RaiseCanExecuteChanged();
            }
        }

        private BitmapSource _diffImage;
        /// <summary>
        /// 差分画像
        /// </summary>
        public BitmapSource DiffImage
        {
            get { return _diffImage; }
            set { SetProperty(ref _diffImage, value); }
        }
        #endregion

        #region Commands
        public DelegateCommand BrowseInputImage1Command { get; private set; }
        /// <summary>
        /// ファイルダイアログから選択した画像を読み込む
        /// </summary>
        private void BrowseInputImage1Execute()
        {
            InputImageName1 = GetOpenFileName();
            if (string.IsNullOrEmpty(InputImageName1)) return;

            InputImage1 = new BitmapImage();
            InputImage1.BeginInit();
            InputImage1.UriSource = new Uri(InputImageName1, UriKind.RelativeOrAbsolute);
            InputImage1.EndInit();
        }

        public DelegateCommand BrowseInputImage2Command { get; private set; }
        /// <summary>
        /// ファイルダイアログから選択した画像を読み込む
        /// </summary>
        private void BrowseInputImage2Execute()
        {
            InputImageName2 = GetOpenFileName();
            if (string.IsNullOrEmpty(InputImageName2)) return;

            InputImage2 = new BitmapImage();
            InputImage2.BeginInit();
            InputImage2.UriSource = new Uri(InputImageName2, UriKind.RelativeOrAbsolute);
            InputImage2.EndInit();
        }

        public DelegateCommand DiffCommand { get; private set; }
        private bool CanDiffExecute()
        {
            if (InputImage1 == null) return false;
            if (InputImage2 == null) return false;
            if (string.IsNullOrEmpty(InputImageName1)) return false;
            if (string.IsNullOrEmpty(InputImageName2)) return false;

            if (InputImage1.Height != InputImage2.Height) return false;
            if (InputImage1.Width != InputImage2.Width) return false;

            return true;
        }

        private void DiffExecute()
        {
            using (Mat image1 = Cv2.ImRead(InputImageName1))
            using (Mat image2 = Cv2.ImRead(InputImageName2))
            using (Mat diffImage = new Mat(new OpenCvSharp.Size(image1.Cols, image1.Rows), MatType.CV_8UC3))
            {
                Cv2.Absdiff(image1, image2, diffImage);
                DiffImage = BitmapSourceConverter.ToBitmapSource(diffImage);
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            this.BrowseInputImage1Command = new DelegateCommand(BrowseInputImage1Execute, () => true);
            this.BrowseInputImage2Command = new DelegateCommand(BrowseInputImage2Execute, () => true);
            this.DiffCommand = new DelegateCommand(DiffExecute, CanDiffExecute);
        }



        private string GetOpenFileName()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "画像ファイル|*.png;*jpg:*gif;*bmp";

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return "";
        }
    }
}
