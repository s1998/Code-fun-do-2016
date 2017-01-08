using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Net;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Windows.Storage;
using System.Threading.Tasks;
using System.Windows;
using Windows.UI.Popups;
using Newtonsoft.Json;
using Json.NET;
using System.Text;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FaceFinder
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        string[] selectedImagePath;
        int noOfSelectedImages = 0;
        //bool b1_clicked = false;
        //bool b2_clicked = false;
        bool face_matched = false;

        answer a;
        public class Product
        {
            public string faceId1;
            public string faceId2;
        }

        public class answer
        {
            public Boolean isIdentical;
            public double confidence;
        }

        Face[] faceRects;
        Face[] faceRects2;
        Face f1, f2;
        string id1, id2;
        StorageFile mainFile;
        StorageFile[] selectedFiles;

        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("dc21903bc5c049ce98f416bbf99ef048");

        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath,StorageFile f)
        {
            await Task.Delay(3001);

            try
            {
                using (var s = await f.OpenReadAsync())
                {
                    using (Stream imageFileStream = s.AsStreamForRead() )
                    {
                        var faces = await faceServiceClient.DetectAsync(imageFileStream);
                        return faces.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                var dialog = new MessageDialog("The app can't be connected to intenet. Please check your settings .");
                await dialog.ShowAsync();
                return new Face[0];
            }
        }



        private async void b1_click(object sender, RoutedEventArgs e)
        {
            b2.IsEnabled = false;
            b2.Visibility = Visibility.Collapsed;
            b3.IsEnabled = false;
            b3.Visibility = Visibility.Collapsed;

            textBlock.Text = "Select main image. ";


            //var openDlg = new Microsoft.Win32.OpenFileDialog();

            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
                mainFile = file;
                this.textBlock.Text = "Picked photo: " + file.Name;
            }
            else
            {
                this.textBlock.Text = "Operation cancelled.";
            }
           
           
            this.textBlock.Text = "Checking the main image..... ";
            faceRects = await UploadAndDetectFaces(file.Path, file);
            this.textBlock.Text = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

            if (faceRects.Length == 1)
            {
                textBlock.Text = "Now select another set of images. ";
                f1 = faceRects[0];
                id1 = f1.FaceId.ToString();
            }
            else
            {
                textBlock.Text = "Selected image is invalid. Please select an image with only one person in it.";
                return;
            }
            //b1_clicked = true;
            textBlock.Text = "Now select another set of images.";


            b2.IsEnabled = true;
            b2.Visibility = Visibility.Visible;
        }

        private async void b2_click(object sender, RoutedEventArgs e)
        {
            b3.IsEnabled = false;
            b3.Visibility = Visibility.Collapsed;
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
          
            picker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");

            var tempFiles = await picker.PickMultipleFilesAsync();
            selectedFiles = new StorageFile[tempFiles.Count()];
            selectedImagePath = new string[tempFiles.Count()];

            noOfSelectedImages = tempFiles.Count();

            int i = 0;
            foreach (StorageFile f in tempFiles)
            {
                selectedFiles[i] = f;
                selectedImagePath[i] = f.Path;
                i++;
            }

            if (noOfSelectedImages == 0)
            {

                textBlock.Text = "Please select at least one image.";
                return;

            }

            this.textBlock.Text = "A total of " + noOfSelectedImages.ToString() + " have been selected. Click on Match button to get all the images that match with the given face. ";
            b3.IsEnabled = true;
            b3.Visibility = Visibility.Visible;

        }

        private async void b3_click(object sender, RoutedEventArgs e)
        {
            int k = 0;

            string s = DateTime.Now.ToString("hhmm");
            string newp = "Result";
            string newPathDirectory = newp + s;

            await KnownFolders.PicturesLibrary.CreateFolderAsync(newPathDirectory);
            StorageFolder temp = await KnownFolders.PicturesLibrary.GetFolderAsync(newPathDirectory);
            //StorageFolder temp = KnownFolders.PicturesLibrary;



            BitmapImage img = new BitmapImage();
            img = await LoadImage(mainFile);
            image.Source = img;



            for (int i = 0; i < selectedImagePath.Count(); i++)
            {
                string filePath = selectedImagePath[i];
                
                faceRects2 = await UploadAndDetectFaces(selectedImagePath[i],selectedFiles[i]);

                BitmapImage img2 = new BitmapImage();
                img = await LoadImage(selectedFiles[i]);
                image1.Source = img;

                textBlock.Text = "Performing operation for image " + (i + 1).ToString() + "/" + noOfSelectedImages.ToString() + Environment.NewLine + "Main image : " + mainFile.Name + Environment.NewLine + "Comparing with : " + selectedFiles[i].Name;

                for (int j = 0; j < faceRects2.Length; j++)
                {
                    face_matched = false;

                    f2 = faceRects2[j];
                    id2 = f2.FaceId.ToString();

                    Product p = new Product();
                    p.faceId1 = id1;
                    p.faceId2 = id2;

                    string output = JsonConvert.SerializeObject(p);

                    //textBlock.Text = output;

                    await MakeRequest(output);

                    //await MakeRequest(output);
                    if (face_matched)
                    {
                        TransferToStorage(selectedFiles[i], temp, selectedImagePath[i]);
                        k++;
                        break;
                    }

                    
                }

                textBlock.Text = "No of matched images are :" + k.ToString() + Environment.NewLine + "Operation complete. All files have been saved in resultImages folder in the directory of selected images." + Environment.NewLine+" Click on close to exit. " + Environment.NewLine + "Images with positive result have been saved in the folder : " + temp.Name+" inside Pictures folder of the user. ";

            }
            
        }

        private async void TransferToStorage (StorageFile f , StorageFolder fo, string p)
        {
            // Has the file been copied already?
            try
            {
                await ApplicationData.Current.LocalFolder.GetFileAsync(f.Name);
                // No exception means it exists
                return;
            }
            catch (System.IO.FileNotFoundException)
            {
                // The file obviously doesn't exist

            }
            // Cant await inside catch, but this works anyway
            //StorageFile stopfile = await StorageFile.GetFileFromPathAsync(p);
            await f.CopyAsync(fo);
        }

        private void b4_click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        public async Task MakeRequest(string s)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.projectoxford.ai/face/v1.0/verify");
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Ocp-Apim-Subscription-Key"] =  "dc21903bc5c049ce98f416bbf99ef048";
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] data = encoding.GetBytes(s);



            using (Stream temp = await request.GetRequestStreamAsync())
            {
                temp.Write(data, 0, data.Length);
            }

            string output;
            using (WebResponse response = await request.GetResponseAsync())
            {
                using (Stream stream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        var receivedData = sr.ReadToEnd();
                        output = receivedData.ToString();
                    }
                }
            }

            a = JsonConvert.DeserializeObject<answer>(output);

            if (a.isIdentical)
                face_matched = true;

            await Task.Delay(3001);
        }

        private static async Task<BitmapImage> LoadImage(StorageFile file)
        {
            BitmapImage bitmapImage = new BitmapImage();
            FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

            bitmapImage.SetSource(stream);

            return bitmapImage;

        }
    }
}
