namespace CloudAppBrowser.Core
{
    public static class StringUtils
    {
        public static string Abbreviate(this string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }
            if (value.Length <= maxLength)
            {
                return value;
            }
            return value.Substring(0, maxLength - 3) + "...";
        }
    }
}