using AutoGestao.Enumerador;
using System.ComponentModel;

namespace AutoGestao.Extensions
{
    public static class EnumExtension
    {
        public static Dictionary<int, string> GetEnumDictionary<T>()
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T não é do tipo Enum");
            }

            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .ToDictionary(t => (int)(object)t, t => t.GetDescription());
        }

        public static List<T> GetEnumList<T>()
        {
            var array = (T[])Enum.GetValues(typeof(T));
            var list = new List<T>(array);
            return list;
        }

        public static EnumNaoSim ToEnumNaoSim(this bool aValor)
        {
            return aValor ? EnumNaoSim.Sim : EnumNaoSim.Nao;
        }

        public static T ToEnum<T>(this int aValor)
        {
            var xValorEnum = Enum.ToObject(typeof(T), aValor);
            return (T)xValorEnum;
        }

        public static int ToInt(this Enum aValor)
        {
            return Convert.ToInt32(aValor);
        }

        public static string GetDescription<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Description : source.ToString();
        }

        public static string GetCategory<T>(this T source)
        {
            var field = source.GetType().GetField(source.ToString());
            var attributes = (CategoryAttribute[])field.GetCustomAttributes(typeof(CategoryAttribute), false);
            return attributes != null && attributes.Length > 0 ? attributes[0].Category : source.ToString();
        }

    }
}