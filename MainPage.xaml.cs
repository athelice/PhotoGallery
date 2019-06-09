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
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;

using Windows.Storage.FileProperties;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    
 public class ImageItem
        {
            public BitmapImage ImageData { get; set; }
            public string ImageName { get; set; }
}

public sealed partial class MainPage : Page

    {
       
        public MainPage()
        {
            this.InitializeComponent();
        }
       
        public async void AddImage_Click(object sender, RoutedEventArgs e)
        {
            //trigger dialogue box to enable the user to select an image

            var picker = new FileOpenPicker();

            //WHen the Browse dialog opens show files in List mode
            picker.ViewMode = PickerViewMode.List;

            //Start location for browsing
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");


            //pick multiple files and store in files

            var files = await picker.PickMultipleFilesAsync();
            List<ImageItem> ImageList = new List<ImageItem>();
           
            StringBuilder output = new StringBuilder("Picked Files: \n");
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    using (IRandomAccessStream filestream = await files[i].OpenAsync(FileAccessMode.Read))
                    {
                        ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
                        ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
                        uint resize = 400;
                        BitmapImage bitmapImage = new BitmapImage();
                        StorageItemThumbnail thumb = await files[i].GetThumbnailAsync(thumbnailMode, resize, thumbnailOptions);
                        await bitmapImage.SetSourceAsync(filestream);
                        bitmapImage.SetSource(thumb);
                        ImageList.Add(new ImageItem() { ImageData = bitmapImage, ImageName = files[i].Name });

                    }


                }
                PhotoAlbum.ItemsSource = ImageList;

            }
        }

        public void PhotoAlbum_ItemClick(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(ImagePage),e.ClickedItem);
        }

    }
}


