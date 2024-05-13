using Un4seen.Bass.AddOn.Tags;

namespace Radio
{
    internal class TagInfo(TAG_INFO tagInfo)
    {
        private string _artist = tagInfo.artist;
        private string _title = tagInfo.title;

        public bool Update(TAG_INFO tagInfo)
        {
            if (_artist == tagInfo.artist && _title == tagInfo.title)
            {
                return false;
            }

            _artist = tagInfo.artist;
            _title = tagInfo.title;

            return true;
        }
    }

    internal static class TAG_INFO_Extensions
    {
        public static void Display(this TAG_INFO tagInfo)
        {
            if (string.IsNullOrEmpty(tagInfo.artist))
            {
                Console.Title = tagInfo.title;
            }
            else
            {
                Console.Title = $"{tagInfo.artist} / {tagInfo.title}";
                Console.WriteLine($"Artist: {tagInfo.artist}");
            }

            Console.WriteLine($"Title: {tagInfo.title}");
        }
    }
}
