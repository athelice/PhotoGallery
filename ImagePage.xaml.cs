using System;
using System.Collections.Generic;
using System.Diagnostics;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace PictureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ImagePage : Page
    {
        public ImagePage()
        {
            this.InitializeComponent();
        }

        private void MyFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            List<ImageItem> list1 = e.Parameter as List<ImageItem>;
            // ImageItem displayitem = e.Parameter as ImageItem;
           
            MyFlipView1.ItemsSource = list1;
            //var filedetails = MyFlipView1.ItemsSource;
            ImageItem imgItem = MainPage.displayitem;
   
           
         
            foreach (ImageItem i in list1)
            {

                while (imgItem.ImageName.Equals(i.ImageName))
                {
                    MyFlipView1.SelectedItem = i;
                    DisplayImageName.Text = "Name: " + i.ImageName;
                    DisplayFileType.Text = "Type: " + i.FileType;
                    DisplayFileSize.Text = "Size: " + i.Size.ToString();
                    DisplayDateModified.Text = "Date Modified: " + i.DateModified.ToString();
                    break;
                }

            }

            base.OnNavigatedTo(e);

        }
        public void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

      
    }
}
