using System;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;

namespace Kogler.SerialCOM
{
    public class Logger
    {
        public FlowDocument Document { get; } = new FlowDocument();
        public StringBuilder Log { get; } = new StringBuilder();

        public void Write(string text, bool inline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            RunInUI(() =>
            {
                Paragraph p = null;
                if (inline)
                {
                    p = Document.Blocks.LastBlock as Paragraph;
                }
                if (p == null)
                {
                    p = new Paragraph { TextAlignment = TextAlignment.Left };
                    Document.Blocks.Add(p);
                }
                //var time = new Run(DateTime.Now.ToLongTimeString() + ": ");
                //p.Inlines.Add(time);
                var t = new Run(text);
                p.Loaded += ParagrafToView;
                p.Inlines.Add(t);
                Log?.AppendLine(text);
            });
        }

        private static async void RunInUI(Action action)
        {
            if (SynchronizationContext.Current != null) action();
            else await Application.Current.Dispatcher.BeginInvoke(action);
        }

        private static void ParagrafToView(object sender, RoutedEventArgs e)
        {
            var p = (Paragraph)sender;
            p.Loaded -= ParagrafToView;
            p.BringIntoView();
        }
    }
}