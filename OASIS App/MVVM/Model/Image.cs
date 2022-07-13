using System;

namespace OASIS_App.MVVM.Model
{
    public class Image
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public Image() { }

        public Image(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            Path = path;
        }

        public Image(string path, string name)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Path = path;
            Name = name;
        }
    }
}
