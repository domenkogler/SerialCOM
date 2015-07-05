using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Kogler.SerialCOM
{
    public abstract class SerialModel
    {
        public List<List<string>> Entries { get; } = new List<List<string>>();

        protected SerialModel(string separator)
        {
            Separator = separator;
        }

        public virtual bool AddData(string data)
        {
            var splitted = Split(data);
            var isNew = CanCreateNewModel(splitted);
            if (isNew == null) return false;
            if (isNew.Value) Entries.Add(splitted.ToList());
            else
            {
                var last = Entries.LastOrDefault();
                if (last == null) return false;
                last.AddRange(splitted);
            }
            return isNew.Value;
        }

        protected static string[] Split(string data)
        {
            return data.Split(new[] {Separator}, StringSplitOptions.None);
        }

        protected static string Join(IEnumerable<string> data)
        {
            return string.Join(Separator, data);
        }

        protected static string Separator { get; private set; }

        private bool? CanCreateNewModel(params string[] data)
        {
            if (data.Length == 0) return null;
            return CreateNewModel(data);
        }

        protected abstract bool CreateNewModel(params string[] data);
    }

    public class BisModel : SerialModel
    {
        public const string Header = "TIME               |DSC     |PIC     |Filters |Alarm   |Lo-Limit|Hi-Limit|Silence |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |";
        public string[] Headers => Split(Header);
        public const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss";

        private static int? _slots;
        private static int Slots => (int)(_slots ?? (_slots = Split(Header).Length));

        public BisModel() : base("|") { }

        protected override bool CreateNewModel(params string[] data)
        {
            var first = data[0].Trim();
            DateTime date;
            var ok = DateTime.TryParseExact(first, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,  out date);
            if (!ok) return false;
            var diff = date - DateTime.Now;
            var min = TimeSpan.FromDays(1);
            var isNew = diff < min;
            if (isNew) FilterData();
            return isNew;
        }

        private void FilterData()
        {
            var last = Entries.LastOrDefault();
            if (last == null || last.Count <= Slots) return;
            var lastTrim = last.Select(d => d.Trim());
            var data = Join(lastTrim);
            data = data.Replace($"{Separator}{Separator}", Separator);
            data = data.Replace($".{Separator}", ".");
            data = data.Replace($"{Separator}.", ".");
            var split = Split(data);
            last.Clear();
            last.AddRange(split);
        }

        public override string ToString()
        {
            //TODO: .|, |., 0|0, || 
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            Entries.ForEach(e => sb.AppendLine(Join(e)));
            return sb.ToString();
        }
    }
}