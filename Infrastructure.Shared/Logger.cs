using System.Text;
using System.Windows;
using System.Windows.Documents;
using Kogler.Framework;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    public class Logger : ILogger
    {
        public StringBuilder Log { get; } = new StringBuilder();

        public virtual void Write(string text, bool inline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            if (inline) Log.Append(text);
            else Log.AppendLine(text);
        }
    }

    public class DocumentLogger : Logger
    {
        public FlowDocument Document { get; } = new FlowDocument();

        public override void Write(string text, bool inline = false)
        {
            if (string.IsNullOrEmpty(text)) return;
            base.Write(text, false);
            Dispatcher.RunInUI(() =>
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
            });
        }

        private static void ParagrafToView(object sender, RoutedEventArgs e)
        {
            var p = (Paragraph)sender;
            p.Loaded -= ParagrafToView;
            p.BringIntoView();
        }
    }
}