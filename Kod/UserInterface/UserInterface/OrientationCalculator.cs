using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserInterface
{
    class OrientationCalculator
    {
        private static readonly int ACC_MEASURMENT_HISTORY = 3;
        private static readonly int GYR_MEASURMENT_HISTORY = 3;
        private static readonly float RAW_ACC_TO_G = 0.0039f;
        private static readonly float GYRO_SENSITIVITY = 8.75e-3f;
        private static readonly float SAMP_PERIOD = 0.05f;
        private static readonly float RADIANS_TO_DEG_COEF = 180.0f / (float)(Math.PI);
        private static readonly float COMPLEMENTARY_FILTER_GYRO_WEIGHT = 0.95f;
        private ThreeAxisMeasurmentData acc;
        private ThreeAxisMeasurmentData vel;
        private ThreeAxisMeasurmentData pos;
        private ThreeAxisMeasurmentData rate;
        private ThreeAxisMeasurmentData ang;
        private ThreeCoordinateValue posF;
        private ThreeCoordinateValue velF;
        private ThreeCoordinateValue angF;
        private ThreeCoordinateValue angF2;
        int zeroAccXCnt;
        int zeroAccYCnt;

        public OrientationCalculator()
        {
            acc = new ThreeAxisMeasurmentData(ACC_MEASURMENT_HISTORY);
            vel = new ThreeAxisMeasurmentData(ACC_MEASURMENT_HISTORY);
            pos = new ThreeAxisMeasurmentData(ACC_MEASURMENT_HISTORY);

            rate = new ThreeAxisMeasurmentData(GYR_MEASURMENT_HISTORY);
            ang = new ThreeAxisMeasurmentData(GYR_MEASURMENT_HISTORY);

            posF = new ThreeCoordinateValue();
            velF = new ThreeCoordinateValue();
            angF = new ThreeCoordinateValue();
            angF2 = new ThreeCoordinateValue();
        }

        public float[] position
        {
            get { return new float[]{ posF.x, posF.y, posF.z}; }
        }

        public float[] angle
        {
            get { return new float[] { angF.x, angF.y, angF.z }; }
        }

        private float posRatio
        {
            get { return RAW_ACC_TO_G * SAMP_PERIOD; }
        }

        private float velRatio
        {
            get { return RAW_ACC_TO_G * SAMP_PERIOD * SAMP_PERIOD; }
        }

        private float angRatio
        {
            get { return GYRO_SENSITIVITY * SAMP_PERIOD; }
        }

        public void countOrientationForSensorData(int[] accData, int[] gyroData)
        {
            getPosition(accData, gyroData);

            getAngle(accData, gyroData);
        }

        private void getPosition(int[] accData, int[] gyroData)
        {
            acc.x[1] = accData[0];
            acc.y[1] = accData[1];
            acc.z[1] = accData[2];

            Console.WriteLine("{0} {1} {2}", acc.x[1] * RAW_ACC_TO_G, acc.y[1] * RAW_ACC_TO_G, acc.z[1] * RAW_ACC_TO_G);

	        vel.x[1] = vel.x[0]+ acc.x[0]+ ( (acc.x[1] - acc.x[0]) / 2);
	        pos.x[1] = pos.x[0] + vel.x[0] + ( (vel.x[1] - vel.x[0]) / 2);

	        vel.y[1] = vel.y[0] + acc.y[0] + ( (acc.y[1] - acc.y[0]) / 2);
	        pos.y[1] = pos.y[0] + vel.y[0] + ( (vel.y[1] - vel.y[0]) / 2);

	        acc.x[0] = acc.x[1];
	        acc.y[0] = acc.y[1];

	        vel.x[0] = vel.x[1];
	        vel.y[0] = vel.y[1];

	        checkIfMotionEnded();

	        pos.x[0] = pos.x[1];
	        pos.y[0] = pos.y[1];

	        posF.x = posRatio * pos.x[1];
	        posF.y = posRatio * pos.y[1];
	        velF.x = velRatio * vel.x[1];
	        velF.y = velRatio * vel.y[1];
        }

        private void checkIfMotionEnded()
        {
	        if (0 == acc.x[1]){

		        ++zeroAccXCnt;
	        }
	        else {

		        zeroAccXCnt = 0;
	        }

	        if (zeroAccXCnt >= 25){

		        vel.x[1] = 0;
		        vel.x[0] = 0;
	        }

	        if (0 == acc.y[1]){

		        ++zeroAccYCnt;
	        }
	        else {

		        zeroAccYCnt = 0;
	        }

	        if (zeroAccYCnt >= 25){

		        vel.y[1] = 0;
		        vel.y[0] = 0;
	        }
        }

        private void getPosition2(int[] accData, int[] gyroData)
        {
            acc.x[1] = accData[0];
            acc.y[1] = accData[1];
            acc.z[1] = accData[2];

	        pos.x[1] += vel.x[1] + acc.x[1] / 2;
	        vel.x[1] = vel.x[1] + acc.x[1];

	        pos.y[1] += vel.y[1] + acc.y[1] / 2;
	        vel.y[1] = vel.y[1] + acc.y[1];

	        //TODO: ???
	        //checkIfMotionEnded();

	        posF.x = posRatio * pos.x[1];
	        posF.y = posRatio * pos.y[1];
	        velF.x = velRatio * vel.x[1];
	        velF.y = velRatio * vel.y[1];
        }

        private void getAngle(int[] accData, int[] gyroData)
        {
            rate.x[1] = gyroData[0];
            rate.y[1] = gyroData[1];
            rate.z[1] = gyroData[2];

	        ang.x[1] = ang.x[0] + rate.x[0] + ( (rate.x[1] - rate.x[0]) / 2);

	        ang.y[1] = ang.y[0] + rate.y[0] + ( (rate.y[1] - rate.y[0]) / 2);

	        ang.z[1] = ang.z[0] + rate.z[0] + ( (rate.z[1] - rate.z[0]) / 2);

	        rate.x[0] = rate.x[1];
	        rate.y[0] = rate.y[1];
	        rate.z[0] = rate.z[1];

	        //TODO: equivalent ???
	        //checkIfMotionEnded();

	        ang.x[0] = ang.x[1];
	        ang.y[0] = ang.y[1];
	        ang.z[0] = ang.z[1];

	        angF.x = angRatio * ang.x[1];
	        angF.y = angRatio * ang.y[1];
            angF.z = -angRatio * ang.z[1];
            
            angF2.x = (float)Math.Atan2(accData[1], accData[2]) * RADIANS_TO_DEG_COEF;
            angF2.y = (float)Math.Atan2(-accData[0], Math.Sqrt(accData[1] * accData[1] + accData[2] * accData[2])) * RADIANS_TO_DEG_COEF;

            angF.x = -COMPLEMENTARY_FILTER_GYRO_WEIGHT * angF.x - (1.0f - COMPLEMENTARY_FILTER_GYRO_WEIGHT) * angF2.x;
            angF.y = -COMPLEMENTARY_FILTER_GYRO_WEIGHT * angF.y - (1.0f - COMPLEMENTARY_FILTER_GYRO_WEIGHT) * angF2.y;
        }
    }

    class ThreeAxisMeasurmentData
    {
        public int[] x;
        public int[] y;
        public int[] z;

        public ThreeAxisMeasurmentData(int measurmentHistoryLength)
        {
            x = new int[measurmentHistoryLength];
            y = new int[measurmentHistoryLength];
            z = new int[measurmentHistoryLength];
        }
    }

    class ThreeCoordinateValue
    {
        public float x;
        public float y;
        public float z;
    }
}
