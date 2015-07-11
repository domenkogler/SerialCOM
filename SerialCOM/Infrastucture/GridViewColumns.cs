using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Kogler.SerialCOM
{
    public static class GridViewColumns
    {
        [AttachedPropertyBrowsableForType(typeof (GridView))]
        public static IEnumerable<GridViewColumn> GetColumnsSource(DependencyObject obj)
        {
            return (IEnumerable<GridViewColumn>) obj.GetValue(ColumnsSourceProperty);
        }

        public static void SetColumnsSource(DependencyObject obj, IEnumerable<GridViewColumn> value)
        {
            obj.SetValue(ColumnsSourceProperty, value);
        }
        
        public static readonly DependencyProperty ColumnsSourceProperty = DependencyProperty
            .RegisterAttached("ColumnsSource",
                typeof(IEnumerable<GridViewColumn>),
                typeof(GridViewColumns),
                new UIPropertyMetadata(null, ColumnsSourceChanged));

        private static void ColumnsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var gridView = obj as GridView;
            var columns = e.NewValue as IEnumerable<GridViewColumn>;
            if (gridView == null || columns == null) return;
            gridView.Columns.Clear();
            foreach (var column in columns)
            {
                gridView.Columns.Add(column);
            }
        }
    }
}