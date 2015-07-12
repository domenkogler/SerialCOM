using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kogler.SerialCOM
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SerialModelTemplate { get; set; }
        public DataTemplate LoggerTemplate { get; set; }
        public DataTemplate ContentTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SerialModel) return SerialModelTemplate;
            if (item is Logger) return LoggerTemplate;
            if (item is PaneViewModel) return ContentTemplate;
            
            return base.SelectTemplate(item, container);
        }
    }

    class PanesStyleSelector : StyleSelector
    {
        public Style AnchorableStyle { get; set; }
        public Style DocumentStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is AnchorableViewModel) return AnchorableStyle;

            if (item is DocumentViewModel) return DocumentStyle;

            return base.SelectStyle(item, container);
        }
    }
}
