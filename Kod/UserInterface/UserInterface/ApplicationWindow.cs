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
        private OrientationCalculator orientationCalc;
        private DataVisualisation.VisualisationMode graphicWindowMode;

        public ApplicationWindow()
        {
            InitializeComponent();
            SerialPortCommunicationInit();

            orientationCalc = new OrientationCalculator();
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
            else
            {
                string measurementsString = args.Data;
                //string gyroscopeString = string.Empty;
                //string accelerometerString = string.Empty;

                //if ((message.IndexOf("ACL") != message.LastIndexOf("ACL")) || (message.IndexOf("GYR") != message.LastIndexOf("GYR")))
                //{
                //    return;
                //}

                //if ((message.IndexOf("GYR") != -1) && (message.IndexOf("ACL") != -1))
                //{
                //    if (message.IndexOf("GYR") == 0)            //in case "GYR" in the first part of the message
                //    {
                //        accelerometerString = "";
                //    }
                //    else
                //    {
                //        accelerometerString = message.Substring(0, message.IndexOf("GYR") - 1);
                //    }

                //    gyroscopeString = message.Substring(message.IndexOf("GYR"));
                //}
                //else if (message.IndexOf("ACL") != -1)
                //{
                //    accelerometerString = message;
                //}
                //else
                //{
                //    gyroscopeString = message;
                //}

                string[] measurmentStrings = measurementsString.Split(' ');
                string[] accelerometerDataStrings = new string[3];

                Array.Copy(measurmentStrings, 0, accelerometerDataStrings, 0, 3);

                //if (accelerometerString.Length > 0)
                //{
                //    Array.Copy(accelerometerString.Split(' '), 1, accelerometerData, 0, 3);

                //    if(accelerationX.InvokeRequired) {
                //        accelerationX.Invoke(new MethodInvoker(delegate { accelerationX.Text = accelerometerData[0]; })); 
                //    }

                //    if(accelerationY.InvokeRequired) {
                //        accelerationY.Invoke(new MethodInvoker(delegate { accelerationY.Text = accelerometerData[1]; })); 
                //    }

                //    if(accelerationZ.InvokeRequired) {
                //        accelerationZ.Invoke(new MethodInvoker(delegate { accelerationZ.Text = accelerometerData[2]; })); 
                //    }
                //}


                string[] gyroscopeDataStrings = new string[3];

                Array.Copy(measurmentStrings, 3, gyroscopeDataStrings, 0, 3);

                //Console.WriteLine("ACCELEROMETER:" + String.Join(" ", accelerometerDataStrings));
                //Console.WriteLine("GYROSCOPE: " + String.Join(" ", gyroscopeDataStrings));

                //if (gyroscopeString.Length > 0)
                //{
                //    Array.Copy(gyroscopeString.Split(' '), 1, gyroscopeData, 0, 3);

                //    if(angleX.InvokeRequired) {
                //        angleX.Invoke(new MethodInvoker(delegate { angleX.Text = gyroscopeData[0]; })); 
                //    }

                //    if(angleY.InvokeRequired) {
                //        angleY.Invoke(new MethodInvoker(delegate { angleY.Text = gyroscopeData[1]; })); 
                //    }

                //    if(angleZ.InvokeRequired) {
                //        angleZ.Invoke(new MethodInvoker(delegate { angleZ.Text = gyroscopeData[2]; })); 
                //    }
                //}

                int valuesNum = 3;
                string positionString = "";
                string angleString = "";

                //if(accelerometerString.Length > 0) {

                    int[] accMeasurments = new int[]{0, 0, 0};
                    int[] gyroMeasurments = new int[]{ 0, 0, 0 };

                    for (int valuesCnt = 0; valuesCnt < valuesNum; ++valuesCnt )
                    {
                        accMeasurments[valuesCnt] = int.Parse(accelerometerDataStrings[valuesCnt]);
                        gyroMeasurments[valuesCnt] = int.Parse(gyroscopeDataStrings[valuesCnt]);
                    }

                    orientationCalc.countOrientationForSensorData(accMeasurments, gyroMeasurments);

                    float[] position = orientationCalc.position;
                    float[] angles = orientationCalc.angle;
                    string[] positionStrings = new string[3];
                    string[] angleStrings = new string[3];

                    for (int valuesCnt = 0; valuesCnt < valuesNum; ++valuesCnt)
                    {
                        positionStrings[valuesCnt] = position[valuesCnt].ToString();
                        angleStrings[valuesCnt] = angles[valuesCnt].ToString();
                    }

                    accelerationX.Text = positionStrings[0];
                    accelerationY.Text = positionStrings[1];
                    accelerationZ.Text = positionStrings[2];

                    angleX.Text = angleStrings[0];
                    angleY.Text = angleStrings[1];
                    angleZ.Text = angleStrings[2];

                    positionString = String.Join(" ", positionStrings);

                    angleString = String.Join(" ", angleStrings);
               // }

                visualisation.Dispatcher.Invoke(() => ApplyMovement(position, angles));
            }            
        }

        public void ApplyMovement(float[] accelerometerData, float[] gyroscopeData)
        {
            //Console.WriteLine("ACCELEROMETER:" + accelerometer);
            //Console.WriteLine("GYROSCOPE: " + gyroscope);
            //string[] accelerometerData;
            //string[] gyroscopeData;

            //accelerometerData = accelerometer.Split(' ');
            //gyroscopeData = gyroscope.Split(' ');

            float accX = accelerometerData[0];//float.Parse(accelerometerData[0], System.Globalization.CultureInfo.CurrentCulture)/10;
            float accY = accelerometerData[1];// float.Parse(accelerometerData[1], System.Globalization.CultureInfo.CurrentCulture) / 10;
            float accZ = accelerometerData[2];//float.Parse(accelerometerData[2], System.Globalization.CultureInfo.CurrentCulture) / 10;

            float angX = gyroscopeData[0];// float.Parse(gyroscopeData[0], System.Globalization.CultureInfo.CurrentCulture);
            float angY = gyroscopeData[1];// float.Parse(gyroscopeData[1], System.Globalization.CultureInfo.CurrentCulture);
            float angZ = gyroscopeData[2];// float.Parse(gyroscopeData[2], System.Globalization.CultureInfo.CurrentCulture);

            if (!accelerationMeasurementStarted)
            {
                //if (accelerometer.Length > 0)
                //{
                    visualisation.AccelerationX = accX;
                    visualisation.AccelerationY = accY;
                    visualisation.AccelerationZ = accZ;
                    AccelerationX = visualisation.AccelerationX;
                    AccelerationY = visualisation.AccelerationY;
                    AccelerationZ = visualisation.AccelerationZ;
                //}
                accelerationMeasurementStarted = true;
            }
            else
            {
                //if (accelerometer.Length > 0)
                //{
                    visualisation.AccelerationX = AccelerationX - accX;
                    visualisation.AccelerationY = AccelerationY - accY;
                    visualisation.AccelerationZ = AccelerationZ - accZ;
                    AccelerationX = accX;
                    AccelerationY = accY;
                    AccelerationZ = accZ;
                //}
            }
            if (!angleMeasuremetStarted)
            {
                //if (gyroscope.Length > 0)
                //{
                    AngleX = visualisation.AngleX = angX;
                    AngleY = visualisation.AngleY = angY;
                    AngleZ = visualisation.AngleZ = angZ;
                    AngleX = visualisation.AngleX;
                    AngleY = visualisation.AngleY;
                    AngleZ = visualisation.AngleZ;
                //}
                angleMeasuremetStarted = true;
            }
            else
            {
                //if (gyroscope.Length > 0)
                //{
                    //gyroscopeData = gyroscope.Split(' ');
                    visualisation.AngleX = AngleX - angX;
                    visualisation.AngleY = AngleY - angY;
                    visualisation.AngleZ = AngleZ - angZ;
                    AngleX = angX;
                    AngleY = angY;
                    AngleZ = angZ;
                //}
            }
            visualisation.ApplyTransformation();
        }

        private void ResetMovement()
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

        private void ClearVisualisation()
        {
            visualisation.Dispatcher.Invoke(() => ResetMovement());
            serialPortManager.ClearMicroprocessorVariables();
            accelerationX.Text = "";
            accelerationY.Text = "";
            accelerationZ.Text = "";
            angleX.Text = "";
            angleY.Text = "";
            angleZ.Text = "";
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
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            serialPortManager.StopCommunication();
            ClearVisualisation();
        }

        private void rotationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (rotationRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.RotationMode;
                ClearVisualisation();
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false;
            }
        }

        private void translationRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (translationRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.TranslationMode;
                ClearVisualisation();
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false;
            }
        }

        private void fullRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (fullRadioButton.Checked == true)
            {
                graphicWindowMode = DataVisualisation.VisualisationMode.FullPositionMode;
                ClearVisualisation();
                angleMeasuremetStarted = false;
                accelerationMeasurementStarted = false;
            }
        }
    }
}
