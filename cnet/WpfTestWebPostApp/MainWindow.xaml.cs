using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
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

namespace WpfTestWebPostApp
{
    public class MediaParam
    {
        string Key;// 於API037回應中取得WK欄位值
        string Ccid;// 於API037回應中取得WC欄位值
        string OBUid;// 授權碼，於API002取得DD欄位值
        string UserAccount;//     授權碼，於API002取得DD欄位值
        string MsgTypeID;//   訊息種類，填入15
        string D_id;//    於API019取得TD欄位值
        public string FileName;//    多媒體名稱，英數字無特殊符號
        string FileUploadType;//  回傳多媒體種類，填入02
        public MediaParam()
        {
            Key = "Keydata";
            Ccid = "Cciddata";
            OBUid = "OBUiddata";
            UserAccount = "UserAccountdata";
            MsgTypeID = "15";
            D_id = "D_iddata";
            FileUploadType = "02";
        }
        public void AddWebHeader(HttpWebRequest wr)
        {
            wr.Headers.Add("Key", Key);
            wr.Headers.Add("Ccid", Ccid);
            wr.Headers.Add("OBUid", OBUid);
            wr.Headers.Add("UserAccount", UserAccount);
            wr.Headers.Add("MsgTypeID", MsgTypeID);
            wr.Headers.Add("D_id", D_id);
            wr.Headers.Add("FileName", FileName);
            wr.Headers.Add("FileUploadType", FileUploadType);
        }
    }
    public class OptionMediaParam
    {
        string Lon;// 多媒體檔案拍攝經度, ex:121.548415(小數點後六位)
        string Lat;//     多媒體檔案拍攝緯度, ex:24.548415(小數點後六位)
        string FileTime;//   多媒體檔案拍攝時間, ex:2011/12/16 15:00:00 
        string FileInfo; //多媒體檔案備註說明
        public OptionMediaParam()
        {
            Lon = "121.548415";
            FileInfo = "just test";
        }
        public void AddWebHeader(HttpWebRequest wr)
        {
            if (!string.IsNullOrEmpty(Lon))
            {
                wr.Headers.Add("Lon", Lon);
            }
            if (!string.IsNullOrEmpty(Lat))
            {
                wr.Headers.Add("Lat", Lat);
            }
            if (!string.IsNullOrEmpty(FileTime))
            {
                wr.Headers.Add("FileTime", FileTime);
            }
            if (!string.IsNullOrEmpty(FileInfo))
            {
                wr.Headers.Add("FileInfo", FileInfo);
            }
        }

    }
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileUpload_Host;
        string Port;
        string filename;
        MediaParam mediaParam = new MediaParam();
        OptionMediaParam optionMediaParam = new OptionMediaParam();
        public MainWindow()
        {
            InitializeComponent();
            InitUrlData();
        }

        private void InitUrlData()
        {
            FileUpload_Host = tbHost.Text;
            Port = tbPort.Text;
            string url = $"http://{FileUpload_Host}:{Port}/FMS_WS/Services/API/FileUpload/";
            tbUrl.Text = url;
            //Console.WriteLine(url);

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitUrlData();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filename = openFileDialog.FileName;
                Console.WriteLine(filename);
                //MediaParam mediaParam = new MediaParam();
                mediaParam.FileName = System.IO.Path.GetFileNameWithoutExtension(filename);

                Console.WriteLine(mediaParam.FileName);
                //增加其他參數
                NameValueCollection nvc = new NameValueCollection();
                // 上傳檔案
                var result = UploadFileByHttpWebRequest(tbUrl.Text,
                    filename, "imgUploader", "image/jpeg", nvc);

                //回傳結果
                Console.WriteLine(result);

            }
        }
        /// <summary>
        /// 上傳檔案至 Server 透過HttpWebRequest
        /// </summary>
        /// <param name="url">上傳網址</param>
        /// <param name="file">檔案位置</param>
        /// <param name="paramName">該control name</param>
        /// <param name="contentType">image type </param>
        /// <param name="nvc">其他參數</param>
        /// <returns></returns>
        public string UploadFileByHttpWebRequest(string url, string file, string paramName, string contentType, NameValueCollection nvc)
        {

            // Debug.WriteLine(string.Format("上傳至 {0} to {1}", file, url));

            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            //head add
            mediaParam.AddWebHeader(wr);
            optionMediaParam.AddWebHeader(wr);
            //wr.Headers.Add("Cache-Control", "max-age=0");
            //wr.Headers.Add("Key", "Keydata");


            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;


            Stream rs = wr.GetRequestStream();

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, paramName, file, contentType);
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4096];
            int bytesRead = 0;
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                rs.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            var result = "";
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                //成功回傳結果
                result = reader2.ReadToEnd();

                //File.WriteAllText(Server.MapPath("log.txt"), string.Format("Server 回應{0}", reader2.ReadToEnd()));
            }
            catch (Exception ex)
            {

                // File.WriteAllText(Server.MapPath("log.txt"), string.Format("上傳失敗，錯誤訊息: {0}", ex.Message));
                if (wresp != null)
                {
                    wresp.Close();

                }

            }

            wr = null;
            return result;

        }
    }
}
