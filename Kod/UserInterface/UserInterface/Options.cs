using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace UserInterface
{
    public partial class Options : Form
    {

        public Options()
        {
            InitializeComponent();
            LoadCOMData();
        }

        private void LoadCOMData()
        {
            portNameCombo.DataSource = CommunicationParameters.AvailablePortNames;
            baudRateCombo.DataSource = CommunicationParameters.AvailableBaudRates;
            dataBitsCombo.DataSource = CommunicationParameters.DataBits;
            parityCombo.DataSource = Enum.GetValues(typeof(Parity));
            stopBitsCombo.DataSource = Enum.GetValues(typeof(StopBits));

            portNameCombo.DisplayMember = CommunicationParameters.PortName;
            baudRateCombo.DisplayMember = CommunicationParameters.BaudRate.ToString();
            dataBitsCombo.DisplayMember = CommunicationParameters.DataBits.ToString();
            parityCombo.DisplayMember = CommunicationParameters.ParityBits.ToString();
            stopBitsCombo.DisplayMember = CommunicationParameters.StopBit.ToString();
        }

        private void connectPutton_Click(object sender, EventArgs e)
        {
            CommunicationParameters.PortName = portNameCombo.DisplayMember;
            CommunicationParameters.BaudRate = Int32.Parse(baudRateCombo.DisplayMember);
            CommunicationParameters.DataBits = Int32.Parse(dataBitsCombo.DisplayMember);
            CommunicationParameters.ParityBits = (Parity)parityCombo.SelectedIndex;
            CommunicationParameters.StopBit = (StopBits)stopBitsCombo.SelectedIndex;
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
