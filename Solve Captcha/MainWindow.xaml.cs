using Newtonsoft.Json;
using Solve_Captcha.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using CheckBox = System.Windows.Controls.CheckBox;
using Clipboard = System.Windows.Clipboard;
using Image = System.Drawing.Image;
using MessageBox = System.Windows.Forms.MessageBox;
using Pen = System.Drawing.Pen;
using Size = System.Drawing.Size;

namespace Solve_Captcha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<KeyApi> keys = new List<KeyApi>();
        public int selectX;
        public int selectY;
        public int selectWidth;
        public int selectHeight;
        private bool checkSolver = false, isRun = false;
        private Bitmap bm;
        private BitmapImage bi;
        private string fileName = @"config";
        private CaiDat caiDat = new CaiDat();
        private KeyApi keySelect = new KeyApi();
        private int errorCount = 0;

        //This variable control when you start the right click
        public MainWindow()
        {
            InitializeComponent();
            try
            {
                if (File.Exists(this.fileName))
                {
                    this.caiDat = JsonConvert.DeserializeObject<CaiDat>(File.ReadAllText(this.fileName));
                    foreach (KeyApi key in this.caiDat.keys)
                    {
                        if (key.IsSelect == true)
                        {
                            lblIsSelect.Text = "Mode: " + key.Name;
                            break;
                        }
                    }
                    chkSaveTo.IsChecked = this.caiDat.Clipboard;
                    this.SetKeySelect();
                }
                else
                {
                    this.caiDat.keys = new List<KeyApi>();
                    this.caiDat.keys.Add(new KeyApi() { ID = 1, Name = "Anycapcha", Key = "", IsSelect = true });
                    this.caiDat.keys.Add(new KeyApi() { ID = 2, Name = "2Capcha", Key = "", IsSelect = false });
                    this.Save();
                }
            }
            catch
            {
                this.caiDat.keys = new List<KeyApi>();
                this.caiDat.keys.Add(new KeyApi() { ID = 1, Name = "Anycapcha", Key = "", IsSelect = true });
                this.caiDat.keys.Add(new KeyApi() { ID = 2, Name = "2Capcha", Key = "", IsSelect = false });
                this.Save();
            }
        }
        private void Save()
        {
            try
            {
                File.WriteAllText(this.fileName, JsonConvert.SerializeObject(this.caiDat, Formatting.Indented));
            }
            catch
            {
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            foreach(KeyApi api in this.caiDat.keys)
            {
                api.Amount = "";
            }
            Setting setting = new Setting(this.caiDat.keys, (bool)chkOnTop.IsChecked);
            setting.ShowDialog();
            if (setting.DialogResult == true)
            {
                foreach (KeyApi key in this.caiDat.keys)
                {
                    if (key.IsSelect == true)
                    {
                        lblIsSelect.Text = "Mode: " + key.Name;
                        break;
                    }
                }
                this.errorCount = 0;
                this.SetKeySelect();
                this.Save();
            }
        }
        private void btnSnipping_Click(object sender, RoutedEventArgs e)
        {
            Form2 f2 = new Form2();
            f2.Show();
            f2.RequestClose += delegate ()
            {
                this.selectX = f2.selectX;
                this.selectY = f2.selectY;
                this.selectWidth = f2.selectWidth;
                this.selectHeight = f2.selectHeight;
                bool flag = this.selectWidth > 0 && this.selectHeight > 0;
                if (flag)
                {
                    try
                    {
                        this.bm = new Bitmap(this.selectWidth, this.selectHeight);
                        Graphics graphics = Graphics.FromImage(this.bm);
                        Size blockRegionSize = new Size(this.selectWidth, this.selectHeight);
                        graphics.CopyFromScreen(this.selectX, this.selectY, 0, 0, blockRegionSize);

                        Bitmap bImage = this.bm;
                        System.IO.MemoryStream ms = new MemoryStream();
                        bImage.Save(ms, ImageFormat.Jpeg);
                        byte[] byteImage = ms.ToArray();
                        var SigBase64 = Convert.ToBase64String(byteImage);
                        this.Dispatcher.Invoke(() =>
                        {
                            byte[] binaryData = null;
                            try
                            {
                                binaryData = Convert.FromBase64String(SigBase64);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }

                            this.bi = new BitmapImage();
                            this.bi.BeginInit();
                            this.bi.StreamSource = new MemoryStream(binaryData);
                            this.bi.EndInit();
                            image.Source = this.bi;
                            this.checkSolver = true;
                        });
                    }
                    catch (Exception)
                    {
                    }
                }
            };
        }
        private void SetKeySelect()
        {
            try
            {
                this.keySelect = this.caiDat.keys.FindLast(item => item.IsSelect);
            }
            catch { }
        }
        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (this.isRun)
            {
                this.UpdateNoti("Đang chạy ! Chờ ");
                return;
            }
            this.isRun = true;
            this.ChangerColor(NotiColor.RED);
            if (!this.checkSolver)
            {
                this.MessShow("Chọn Vùng Chụp Trước");
                this.isRun = false;
                return;
            }
            if (String.IsNullOrEmpty(this.keySelect.Key))
            {
                this.MessShow("Hãy điền Key");
                this.isRun = false;
                return;
            }
            if (this.errorCount >= 10)
            {
                this.MessShow("Kiểm tra lại KEY hoặc tiền");
                this.isRun = false;
                return;
            }
            Task.Factory.StartNew(() =>
            {
                this.RunSubmit();
                this.isRun = false;
            });
        }
        private void RunSubmit()
        {
            try
            {
                string base64 = this.CaptureImage();
                if (string.IsNullOrEmpty(base64))
                {
                    this.ChangerColor(NotiColor.YELLOW);
                    this.UpdateNoti("Error Get Image");
                    this.errorCount++;
                    return;
                }
                if (this.keySelect.ID == 1)
                {
                    this.AnserCapByAnyCapCha(this.keySelect.Key, base64);
                }
                else
                {
                    this.AnserCapBy2CapCha(this.keySelect.Key, base64);
                }
            }
            catch
            {
                this.ChangerColor(NotiColor.YELLOW);
                this.UpdateNoti("Lỗi Chạy");
            }
        }
        private void AnserCapBy2CapCha(string key, string base64)
        {
            try
            {
                this.UpdateNoti("Waiting...");
                string IdCap = CapChaHelper.GuiCapCha(key, base64);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (IdCap.Contains("VGDVCHGT-Error"))
                    {
                        this.ChangerColor(NotiColor.YELLOW);
                        this.UpdateNoti("Error Send CapCha");
                        this.errorCount++;
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                int seconds = 7;
                this.UpdateNoti("ID " + IdCap);
                while (seconds > 0)
                {
                    this.UpdateNoti("Wait " + seconds + "s");
                    seconds--;
                    Thread.Sleep(1000);
                }
                string anser = CapChaHelper.NhanKetQua(key, IdCap);
                int dem = 0;
                while (true)
                {
                    if (anser.Contains("VGDVCHGT-Error"))
                    {
                        this.UpdateNoti("Wait More Time");
                        dem++;
                        if (dem >= 5)
                        {
                            this.ChangerColor(NotiColor.YELLOW);
                            this.UpdateNoti("Error Send CapCha");
                            this.errorCount++;
                            return;
                        }
                        seconds = 3;
                        while (seconds > 0)
                        {
                            this.UpdateNoti("Wait " + seconds + "s");
                            seconds--;
                            Thread.Sleep(1000);
                        }
                        anser = CapChaHelper.NhanKetQua(key, IdCap);
                    }
                    else
                    {
                        break;
                    }
                }
                this.DoneCap(anser);
            }
            catch
            {
                this.ChangerColor(NotiColor.YELLOW);
                this.UpdateNoti("Error");
                this.errorCount++;
                return;
            }
        }
        private void AnserCapByAnyCapCha(string key, string base64)
        {
            try
            {
                this.UpdateNoti("Waiting...");
                string IdCap = AnyCapChaHelper.GuiCapCha(key, base64);
                while (true)
                {
                    Thread.Sleep(1000);
                    if (IdCap.Contains("VGDVCHGT-Error"))
                    {
                        this.ChangerColor(NotiColor.YELLOW);
                        this.UpdateNoti("Error Send CapCha");
                        this.errorCount++;
                        return;
                    }
                    else
                    {
                        break;
                    }
                }
                int seconds = 1;
                this.UpdateNoti("ID " + IdCap);
                while (seconds > 0)
                {
                    this.UpdateNoti("Wait " + seconds + "s");
                    seconds--;
                    Thread.Sleep(1000);
                }
                string anser = AnyCapChaHelper.NhanKetQua(key, IdCap);
                int dem = 0;
                while (true)
                {
                    if (anser.Contains("VGDVCHGT-Error"))
                    {
                        this.UpdateNoti("Wait More Time");
                        dem++;
                        if (dem >= 3)
                        {
                            this.ChangerColor(NotiColor.YELLOW);
                            this.UpdateNoti("Error Send CapCha");
                            this.errorCount++;
                            return;
                        }
                        Thread.Sleep(1000);
                        anser = AnyCapChaHelper.NhanKetQua(key, IdCap);
                    }
                    else
                    {
                        break;
                    }
                }
                this.DoneCap(anser);
            }
            catch
            {
                this.ChangerColor(NotiColor.YELLOW);
                this.UpdateNoti("Error");
                this.errorCount++;
                return;
            }
        }
        private void DoneCap(string anser)
        {
            this.Dispatcher.Invoke(() =>
            {
                txtDoneCap.Text = anser;
                this.ChangerColor(NotiColor.GREEN);
                this.errorCount = 0;
                if (this.caiDat.Clipboard)
                {
                    Clipboard.SetText(anser);
                }
                this.UpdateNoti("Done");
            });
        }
        private void ChangerColor(NotiColor notiColor)
        {
            this.Dispatcher.Invoke(() =>
            {
                switch (notiColor)
                {
                    case NotiColor.RED:
                        btnNotiColor.Background = Brushes.Red;
                        break;
                    case NotiColor.GREEN:
                        btnNotiColor.Background = Brushes.Green;
                        break;
                    case NotiColor.YELLOW:
                        btnNotiColor.Background = Brushes.Yellow;
                        break;
                }
            });
        }
        private void UpdateNoti(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                lblNoti.Text = str;
            });
        }
        private void MessShow(string str)
        {
            this.Dispatcher.Invoke(() =>
            {
                MessageBox.Show(str, "Thông báo");
            });
        }
        private string CaptureImage()
        {
            string base64 = "";
            try
            {
                this.bm = new Bitmap(this.selectWidth, this.selectHeight);
                Graphics graphics = Graphics.FromImage(this.bm);
                Size blockRegionSize = new Size(this.selectWidth, this.selectHeight);
                graphics.CopyFromScreen(this.selectX, this.selectY, 0, 0, blockRegionSize);

                Bitmap bImage = this.bm;
                System.IO.MemoryStream ms = new MemoryStream();
                bImage.Save(ms, ImageFormat.Jpeg);
                byte[] byteImage = ms.ToArray();
                base64 = Convert.ToBase64String(byteImage);
                this.Dispatcher.Invoke(() =>
                {
                    byte[] binaryData = null;
                    try
                    {
                        binaryData = Convert.FromBase64String(base64);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                    this.bi = new BitmapImage();
                    this.bi.BeginInit();
                    this.bi.StreamSource = new MemoryStream(binaryData);
                    this.bi.EndInit();
                    image.Source = this.bi;
                });
                //Image demo = (Image)bImage;

                //demo.Save(@"" + 10 + "-number_1.png", System.Drawing.Imaging.ImageFormat.Png);
            }
            catch (Exception)
            {

            }
            return base64;
        }

        private void chkSaveTo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox check = sender as CheckBox;
                this.caiDat.Clipboard = (bool)check.IsChecked;
                this.Save();
            }
            catch
            {

            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(txtDoneCap.Text);
        }

        private void chkOnTop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox check = sender as CheckBox;
                this.Topmost = (bool)check.IsChecked;
            }
            catch
            {

            }
        }
        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
        private void chkGreen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox check = sender as CheckBox;
                switch (check.Name)
                {
                    case "chkGreen":
                        this.ChangerColor(NotiColor.GREEN);
                        chkYellow.IsChecked = false;
                        break;
                    case "chkYellow":
                        this.ChangerColor(NotiColor.YELLOW);
                        chkGreen.IsChecked = false;
                        break;
                }
            }
            catch
            {

            }
        }
    }
}
