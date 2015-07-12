﻿using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.AvalonDock.Layout;

namespace Kogler.SerialCOM
{
    public class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SerialModelTemplate { get; set; }
        public DataTemplate LoggerTemplate { get; set; }
        public DataTemplate AnchorableTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is SerialModel) return SerialModelTemplate;
            if (item is Logger) return LoggerTemplate;
            if (item is LayoutAnchorable) return AnchorableTemplate;
            
            return base.SelectTemplate(item, container);
        }
    }
}