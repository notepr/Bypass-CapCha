using Solve_Captcha.Model;
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
using System.Windows.Shapes;

namespace Solve_Captcha
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : Window
    {
        private List<KeyApi> lstKey;
        public Setting()
        {
            InitializeComponent();
            //lvSetting.Items.Add()
        }
        public Setting(List<KeyApi> keys,bool isOntop)
        {
            InitializeComponent();
            this.lstKey = keys;
            //lvSetting.Items.Add()
            lvSetting.ItemsSource = keys;
            lvSetting.Items.Refresh();
            this.Topmost = isOntop;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            KeyApi api = (sender as FrameworkElement).DataContext as KeyApi;
            foreach (KeyApi key in lvSetting.Items)
            {
                if (api.ID != key.ID)
                    key.IsSelect = false;
            }
            lvSetting.Items.Refresh();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCheckAmount_Click(object sender, RoutedEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Parallel.ForEach(this.lstKey, key =>
                {
                    if(key.ID == 1)
                    {
                        key.Amount = AnyCapChaHelper.CheckAmount(key.Key);
                    }
                    else
                    {
                        key.Amount = CapChaHelper.CheckAmount(key.Key);
                    }
                });
                this.Dispatcher.Invoke(() =>
                {
                    lvSetting.Items.Refresh();
                });
            });
        }
    }
}
