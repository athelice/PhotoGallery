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
using System.ComponentModel;
using System.Threading.Tasks;
using System.Security;
using System.Security.Permissions;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace PictureApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ImageList Images { get; set; }

        private String SavePath { get; set; }

        public MainPage()
        {
            Images = new ImageList();
            this.InitializeComponent();

            LoadFromDisk();
        }

        public async Task<bool> SaveToDiskAsync()
        {
            var workingDirectory = ApplicationData.Current.LocalFolder;
            var file = await workingDirectory.CreateFileAsync("saveddata.dat", CreationCollisionOption.ReplaceExisting);

            var textStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            var textWriter = new DataWriter(textStream);

            foreach (ImageItem image in Images.Images)
            {
                if (image.FileRef.IsAvailable)
                {
                    textWriter.WriteUInt16((ushort)image.FileRef.Path.Length);
                    textWriter.WriteString(image.FileRef.Path);
                }
            }

            await textWriter.StoreAsync();
            return await textStream.FlushAsync();
        }

        public async void LoadFromDisk()
        {
            try
            {
                var workingDirectory = ApplicationData.Current.LocalFolder;
                var file = await workingDirectory.GetFileAsync("saveddata.dat");


                var textStream = await file.OpenAsync(FileAccessMode.Read);
                var textReader = new DataReader(textStream);

                var fileLength = textStream.Size;

                Images = new ImageList();

               var bytesLoaded = await textReader.LoadAsync((uint)fileLength);

                while (true)
                {
                    ushort FilePathLength = textReader.ReadUInt16();
                    var FilePath = textReader.ReadString(FilePathLength);

                    var ConvertedPath = FilePath.Replace("\\", "/");

                    // This fails everytime and I don't know why. The directory is valid though!                    
                    if (File.Exists(ConvertedPath))
                    {
                        var storageFile = await StorageFile.GetFileFromPathAsync(ConvertedPath);

                        await AddImageFromFileReference(storageFile);
                    }
                }
            }
            catch (FileNotFoundException Ex)
            {
                // First run this will always trigger
            }
            catch (EndOfStreamException Ex)
            {
                // No more to read
            }
            catch (Exception e)
            {
                //TODO
            }
        }

        private async Task AddImageFromFileReference(StorageFile fileRef)
        {
            Windows.Storage.AccessCache.StorageApplicationPermissions.FutureAccessList.Add(fileRef);

            IRandomAccessStream filestream = await fileRef.OpenAsync(FileAccessMode.Read, StorageOpenOptions.AllowReadersAndWriters);

            ThumbnailMode thumbnailMode = ThumbnailMode.PicturesView;
            ThumbnailOptions thumbnailOptions = ThumbnailOptions.UseCurrentScale;
            uint resize = 400;
            BitmapImage bitmapImage = new BitmapImage();
            StorageItemThumbnail thumb = await fileRef.GetThumbnailAsync(thumbnailMode, resize, thumbnailOptions);
            await bitmapImage.SetSourceAsync(filestream);
            bitmapImage.SetSource(thumb);

            ImageItem NewItem = new ImageItem() { ImageData = bitmapImage, FileRef = fileRef, FileName = fileRef.Name };

            filestream.Dispose();

            Images.Add(NewItem);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null && e.Parameter.GetType() == typeof(ImageList))
            {
                Images = e.Parameter as ImageList;
            }

            base.OnNavigatedTo(e);

            //PhotoAlbum.ItemsSource = Images.Images;
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
   
            StringBuilder output = new StringBuilder("Picked Files: \n");
            if (files.Count > 0)
            {
                for (int i = 0; i < files.Count; i++)
                {
                    await AddImageFromFileReference(files[i]);
                }

                PhotoAlbum.ItemsSource = Images.Images;

                await SaveToDiskAsync();
            }
        }

        public void PhotoAlbum_ItemClick(object sender, ItemClickEventArgs e)
        {
            ImageClickedEventArgs Args = new ImageClickedEventArgs() { ImagesList = Images, SelectedItem = e.ClickedItem as ImageItem };
            Frame.Navigate(typeof(ImagePage), Args);
        }

    }
}


