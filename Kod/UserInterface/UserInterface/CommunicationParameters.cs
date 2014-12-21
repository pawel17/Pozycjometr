using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace UserInterface
{
    static class CommunicationParameters
    {
        private static string portName = "COM4";
        private static int baudRate = 115200;
        private static int dataBits = 8;
        private static Parity parityBit = Parity.None;
        private static StopBits stopBit = StopBits.One;
        private static string[] availablePortNames;
        private static int[] availableBaudRates = new int[] { 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 46080, 921600 };
        private static int[] availableDataBits = new int[] { 5, 6, 7, 8 };       
        
        public static string PortName
        {
            get { return portName; }
            set
            {
                if (portName != value)
                {
                    portName = value;
                }
            }
        }

        public static int BaudRate
        {
            get { return baudRate; }
            set
            {
                if (baudRate != value)
                {
                    baudRate = value;
                }
            }
        }

        public static int DataBits
        {
            get { return dataBits; }
            set
            {
                if (dataBits != value)
                {
                    dataBits = value;
                }
            }
        }

        public static Parity ParityBits
        {
            get { return parityBit; }
            set
            {
                if (parityBit != value)
                {
                    parityBit = value;
                }
            }
        }

        public static StopBits StopBit
        {
            get { return stopBit; }
            set
            {
                if (stopBit != value)
                {
                    stopBit = value;
                }
            }
        }

        public static string[] AvailablePortNames
        {
            get { return availablePortNames; }
            set { availablePortNames = value; }
        }

        public static int[] AvailableBaudRates
        {
            get { return availableBaudRates; }
            set { availableBaudRates = value; }
        }

        public static int[] AvailableDataBits
        {
            get { return availableDataBits; }
            set { availableDataBits = value; }
        }
    }
}
