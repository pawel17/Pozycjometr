using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;
using System.Windows.Forms.Integration;
using System.Windows.Media;

namespace UserInterface
{
    public partial class ApplicationWindow : Form
    {
        private DataVisualisation.MainWindow visualisation;
        private ElementHost visualisationHost;
        private COMPortManager serialPortManager;
        private bool startAnimationFlag = false;

        public ApplicationWindow()
        {
            InitializeComponent();
            SerialPortCommunicationInit();
        }

        private void SerialPortCommunicationInit()
        {
            serialPortManager = new COMPortManager();
            serialPortManager.ReceivedData += new EventHandler<ReceivedDataEventArgs>(NewSerialDataReceived);
        }

        private void NewSerialDataReceived(object sender, ReceivedDataEventArgs args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new EventHandler<ReceivedDataEventArgs>(NewSerialDataReceived), new object[] { sender, args });
            }

            string message = Convert.ToBase64String(args.Data);
            string[] elements = message.Split(' ');

            if(startAnimationFlag)
            {
                if (elements[0] == "ACL")
                {
                    accelerationX.Text = elements[1];
                    accelerationY.Text = elements[2];
                    accelerationZ.Text = elements[3];

                    visualisation.ApplyAcceleration(float.Parse(elements[1]), float.Parse(elements[2]), float.Parse(elements[3]));
                }
                else if (elements[0] == "GYR")
                {
                    angleX.Text = elements[1];
                    angleY.Text = elements[2];
                    angleZ.Text = elements[3];

                    visualisation.ApplyRotation(float.Parse(elements[1]), float.Parse(elements[2]), float.Parse(elements[3]));
                }
            }
        }

        private void ApplicationWindow_FormClosed(object sender, FormClosedEventArgs args)
        {
            serialPortManager.Dispose();
            System.Windows.Forms.Application.Exit();
        }

        private void ApplicationWindow_Load(object sender, EventArgs e)
        {
            visualisationHost = new ElementHost();
            visualisationHost.Dock = DockStyle.Fill;
            visualisation = new DataVisualisation.MainWindow();
            visualisationHost.Child = visualisation;
            mainPanel.Controls.Add(visualisationHost);
        }

        private void connectionSettings_Click(object sender, EventArgs e)
        {
            UserInterface.Options options = new UserInterface.Options();
            options.Show();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            string text = "Autorzy: Łukasz Cisowski, Paweł Mazik\nPromotor: dr inż. Krzysztof Świentek\nIV rok Informatyka Stosowana Wydział Fizyki i Informatyki Stosowanej\nAkademia Górniczo-Hutnicza im. Stanisława Staszica w Krakowie";
            System.Windows.Forms.MessageBox.Show(text, "O programie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            if (!serialPortManager.StartCommunication())
            {
                System.Windows.Forms.MessageBox.Show("Nie nawiązano połączenia z portem COM,\n proszę sprawdzić ustawienia", "O programie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                startAnimationFlag = false;
            }
            else
            {
                startAnimationFlag = true;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            serialPortManager.StopCommunication();
            startAnimationFlag = false;
        }
    }
}
