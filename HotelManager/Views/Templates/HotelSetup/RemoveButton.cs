using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HotelManager.Views.Templates.HotelSetup
{
    public class RemoveButton : Button
    {
        private readonly string[] toolTipParams =
        {
            "сгради",
            "етажи",
            "стаи",
            "резервации"
        };

        public RemoveButton(int id, RemoveType removeType)
        {
            HorizontalAlignment = HorizontalAlignment.Right;
            Content = " - ";
            Name = $"BId{id}";
            ToolTip = $"Могат да се изтриват само {toolTipParams[(int)removeType]}, които нямат {toolTipParams[(int)removeType + 1]}.";
            FontSize = 18;
            FontWeight = FontWeights.ExtraBold;
            Padding = new Thickness(2);
            Background = new SolidColorBrush(Colors.OrangeRed);
        }
    }
}