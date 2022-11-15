using System.Text.RegularExpressions;

namespace MVCAbelStoreWeb
{

    public enum Roles
    {
        Administrators, ProductManagers, OrderManager, Members
    }
    public static class AppExtentions
    {
        public static string ToSafeUrlString(this string text) => Regex.Replace(string.Concat(text.Where(p => char.IsLetterOrDigit(p) || char.IsWhiteSpace(p))), @"\s+", "-").ToLower();
    }


}
