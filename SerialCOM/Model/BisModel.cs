using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace Kogler.SerialCOM
{
    public abstract class SerialModel
    {
        public List<string> Entries { get; } = new List<string>();

        protected SerialModel(string separator)
        {
            Separator = separator;
        }

        public virtual bool AddData(string data)
        {
            var isNew = IsNew(data);
            if (isNew == null) return false;
            if (isNew.Value) Entries.Add(data);
            else
            {
                if (Entries.Count == 0) return false;
                var last = Entries.Count - 1;
                Entries[last] = Entries[last] + data;
            }
            return isNew.Value;
        }

        protected static IEnumerable<string> Split(string data)
        {
            return data.Split(new[] {Separator}, StringSplitOptions.None).Select(d=>d.Trim());
        }

        protected static string Join(IEnumerable<string> data)
        {
            return string.Join(Separator, data);
        }

        protected static string Separator { get; private set; }

        private bool? IsNew(string data)
        {
            if (data.Length == 0) return null;
            return IsNewModel(data);
        }

        protected abstract bool IsNewModel(string data);
    }

    public class BisModel : SerialModel
    {
        public const string Header = "TIME               |DSC     |PIC     |Filters |Alarm   |Lo-Limit|Hi-Limit|Silence |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |";
        public const string SampleData =
            @"07/03/2015 13:39:45|       10|      27|On      |None    |Off    
 |Off     |Yes     |     0.0| 
   10.1|    0000|    36.2|    6
0.9|    26.9|   100.0|      10
|00000000|     0.0|     0.1|  
  0000|    32.8|    60.9|    2
6.9|   100.0|       7|00000000
|     0.0|    10.1|    0000|  
  36.2|    60.9|    26.9|   100.0|       0|00000000|
 
07/03/2015 13:39:50|       10|    
  27|On      |None    |Off    
 |Off     |Yes     |     0.0|    10.0|    0000|    34.6|    60.9|    26.9|   100.0|      10
|00000000|     0.0|     0.2|    0000|    31.6|    60.9|    2
6.9|   100.0|       7|00000000|     0.0|    10.0|    0000|  
  34.6|    60.9|    26.9|   10
0.0|       0|00000000|
 
07/03/2015 13:39:55|       10|      27|On 
     |None    |Off     |Off   
  |Yes     |     0.0|    10.1|
    0000|    37.8|    60.6|   
 26.5|   100.0|      10|00000008|     0.0|     0.1|    0000|
    35.2|    60.6|    26.5|   1
00.0|       7|00000000|     0.
0|    10.1|    0000|    37.8|    60.6|    26.5|   100.0|    
   0|00000008|
 
07/03/2015 13:40:00|       10|      27
|On      |None    |Off     |Off     |Yes     |     0.0|    1
0.4|    0000|    40.7|    59.3
|    26.8|   100.0|      10|00000080|     0.0|     0.1|    0
000|    38.6|    59.3|    26.8
|   100.0|       7|00000000|   
  0.0|    10.4|    0000|    40.7|    59.3|    26.8|   100.0|
       0|00000080|
 
07/03/2015 13:40:05|       10|  
    27|On      |None    |Off     |Off     |Yes     |     0.0|
    10.9|    0000|    42.3|   
 60.9|    28.6|   100.0|      
10|00000080|     0.0|     0.1|    0000|    41.3|    60.9|   
 28.6|   100.0|       7|000000
00|     0.0|    10.9|    0000|
    42.3|    60.9|    28.6|   
100.0|       0|00000080|";

        public IEnumerable<string> Headers => Split(Header);
        public const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
        public ObservableCollection<string[]> Rows { get; } = new ObservableCollection<string[]>();
        public IEnumerable<GridViewColumn> Columns => Headers.Select((h, i) => new GridViewColumn { Header = h, DisplayMemberBinding = new Binding($"[{i}]")});

        public BisModel() : base("|") { }

        public void AddSampleData()
        {
            var lines = SampleData.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                AddData(line);
            }
        }

        protected override bool IsNewModel(string data)
        {
            var first = Split(data).FirstOrDefault();
            DateTime date;
            var ok = DateTime.TryParseExact(first, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,  out date);
            if (!ok) return false;
            var diff = date - DateTime.Now;
            var min = TimeSpan.FromDays(1);
            var isNew = diff < min;
            if (isNew)
            {
                var filtered = FilterData();
                if (string.IsNullOrEmpty(filtered)) return isNew;
                Rows.Add(Split(filtered).ToArray());
            }
            return isNew;
        }

        private string FilterData()
        {
            var last = Entries.Count - 1;
            if (last < 0) return null;
            var data = Entries[last];
            data = data.Replace($"{Separator}{Separator}", Separator);
            data = data.Replace($".{Separator}", ".");
            data = data.Replace($"{Separator}.", ".");
            return Entries[last] = data;
        }

        private KeyValuePair<string, string>[] ToDic(string data)
        {
            var splitted = Split(data).ToArray();
            var headers = Headers.ToArray();
            var min = Math.Min(Headers.Count(), splitted.Count());
            var dic = new KeyValuePair<string, string>[min];
            for (var index = 0; index < min; index++)
            {
                dic[index] = new KeyValuePair<string, string>(headers[index],splitted[index]);
            }
            return dic;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine(Header);
            Entries.ForEach(e => sb.AppendLine(e));
            return sb.ToString();
        }
    }
}