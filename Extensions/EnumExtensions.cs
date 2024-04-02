using Microsoft.Extensions.Localization;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace TestClientsRequests.Extensions
{
    public static class EnumExtensions
    {
        public static string GetLocalizedValue<T>(this T enumValue, IStringLocalizer localizer) where T : Enum
        {
            var type = typeof(T);
            var member = type.GetMember(enumValue.ToString());
            var key = member[0].GetCustomAttributes<DisplayAttribute>(false).FirstOrDefault()?.Name ?? enumValue.ToString();
            return localizer[key];
        }
    }
}
