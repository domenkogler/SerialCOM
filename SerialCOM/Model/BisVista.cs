using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

namespace Kogler.SerialCOM
{
    public class BisVista : SerialModel
    {
        #region << Constants >>

        public const string BisHeader = "TIME               |DSC     |PIC     |Filters |Alarm   |Lo-Limit|Hi-Limit|Silence |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |SR11    |SEF07   |BISBIT00|BIS     |TOTPOW07|EMGLOW01|SQI09   |IMPEDNCE|ARTF2   |";
        public const string DateTimeFormat = "MM/dd/yyyy HH:mm:ss";
        public const string BisSampleData =
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

        #endregion

        public BisVista() : base("|")
        {
            Header = BisHeader;
            SampleData = BisSampleData;
            Filters = new[]
            {
                new KeyValuePair<string, string>($"{Separator}{Separator}", Separator),
                new KeyValuePair<string, string>($".{Separator}", "."),
                new KeyValuePair<string, string>($"{Separator}.", ".")
            };
        }

        protected override bool IsNewData(string data)
        {
            var first = Split(data).FirstOrDefault();
            DateTime date;
            var ok = DateTime.TryParseExact(first, DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,  out date);
            if (!ok) return false;
            var diff = date - DateTime.Now;
            var min = TimeSpan.FromDays(1);
            var isNew = diff < min;
            return isNew;
        }

        public override SerialPort GetSerialPort(string portName)
        {
            return new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
        }
    }
}