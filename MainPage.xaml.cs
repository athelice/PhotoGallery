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
using System.Threading.Tasks;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 


    
 public class ImageItem
        {
            public BitmapImage ImageData { get; set; }
            public string ImageName { get; set; }
            public ulong Size { get; set; }
            public DateTimeOffset DateModified { get; set; }

            public string FileType { get; set; }
            public int ImageHeight { get; set; }
            public int ImageWidth { get; set; }
    }

    public class ImageItemList
    {
        public List<ImageItem> listImageItem { get; set; }
        public ImageItemList()
        {
            listImageItem = new List<ImageItem>();
        }
    }

    public sealed partial class MainPage : Page

    {
        public static ImageItemList imgList;
        public static ImageItem displayitem;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (imgList != null)
            {
                PhotoAlbum.ItemsSource = imgList.listImageItem;
            }
        }
        public void PhotoAlbum_ItemClick(object sender, RoutedEventArgs e)
        {
            ItemClickEventArgs args = e as ItemClickEventArgs;
            displayitem = args.ClickedItem as ImageItem;
            this.Frame.Navigate(typeof(ImagePage), PhotoAlbum.ItemsSource);
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

            imgList = new ImageItemList();
            var items = PhotoAlbum.ItemsSource;

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
                        
                        // https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-getting-file-properties
                        BasicProperties basicProperties = await files[i].GetBasicPropertiesAsync();
                        imgList.listImageItem.Add(new ImageItem() { ImageData = bitmapImage,
                            ImageName = files[i].Name,
                            FileType = files[i].FileType,
                            ImageWidth = bitmapImage.PixelWidth,
                            ImageHeight = bitmapImage.PixelHeight,
                            Size = basicProperties.Size,
                            DateModified = basicProperties.DateModified
                    });


                    }


                }
                PhotoAlbum.ItemsSource = imgList.listImageItem;
            }
        }

        private Task<BitmapImage> StorageFileToBitmapImage(StorageFile files)
        {
            throw new NotImplementedException();
        }

    }
}


