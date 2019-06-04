using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Text;

using Windows.Storage.FileProperties;

namespace PictureApp
{

    public class ImageItem
    {
        public BitmapImage ImageData { get; set; }
        public string FileName { get; set; }
        public StorageFile FileRef { get; set; }
    }

    class ImageClickedEventArgs
    {
        public ImageList ImagesList { get; set; }
        public ImageItem SelectedItem { get; set; }
    }

    class ImageList
    {
        public ImageList()
        {
            Images = new List<ImageItem>();
        }

        public List<ImageItem> Images;

        public void Add(ImageItem imageItem)
        {
            Images.Add(imageItem);
        }
    }
}
