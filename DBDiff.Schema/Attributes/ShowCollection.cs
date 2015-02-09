using System;

namespace DBDiff.Schema.Attributes
{
    [AttributeUsage(AttributeTargets.Property,AllowMultiple = true)]
    public sealed class ShowItemAttribute:Attribute 
    {
        public ShowItemAttribute(string name)
        {
            this.Name = name;
            this.Image = "Folder";
        }

        public ShowItemAttribute(string name, string image)
        {
            this.Name = name;
            this.Image = image;
        }

        public ShowItemAttribute(string name, string image, Boolean isFullName)
        {
            this.Name = name;
            this.Image = image;
            this.IsFullName = isFullName;
        }

        public string Name { get; private set; }

        public string Image { get; private set; }

        public Boolean IsFullName { get; private set; }
    }
}
