using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class CustomFile
    {
        public Header Header { get; set; }
        public ICollection<TradeRecord> Trades { get; set; }

        public CustomFile()
        {
            Header = new Header();
            Trades = new List<TradeRecord>();
        }

        public CustomFile(Header header, ICollection<TradeRecord> trades)
        {
            Header = header;
            Trades = trades;
        }
    }
}
