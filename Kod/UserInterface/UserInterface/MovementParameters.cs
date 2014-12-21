using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    public class AccelerometerParameters : EventArgs
    {
        private float accelerationAxisX;
        private float accelerationAxisY;
        private float accelerationAxisZ;

        public AccelerometerParameters(float accX, float accY, float accZ)
        {
            accelerationAxisX = accX;
            accelerationAxisY = accY;
            accelerationAxisZ = accZ;
        }

        public float AccelerationAxisX
        {
            get { return accelerationAxisX; }
            set { accelerationAxisX = value; }
        }

        public float AccelerationAxisY
        {
            get { return accelerationAxisY; }
            set { accelerationAxisY = value; }
        }

        public float AccelerationAxisZ
        {
            get { return accelerationAxisZ; }
            set { accelerationAxisZ = value; }
        }
    }


    public class GyroscopeParameters: EventArgs
    {
        private float angleAxisX;
        private float angleAxisY;
        private float angleAxisZ;

        public GyroscopeParameters(float gyrX, float gyrY, float gyrZ)
        {
            angleAxisX = gyrX;
            angleAxisY = gyrY;
            angleAxisZ = gyrZ;
        }

        public float AccelerationAxisX
        {
            get { return angleAxisX; }
            set { angleAxisX = value; }
        }

        public float AccelerationAxisY
        {
            get { return angleAxisY; }
            set { angleAxisY = value; }
        }

        public float AccelerationAxisZ
        {
            get { return angleAxisZ; }
            set { angleAxisZ = value; }
        }
    }
}
