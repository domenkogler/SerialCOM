using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kogler.SerialCOM
{
    public abstract class SerialModel
    {
        protected List<List<string>> Entries { get; } = new List<List<string>>();

        protected SerialModel(string separator)
        {
            Separator = separator;
        }

        public virtual bool AddData(string data)
        {
            var splitted = data.Split(new[] {Separator}, StringSplitOptions.None);
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

        protected string Separator { get; }

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

        public BisModel() : base("|") { }

        protected override bool CreateNewModel(params string[] data)
        {
            var first = data[0];
            DateTime date;
            return DateTime.TryParse(first, out date);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            Entries.ForEach(e => sb.AppendLine(string.Join(Separator, e)));
            return sb.ToString();
        }
    }
}