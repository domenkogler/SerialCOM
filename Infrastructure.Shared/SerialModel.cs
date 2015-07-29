using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Kogler.SerialCOM.Infrastructure.Shared
{
    [AttributeUsage(AttributeTargets.Class), MetadataAttribute]
    public class SerialModelDefinitionAttribute : ExportAttribute, ISerialModelDescription
    {
        public SerialModelDefinitionAttribute(string description) : base(typeof(SerialModel))
        {
            Description = description;
        }
        public string Description { get; }
    }

    public interface ISerialModelDescription
    {
        string Description { get; }
    }

    public abstract class SerialModel : ISerialModelDescription
    {
        protected SerialModel(string separator)
        {
            Separator = separator;
        }

        public static string Separator { get; private set; }
        public System.IO.Ports.SerialPort Port { get; private set; }
        public string Header { get; protected set; }
        public List<string> Entries { get; } = new List<string>();
        public FixedSizedObservableQueue<string[]> Rows { get; } = new FixedSizedObservableQueue<string[]>(100);
        public IEnumerable<GridViewColumn> Columns => Split(Header).Select((h, i) => new GridViewColumn { Header = h, DisplayMemberBinding = new Binding($"[{i}]") });
        public KeyValuePair<string, string>[] Filters { get; protected set; }
        public abstract string Description { get; }
        public string SampleData { get; protected set; }

        public static IEnumerable<string> Split(string data)
        {
            return data.Split(new[] { Separator }, StringSplitOptions.None).Select(d => d.Trim());
        }

        public static string Join(IEnumerable<string> data)
        {
            return string.Join(Separator, data);
        }

        public void AddSampleData()
        {
            if (string.IsNullOrEmpty(SampleData)) return;
            var lines = SampleData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                AddData(line);
            }
        }

        public void Reset()
        {
            Entries.Clear();
            Rows.Clear();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            Entries.ForEach(e => sb.AppendLine(e));
            return sb.ToString();
        }

        public virtual bool AddData(string data)
        {
            var isNew = IsNew(data);
            if (isNew == null) return false;
            if (isNew.Value)
            {
                var filtered = FilterData();
                if (!string.IsNullOrEmpty(filtered)) Rows.Enqueue(Split(filtered).ToArray());
                Entries.Add(data);
            }
            else
            {
                if (Entries.Count == 0) return false;
                var last = Entries.Count - 1;
                Entries[last] = Entries[last] + data;
            }
            return isNew.Value;
        }

        private string FilterData()
        {
            var last = Entries.Count - 1;
            if (last < 0) return null;
            var data = Entries[last];
            data = Filters.Aggregate(data, (current, filter) => current.Replace(filter.Key, filter.Value));
            return Entries[last] = data;
        }

        private bool? IsNew(string data)
        {
            if (data.Length == 0) return null;
            return IsNewData(data);
        }

        protected abstract bool IsNewData(string data);
        public abstract System.IO.Ports.SerialPort GetSerialPort(string portName);
    }
}