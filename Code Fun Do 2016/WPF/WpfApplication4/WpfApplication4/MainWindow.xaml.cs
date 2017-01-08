using System;
using System.Net;
using System.Web;
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
using Json.NET;
using Newtonsoft.Json;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ComponentModel;
using System.Data;
using System.Drawing;


namespace WpfApplication4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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


        private readonly IFaceServiceClient faceServiceClient = new FaceServiceClient("dc21903bc5c049ce98f416bbf99ef048");

        private async Task<Face[]> UploadAndDetectFaces(string imageFilePath)
        {

            try
            {
                using (Stream imageFileStream = File.OpenRead(imageFilePath))
                {
                    var faces = await faceServiceClient.DetectAsync(imageFileStream);
                    //var faceRects = faces.Select(face => face.FaceRectangle);

                    System.Threading.Thread.Sleep(3005);
                    return faces.ToArray();
                }
            }
            catch (Exception)
            {
                //Title = "Connect to Internet";
                MessageBox.Show("There is no internet connection. Please connect to internet .");

                System.Threading.Thread.Sleep(3005);
                return new Face[0];
            }
        }

        private async void b1_click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Select main image. ";
            b2.IsEnabled = false;
            b2.Visibility = Visibility.Hidden;
            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? result = openDlg.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            string filePath = openDlg.FileName;

             Uri fileUri = new Uri(filePath);
             BitmapImage bitmapSource = new BitmapImage();

             bitmapSource.BeginInit();
             bitmapSource.CacheOption = BitmapCacheOption.None;
             bitmapSource.UriSource = fileUri;
             bitmapSource.EndInit();

             Photo1.Source = bitmapSource;


             /*Title = "Detecting...";*/
            textBlock.Text = "Checking the main image..... ";
            faceRects = await UploadAndDetectFaces(filePath);
            textBlock.Text = String.Format("Detection Finished. {0} face(s) detected", faceRects.Length);

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

            // Title = filePath;        
        }

        private async void b2_click(object sender, RoutedEventArgs e)
        {
            textBlock.Text = "Select another set of images";
            b3.IsEnabled = false;
            b3.Visibility = Visibility.Hidden;

            var openDlg = new Microsoft.Win32.OpenFileDialog();

            openDlg.Multiselect = true;
            openDlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";
            bool? result = openDlg.ShowDialog(this);

            if (!(bool)result)
            {
                return;
            }

            int i = 0;

            selectedImagePath = new string[openDlg.FileNames.Count()];

            foreach (string file in openDlg.FileNames)
            {
                selectedImagePath[i] = file;
                i++;
            }

            noOfSelectedImages = openDlg.FileNames.Count();

            // string filePath = openDlg.FileName;

            /* Uri fileUri = new Uri(filePath);
             BitmapImage bitmapSource = new BitmapImage();

             bitmapSource.BeginInit();
             bitmapSource.CacheOption = BitmapCacheOption.None;
             bitmapSource.UriSource = fileUri;
             bitmapSource.EndInit();


             Title = "Detecting...";
             faceRects2 = await UploadAndDetectFaces(filePath);
             Title = "Detection Finished";
             f2 = faceRects2[0];
             */
             /*Product p1 = new Product();
             p1.name = "Amit";

             string output = JsonConvert.SerializeObject(p1);
             Title = output;*/
            /*
            id2 = f2.FaceId.ToString();
            Title = id2;*/
            //b2_clicked = true;


            if(noOfSelectedImages==0)
            {
                
                textBlock.Text = "Please select at least one image.";
                return;

            }

            textBlock.Text = "A total of " + noOfSelectedImages.ToString() +" have been selected. Click on Match button to get all the images that match with the given face. ";
            b3.IsEnabled = true;
            b3.Visibility = Visibility.Visible;

        
    }

        private void b4_click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void b3_click(object sender, RoutedEventArgs e)
        {
            //Title = "Performing the operation now";
            

            int k = 0;
            for (int i = 0; i < selectedImagePath.Count(); i++)
            {
                textBlock.Text = "Performing operation for image " + (i+1).ToString() + "/"+noOfSelectedImages.ToString();
                string filePath = selectedImagePath[i];

                Uri fileUri = new Uri(filePath);
                BitmapImage bitmapSource = new BitmapImage();

                bitmapSource.BeginInit();
                bitmapSource.CacheOption = BitmapCacheOption.None;
                bitmapSource.UriSource = fileUri;
                bitmapSource.EndInit();
                
                Photo2.Source = bitmapSource;
                

                //Title = "Detecting...";
                faceRects2 = await UploadAndDetectFaces(filePath);
                //Title = String.Format("Detection Finished. {0} face(s) detected 2nd image", faceRects2.Length);
                //f2 = faceRects2[0];

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
                    if (face_matched)
                    {
        
                        k++;
                       //Title = string.Format("Image {0} matched",(i+1).ToString()); 
                       
                        string directoryPath = System.IO.Path.GetDirectoryName(filePath);
                        string newDirectoryPath = directoryPath + "\\resultImages";

                        if (Directory.Exists(newDirectoryPath) == false)
                            Directory.CreateDirectory(newDirectoryPath);
                        string s = DateTime.Now.ToString("hhmm");
                        File.Copy(filePath,
                            System.IO.Path.Combine(newDirectoryPath, System.IO.Path.GetFileNameWithoutExtension(filePath) +"_"+s+"_" + k.ToString() + System.IO.Path.GetExtension(filePath)));

                        break;
                    }

                    /*else
                    {
                        Title = string.Format("Image {0}  not matched", (i + 1).ToString());
                    }*/

                }
                
                textBlock.Text = "No of matched images are :" + k.ToString() + Environment.NewLine + "Operation complete. All files have been saved in resultImages folder in the directory of selected images. Click on close to exit. "; 
               
            }
            /*Product p1 = new Product();
            p1.name = "Amit";

            string output = JsonConvert.SerializeObject(p1);
            Title = output;*/
            /*
            id2 = f2.FaceId.ToString();
            Title = id2;
            }

           /* Product p = new Product();
            p.faceId1 = id1;
            p.faceId2 = id2;
            //p.personGroupId = "Random";
            string output = JsonConvert.SerializeObject(p);
            textBlock.Text = output;
           await MakeRequest(output);
            if(a.isIdentical)
            {
                Title = "Matched";
            }
            else
            {
                Title = "Different";
            }*/

            Title = "Task Completed";
        }


        public async Task MakeRequest(string s)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.projectoxford.ai/face/v1.0/verify");
            request.Method = "POST";
            request.ContentType = "application/json";
            //request.Headers.Add("Content-Type", "application/json");
            request.Headers.Add("Ocp-Apim-Subscription-Key", "dc21903bc5c049ce98f416bbf99ef048");


            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {

                streamWriter.Write(s);
                streamWriter.Flush();
            }

            var response = (HttpWebResponse)request.GetResponse();
            string output;
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var receivedData = streamReader.ReadToEnd();
                output = receivedData.ToString();
            }

            //answer a;

            a = JsonConvert.DeserializeObject<answer>(output);
            if (a.isIdentical)
                face_matched = true;

            System.Threading.Thread.Sleep(3005);
        }
    }
}
