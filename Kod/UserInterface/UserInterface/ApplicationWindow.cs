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
        private bool angleMeasuremetStarted = false;
        private bool accelerationMeasurementStarted = false;
        private float AngleX = 0, AngleY = 0, AngleZ = 0;
        private float AccelerationX = 0, AccelerationY = 0, AccelerationZ = 0;
        private DataVisualisation.VisualisationMode graphicWindowMode;

        public ApplicationWindow()
        {
            InitializeComponent();
            SerialPortCommunicationInit();
            graphicWindowMode = DataVisualisation.VisualisationMode.FullPositionMode;
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
                    accelerometerString = "";
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

            if (!accelerationMeasurementStarted)
            {
                if (accelerometer.Length > 0)
                {
                    accelerometerData = accelerometer.Split(' ');
                    visualisation.AccelerationX = float.Parse(accelerometerData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    visualisation.AccelerationY = float.Parse(accelerometerData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    visualisation.AccelerationZ = float.Parse(accelerometerData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    AccelerationX = visualisation.AccelerationX;
                    AccelerationY = visualisation.AccelerationY;
                    AccelerationZ = visualisation.AccelerationZ;
                }
                accelerationMeasurementStarted = true;
            }
            else
            {
                if (accelerometer.Length > 0)
                {
                    accelerometerData = accelerometer.Split(' ');
                    visualisation.AccelerationX = AccelerationX - float.Parse(accelerometerData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    visualisation.AccelerationY = AccelerationY - float.Parse(accelerometerData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    visualisation.AccelerationZ = AccelerationZ - float.Parse(accelerometerData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    AccelerationX = float.Parse(accelerometerData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    AccelerationY = float.Parse(accelerometerData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                    AccelerationZ = float.Parse(accelerometerData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture)/10;
                }
            }
            if (!angleMeasuremetStarted)
            {
                if (gyroscope.Length > 0)
                {
                    gyroscopeData = gyroscope.Split(' ');
                    visualisation.AngleX = float.Parse(gyroscopeData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    visualisation.AngleY = float.Parse(gyroscopeData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    visualisation.AngleZ = float.Parse(gyroscopeData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    AngleX = visualisation.AngleX;
                    AngleY = visualisation.AngleY;
                    AngleZ = visualisation.AngleZ;
                }
                angleMeasuremetStarted = true;
            }
            else
            {
                if (gyroscope.Length > 0)
                {
                    gyroscopeData = gyroscope.Split(' ');
                    visualisation.AngleX = AngleX - float.Parse(gyroscopeData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    visualisation.AngleY = AngleY - float.Parse(gyroscopeData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    visualisation.AngleZ = AngleZ - float.Parse(gyroscopeData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    AngleX = float.Parse(gyroscopeData[1], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    AngleY = float.Parse(gyroscopeData[2], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                    AngleZ = float.Parse(gyroscopeData[3], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                }
            }
            visualisation.ApplyTransformation();
        }

        public void ResetMovement()
        {
            visualisation.WindowMode = graphicWindowMode;

            visualisation.AccelerationX = -AccelerationX;
            visualisation.AccelerationY = -AccelerationY;
            visualisation.AccelerationZ = -AccelerationZ;

            visualisation.AngleX = -AngleX;
            visualisation.AngleY = -AngleY;
            visualisation.AngleZ = -AngleZ;

            visualisation.ApplyTransformation();
        }

        private void ApplicationWindow_FormClosed(object sender, FormClosedEventArgs args)
        {
            //serialPortManager.Dispose();
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
            startButton.Enabled = false;
            stopButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            serialPortManager.StopCommunication();
            startButton.Enabled = true;
            stopButton.Enabled = false;
        }

        private void rotationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rotationRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.RotationMode;
                visualisation.Dispatcher.Invoke(() => ResetMovement());
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false;
            }         
        }

        private void translationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (translationRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.TranslationMode;
                visualisation.Dispatcher.Invoke(() => ResetMovement());
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false; 
            }         
        }

        private void fullRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fullRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.FullPositionMode;
                visualisation.Dispatcher.Invoke(() => ResetMovement());
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false;
            }
        }
    }
}
