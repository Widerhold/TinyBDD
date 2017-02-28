using System;

namespace TinyBDD
{
    public class StoryAttribute : Attribute
    {
        public string Title { get; set; }

        public string As { get; set; }

        public string IWant { get; set; }

        public string SoThat { get; set; }

        public StoryAttribute(string title)
        {
            Title = title;
        }
    }
}
