using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO.Ports;

namespace UserInterface
{
    public class COMPortManager : IDisposable
    {
        private SerialPort serialPort;
        public event EventHandler<ReceivedDataEventArgs> ReceivedData;

        public COMPortManager()
        {
            CommunicationParameters.AvailablePortNames = SerialPort.GetPortNames();            
        }

        ~COMPortManager()
        {
            Dispose(false);
        }

        public bool StartCommunication()
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            serialPort = new SerialPort(CommunicationParameters.PortName,
                                        CommunicationParameters.BaudRate,
                                        CommunicationParameters.ParityBits,
                                        CommunicationParameters.DataBits,
                                        CommunicationParameters.StopBit);

            serialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPortDataReceived);
            serialPort.Open();
            return serialPort.IsOpen;
        }

        public void StopCommunication()
        {
            serialPort.Close();
        }

        public void ClearMicroprocessorVariables()
        {
            serialPort.Write("RESET");
        }

        public void RefreshAvailablePorts()
        {
            CommunicationParameters.AvailablePortNames = SerialPort.GetPortNames();
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string all = serialPort.ReadLine();

            if (ReceivedData != null)
                ReceivedData(this, new ReceivedDataEventArgs(all));
        }

        public void Dispose()
        {
            Dispose(true);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                serialPort.DataReceived -= new SerialDataReceivedEventHandler(SerialPortDataReceived);
            }

            if (serialPort != null)
            {
                if (serialPort.IsOpen)
                {
                    serialPort.Close();
                }
                serialPort.Dispose();
            }
        }
    }
}
