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
using System.Windows.Media.Media3D;

namespace UserInterface
{
    public partial class ApplicationWindow : Form
    {
        private DataVisualisation.MainWindow visualisation;
        private ElementHost visualisationHost;
        private COMPortManager serialPortManager;
        private bool accelerometerMeasurementBegan = false;
        private bool gyroscopeMeasurementBegan = false;

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

            string message = Encoding.ASCII.GetString(args.Data);
            string gyroscopeString = string.Empty;
            string accelerometerString = string.Empty;

            if ((message.IndexOf("ACL") != message.LastIndexOf("ACL")) || (message.IndexOf("GYR") != message.LastIndexOf("GYR")))
            {
                return;
            }

            if ((message.IndexOf("GYR") != -1) && (message.IndexOf("ACL") != -1))
            {
                if (message.IndexOf("GYR") == 0)            //in case "GYR" in the first part of the message
                {
                    accelerometerString = message;
                }
                else
                {
                    accelerometerString = message.Substring(0, message.IndexOf("GYR") - 1);
                }

                gyroscopeString = message.Substring(message.IndexOf("GYR"));
            }
            else if (message.IndexOf("ACL") != -1)
            {
                accelerometerString = message;
            }
            else
            {
                gyroscopeString = message;
            }

            if (accelerometerString.Length > 0)
            {                
                string[] accelerometerData = accelerometerString.Split(' ');

                if(accelerationX.InvokeRequired) {
                    accelerationX.Invoke(new MethodInvoker(delegate { accelerationX.Text = accelerometerData[1]; })); 
                }

                if(accelerationY.InvokeRequired) {
                    accelerationY.Invoke(new MethodInvoker(delegate { accelerationY.Text = accelerometerData[2]; })); 
                }

                if(accelerationZ.InvokeRequired) {
                    accelerationZ.Invoke(new MethodInvoker(delegate { accelerationZ.Text = accelerometerData[3]; })); 
                }
            }

            if (gyroscopeString.Length > 0)
            {
                string[] gyroscopeData = gyroscopeString.Split(' ');

                if(angleX.InvokeRequired) {
                    angleX.Invoke(new MethodInvoker(delegate { angleX.Text = gyroscopeData[1]; })); 
                }

                if(angleY.InvokeRequired) {
                    angleY.Invoke(new MethodInvoker(delegate { angleY.Text = gyroscopeData[2]; })); 
                }

                if(angleZ.InvokeRequired) {
                    angleZ.Invoke(new MethodInvoker(delegate { angleZ.Text = gyroscopeData[3]; })); 
                }
            }       

            visualisation.Dispatcher.Invoke(() => ApplyMovement(accelerometerString, gyroscopeString));            
        }

        public void ApplyMovement(string accelerometer, string gyroscope)
        {
            string[] accelerometerData;
            string[] gyroscopeData;

            if (!accelerometerMeasurementBegan && accelerometer.Length > 0)
            {
                accelerometerData = accelerometer.Split(' ');
                visualisation.AccelerationX = float.Parse(accelerometerData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AccelerationY = float.Parse(accelerometerData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AccelerationZ = float.Parse(accelerometerData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                accelerometerMeasurementBegan = true;
            }
            else if (accelerometer.Length > 0)
            {
                accelerometerData = accelerometer.Split(' ');
                visualisation.AccelerationX -= float.Parse(accelerometerData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AccelerationY -= float.Parse(accelerometerData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AccelerationZ -= float.Parse(accelerometerData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (!gyroscopeMeasurementBegan && gyroscope.Length > 0)
            {
                gyroscopeData = gyroscope.Split(' ');
                visualisation.AngleX = float.Parse(gyroscopeData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AngleY = float.Parse(gyroscopeData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AngleZ = float.Parse(gyroscopeData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                gyroscopeMeasurementBegan = true;
            }
            else if (gyroscope.Length > 0)
            {
                gyroscopeData = gyroscope.Split(' ');
                visualisation.AngleX -= float.Parse(gyroscopeData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AngleY -= float.Parse(gyroscopeData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                visualisation.AngleZ -= float.Parse(gyroscopeData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
            }
            visualisation.ApplyTransformation();
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
            serialPortManager.RefreshAvailablePorts();
            options.Show();
        }

        private void infoButton_Click(object sender, EventArgs e)
        {
            string text = "Autorzy: Łukasz Cisowski, Paweł Mazik\nPromotor: dr inż. Krzysztof Świentek\nIV rok Informatyka Stosowana Wydział Fizyki i Informatyki Stosowanej\nAkademia Górniczo-Hutnicza im. Stanisława Staszica w Krakowie";
            System.Windows.Forms.MessageBox.Show(text, "O programie", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (CommunicationParameters.AvailableBaudRates.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Nie nawiązano połączenia z portem COM,\n proszę sprawdzić ustawienia", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    serialPortManager.StartCommunication();
                }
            }
            catch (Exception)
            {
                System.Windows.Forms.MessageBox.Show("Brak połączenia z portem COM. \nProszę ustawić odpowiednie paremetry portu.", "Brak połączenia", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            serialPortManager.StopCommunication();
        }
    }
}
