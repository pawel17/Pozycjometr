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
            dataBitsCombo.DataSource = CommunicationParameters.AvailableDataBits;
            parityCombo.DataSource = Enum.GetValues(typeof(Parity));
            stopBitsCombo.DataSource = Enum.GetValues(typeof(StopBits));

            portNameCombo.SelectedItem = CommunicationParameters.PortName;
            baudRateCombo.SelectedItem = CommunicationParameters.BaudRate;
            dataBitsCombo.SelectedItem = CommunicationParameters.DataBits;
            parityCombo.SelectedItem = CommunicationParameters.ParityBits;
            stopBitsCombo.SelectedItem = CommunicationParameters.StopBit;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            CommunicationParameters.PortName = (string)portNameCombo.SelectedItem;
            CommunicationParameters.BaudRate = (int)baudRateCombo.SelectedItem;
            CommunicationParameters.DataBits = (int)dataBitsCombo.SelectedItem;
            CommunicationParameters.ParityBits = (Parity)parityCombo.SelectedIndex;
            CommunicationParameters.StopBit = (StopBits)stopBitsCombo.SelectedIndex;
            Close();
        }

        private void backButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
