using Domain.Enums;

namespace Application.Utils
{
    public static class MenuItemTypeTranslator
    {
        public static string ToFriendlyString(MenuItemType type)
        {
            return type switch
            {
                MenuItemType.Main => "Sanduíche",
                MenuItemType.Side => "Acompanhamento",
                MenuItemType.Drink => "Bebida",
                _ => "Desconhecido"
            };
        }
    }
}
