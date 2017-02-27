using System;

namespace TinyBDD
{
    public class FeatureAttribute : Attribute
    {
        public string Title { get; set; }

        public string AsA { get; set; }

        public string IWant { get; set; }

        public string SoThat { get; set; }

        public FeatureAttribute(string title)
        {
            Title = title;
        }
    }
}
