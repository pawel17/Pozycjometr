using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class ReceivedDataEventArgs : EventArgs
    {
        private string data;

        public ReceivedDataEventArgs(string dataToSave)
        {
            data = dataToSave;
        }

        public string Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
