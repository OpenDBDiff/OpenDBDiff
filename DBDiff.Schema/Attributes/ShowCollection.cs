using System;

namespace DBDiff.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = true)]
    public sealed class ShowItemAttribute:Attribute 
    {
        private string name;
        private string image;
        private Boolean isFullName;

        public ShowItemAttribute(string name)
        {
            this.name = name;
            this.image = "Folder";
        }

        public ShowItemAttribute(string name, string image)
        {
            this.name = name;
            this.image = image;
        }

        public ShowItemAttribute(string name, string image, Boolean isFullName)
        {
            this.name = name;
            this.image = image;
            this.isFullName = isFullName;
        }

        public string Name
        {
            get { return name; }
        }

        public string Image
        {
            get { return image; }
        }

        public Boolean IsFullName
        {
            get { return isFullName; }
        }
    }
}
