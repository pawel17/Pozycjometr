using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class ReceivedDataEventArgs : EventArgs
    {
        private byte[] data;

        public ReceivedDataEventArgs(byte[] dataToSave)
        {
            data = dataToSave;
        }

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }
    }
}
