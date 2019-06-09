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
        // We only need to set this once
        // This way we can continue to add images to it throughout the use of the page
        public static ImageItemList imgList = new ImageItemList();

        public static ImageItem displayitem;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;

        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // If we already have images in the list, dont go through and add them again
            // Only try to add on navigation when there are no images in the list
            if( imgList.listImageItem.Count == 0 )
            {
                // Goes to known folder 
                StorageFolder cameraRoll = KnownFolders.CameraRoll;

                // Gets the files within that folder
                var images = await cameraRoll.GetFilesAsync();

                // Adds the images from the obtained files to the image item list
                // false means dont save the files, because they are already saved in the camera roll
                AddImages(images);
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


            //pick multiple files (can select multiple images from file selector popup) and store in files
            var files = await picker.PickMultipleFilesAsync();

            // Adds the images from the obtained files to the image item list
            // true means save the files to the cameraRoll
            AddImages(files, true);
        }

        /**
         * The purpose of this function is the take a list of storage files, read the images within it and then add them to the imageItemList
         * This was created so that this function can be used on opening of application and on click of new image.
         */
        private async void AddImages(IReadOnlyList<StorageFile> files, bool saveFiles = false )
        {
            if (files.Count > 0)
            {
                // use the cameraRoll known folder
                StorageFolder cameraRoll = KnownFolders.CameraRoll;

                for (int i = 0; i < files.Count; i++)
                {

                    // only do this if we requested to save the files
                    StorageFile imageFile;
                    if ( saveFiles )
                    {
                        try
                        {
                            // copy the file to the camera roll folder and replace the existing if there is already one of the same file name
                            imageFile = await files[i].CopyAsync(cameraRoll, files[i].Name, NameCollisionOption.ReplaceExisting);
                        } catch ( Exception ex )
                        {
                            // If we try to add an image from the same folder to the cameraRoll it will hit this exception
                            // log the error then continue to the next image
                            Console.WriteLine("Caught exception trying to add file " + files[i].Name + ". " + ex.Message);
                            continue;
                        }
                    }
                    else
                    {
                        imageFile = files[i];
                    }

                    using (IRandomAccessStream filestream = await imageFile.OpenAsync(FileAccessMode.Read))
                    {
                        // https://stackoverflow.com/questions/14883384/displaying-a-picture-stored-in-storage-file-in-a-metroapp
                        // This code helps to display the entire bitmap image in the flipview with original/non-modified dimensions
                        BitmapImage bitmapImage = new BitmapImage();
                        FileRandomAccessStream stream = (FileRandomAccessStream)await imageFile.OpenAsync(FileAccessMode.Read);
                        bitmapImage.SetSource(stream);

                        // https://docs.microsoft.com/en-us/windows/uwp/files/quickstart-getting-file-properties
                        BasicProperties basicProperties = await imageFile.GetBasicPropertiesAsync();
                        imgList.listImageItem.Add(new ImageItem()
                        {
                            ImageData = bitmapImage,
                            ImageName = imageFile.Name,
                            FileType = imageFile.FileType,
                            ImageWidth = bitmapImage.PixelWidth,
                            ImageHeight = bitmapImage.PixelHeight,
                            Size = basicProperties.Size,
                            DateModified = basicProperties.DateModified
                        });
                    }

                }
            }

            // Set the photoalbum source as the image item list (displaying it)
            // Set the item source to null and then set it to the value to force a refresh
            // https://social.msdn.microsoft.com/Forums/sqlserver/en-US/3c95941e-1794-4da1-9459-32c42fc0caf4/wpf-refresh-item-on-datagrid-after-update-on-db?forum=wpf
            PhotoAlbum.ItemsSource = null;
            PhotoAlbum.ItemsSource = imgList.listImageItem;

        }

        private Task<BitmapImage> StorageFileToBitmapImage(StorageFile files)
        {
            throw new NotImplementedException();
        }

    }
}


