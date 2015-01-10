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
            graphicWindowMode = DataVisualisation.VisualisationMode.FullPositionMode;
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

                string[] measurmentStrings = measurementsString.Split(' ');
                string[] accelerometerDataStrings = new string[3];

                Array.Copy(measurmentStrings, 0, accelerometerDataStrings, 0, 3);

                string[] gyroscopeDataStrings = new string[3];

                Array.Copy(measurmentStrings, 3, gyroscopeDataStrings, 0, 3);

                int valuesNum = 3;
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

                visualisation.Dispatcher.Invoke(() => ApplyMovement(position, angles));
            }            
        }

        public void ApplyMovement(float[] accelerometerData, float[] gyroscopeData)
        {
            float accX = accelerometerData[0];
            float accY = accelerometerData[1];
            float accZ = accelerometerData[2];

            float angX = gyroscopeData[0];
            float angY = gyroscopeData[1];
            float angZ = gyroscopeData[2];

            if (!accelerationMeasurementStarted)
            {
                visualisation.AccelerationX = accX;
                visualisation.AccelerationY = accY;
                visualisation.AccelerationZ = accZ;
                AccelerationX = visualisation.AccelerationX;
                AccelerationY = visualisation.AccelerationY;
                AccelerationZ = visualisation.AccelerationZ;

                accelerationMeasurementStarted = true;
            }
            else
            {
                visualisation.AccelerationX = AccelerationX - accX;
                visualisation.AccelerationY = AccelerationY - accY;
                visualisation.AccelerationZ = AccelerationZ - accZ;
                AccelerationX = accX;
                AccelerationY = accY;
                AccelerationZ = accZ;
            }
            if (!angleMeasuremetStarted)
            {
                AngleX = visualisation.AngleX = angX;
                AngleY = visualisation.AngleY = angY;
                AngleZ = visualisation.AngleZ = angZ;
                AngleX = visualisation.AngleX;
                AngleY = visualisation.AngleY;
                AngleZ = visualisation.AngleZ;

                angleMeasuremetStarted = true;
            }
            else
            {
                visualisation.AngleX = AngleX - angX;
                visualisation.AngleY = AngleY - angY;
                visualisation.AngleZ = AngleZ - angZ;
                AngleX = angX;
                AngleY = angY;
                AngleZ = angZ;
            }
            visualisation.WindowMode = graphicWindowMode;
            visualisation.ApplyTransformation();
        }

        private void ResetMovement()
        {
            /*visualisation.WindowMode = graphicWindowMode;
            
            visualisation.AccelerationX = -orientationCalc.position[0];
            visualisation.AccelerationY = -orientationCalc.position[1];
            visualisation.AccelerationZ = -orientationCalc.position[2];

            visualisation.AngleX = -orientationCalc.angle[0];
            visualisation.AngleY = -orientationCalc.angle[1];
            visualisation.AngleZ = -orientationCalc.angle[2];

            visualisation.ApplyTransformation();*/
            visualisation.RemoveAllTransformations();
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
            startButton.Enabled = false;
            stopButton.Enabled = true;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            serialPortManager.StopCommunication();
            ClearVisualisation();
            startButton.Enabled = true;
            stopButton.Enabled = false;
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
