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

        // FLIPVIEW
        // EventArgs e is a parameter called e that contains the event data, see the EventArgs MSDN page for more information.
        // Object Sender is a parameter called Sender that contains a reference to the control/object that raised the event
        // E.g. When Button is clicked, the btn_Click event handler will be fired.The "object sender" portion will be a reference to the button which was clicked
        private void MyFlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get the current flip view
            FlipView fv = ((FlipView)sender);

            // If the current flipview doesnt have a selected item just dont do anything
            if( fv.SelectedItem == null )
            {
                return;
            }

            // When a selection is changed we need to change the displayed image discriptions as well
            ImageItem selectedImage = fv.SelectedItem as ImageItem;
            DisplayImageName.Text = "Name: " + selectedImage.ImageName;
            DisplayFileType.Text = "Type: " + selectedImage.FileType;
            DisplayFileSize.Text = "Size: " + selectedImage.Size.ToString() + " bytes";
            DisplayDateModified.Text = "Date Modified: " + selectedImage.DateModified.ToString();

        }

       

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
 
            List<ImageItem> list1 = e.Parameter as List<ImageItem>;
           
            // List of images selected
            MyFlipView1.ItemsSource = list1;

            // Sets the current display text
            ImageItem imgItem = MainPage.displayitem;
            DisplayImageName.Text = "Name: " + imgItem.ImageName;
            DisplayFileType.Text = "Type: " + imgItem.FileType;
            DisplayFileSize.Text = "Size: " + imgItem.Size.ToString() + "bytes";
            DisplayDateModified.Text = "Date Modified: " + imgItem.DateModified.ToString();


            // Setting the currently selected item in the flipview to the MainPage.displayitem
            // This is so when the user clicks on the displayed image in the mainpage, 
            // it will select that image on the flip view in the image page
            foreach (ImageItem i in list1)
            {
                if (imgItem.ImageName.Equals(i.ImageName))
                {
                    MyFlipView1.SelectedItem = i;
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
