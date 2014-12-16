#include <LPC17xx.h>
#include "spihal_ssp.h"
#include "ADXL345Add.h"
#include "L3G4200D.h"
#include <lpc17xx_timer.h>
#include <lpc17xx_clkpwr.h>
#include <math.h>
#include "string.h"
//#include "LED.h"

#define DLEN										1u
#define BUFF_LEN 								1
#define TIMER_PRIORITY					30u
#define ACC_SAMPLES_PER_MEASURE	0x040
#define GYRO_SAMPLES_PER_MEASURE	0x040
//TODO: a bit to high (mayby different vals for each axis ???)
#define ACC_NOISE_TRESHOLD		20
#define ACC_SAMPLES_HISTORY_LEN	2
#define GYRO_NOISE_TRESHOLD		70
#define GYRO_SAMPLES_HISTORY_LEN	2
#define G_ACC					9.81f
#define VEL_UNITS_RATIO(FREQ)	( G_ACC / (FREQ) )
#define POS_UNITS_RATIO(FREQ)	( G_ACC / (FREQ) / (FREQ) )
#define ANG_UNITS_RATIO(FREQ)	(1.0f / (FREQ))
#define RADIANS_TO_DEG_COEF		57.29578f	// 180.0f / M_PI

//Byte_t dummy[DLEN] = {0x96, /*0xE2, 0xC0, 0x31*/}; // 10010110 11100010
//Byte_t read[DLEN];

void timerInit(uint32_t freqHz);
void TIMER1_IRQHandler(void);
void getFilteredAcc(void);
void getFilteredRate(void);
int32_t removeAccNoise(short accSample);
int32_t removeGyroNoise(short gyroSample);
void getPosition(void);
void getPosition2(void);
void getAngle(void);
void checkIfMotionEnded(void);
uint8_t tick;

typedef struct{

	int32_t x[ACC_SAMPLES_HISTORY_LEN];
	int32_t y[ACC_SAMPLES_HISTORY_LEN];
	int32_t z[ACC_SAMPLES_HISTORY_LEN];
} Acc_t;

typedef struct{

	int32_t x[ACC_SAMPLES_HISTORY_LEN];
	int32_t y[ACC_SAMPLES_HISTORY_LEN];
	int32_t z[ACC_SAMPLES_HISTORY_LEN];
} Vel_t;

typedef struct{

	int32_t x[ACC_SAMPLES_HISTORY_LEN];
	int32_t y[ACC_SAMPLES_HISTORY_LEN];
	int32_t z[ACC_SAMPLES_HISTORY_LEN];
} Pos_t;

typedef struct{

	int32_t x[GYRO_SAMPLES_HISTORY_LEN];
	int32_t y[GYRO_SAMPLES_HISTORY_LEN];
	int32_t z[GYRO_SAMPLES_HISTORY_LEN];
}Rate_t;

typedef struct{

	int32_t x[GYRO_SAMPLES_HISTORY_LEN];
	int32_t y[GYRO_SAMPLES_HISTORY_LEN];
	int32_t z[GYRO_SAMPLES_HISTORY_LEN];
}Ang_t;

typedef struct{

	float x;
	float y;
	float z;
} PosF_t;

typedef struct{

	float x;
	float y;
	float z;
} VelF_t;

typedef struct{

	float x;
	float y;
	float z;
} AngF_t;

Acc_t acc;
Vel_t vel;
Pos_t pos;
VelF_t velF;
PosF_t posF;
AngF_t angF, angF2;
Rate_t rate;
Ang_t ang;
float velRatio;
float posRatio;
float angRatio;

int main(void){

	float gx = 0.0f, gy = 0.0f, gz = 0.0f, temp;
	char initSuccess;
	unsigned char dataBuffer[BUFF_LEN] = {129};
	int tickCnt = 0;
	float rawAccToG = 0.0f;
	uint8_t sampleFreqInHz = 1u;
	L3G4200D_Fullscale_t l3g4200dFullScale = L3G4200D_FULLSCALE_250;

	status_t gyroStatus;
	AxesRaw_t axesRaw;

	//LEDInit();
	initSuccess = ADXL345_Init(ADXL345_SPI_COMM);
	ADXL345_SetPowerMode(ADXL345_MEASURE_MODE);
	timerInit(sampleFreqInHz);

	rawAccToG = ADXL345_RawAccToGRatio();
	velRatio = rawAccToG * VEL_UNITS_RATIO(sampleFreqInHz);
	posRatio = rawAccToG * POS_UNITS_RATIO(sampleFreqInHz);
	//ADXL345_SetOffset(0, 0, 150);

	gyroStatus = L3G4200D_Init();
	gyroStatus = L3G4200D_SetODR(L3G4200D_ODR_95Hz_BW_25);
	gyroStatus = L3G4200D_SetMode(L3G4200D_NORMAL);
	gyroStatus = L3G4200D_SetFullScale(l3g4200dFullScale);
	gyroStatus = L3G4200D_SetAxis(L3G4200D_Z_ENABLE | L3G4200D_X_ENABLE | L3G4200D_Y_ENABLE);

	angRatio = L3G4200D_GetSensivityDPS(l3g4200dFullScale) * ANG_UNITS_RATIO(sampleFreqInHz);

	while(1){

		if(tick){

			//getFilteredAcc();
			getFilteredRate();
			//getPosition();
			getPosition2();
			//LEDToggle();
			getAngle();

			ADXL345_GetGxyz(&gx, &gy, &gz);

			tick = 0u;
//			++tickCnt;
//
//			if(100 == tickCnt){
//
//				//LEDToggle();
//				tickCnt = 0;
//			}
		}

		gyroStatus = L3G4200D_GetAngRateRaw(&axesRaw);
	}

	return 0;
}

void getFilteredAcc(void){

	short accX = 0u;
	short accY = 0u;
	short accZ = 0u;
	uint8_t sampleCnt = 0u;

	acc.x[1] = 0;
	acc.y[1] = 0;
	acc.z[1] = 0;

	for(sampleCnt = 0u; sampleCnt < ACC_SAMPLES_PER_MEASURE; ++sampleCnt){

		ADXL345_GetXyz(&accX, &accY, &accZ);

		acc.x[1] += accX;
		acc.y[1] += accY;
		acc.z[1] += accZ;
	}

	acc.x[1] /= ACC_SAMPLES_PER_MEASURE;
	acc.y[1] /= ACC_SAMPLES_PER_MEASURE;
	acc.z[1] /= ACC_SAMPLES_PER_MEASURE;

	acc.x[1] = removeAccNoise( acc.x[1] );
	acc.y[1] = removeAccNoise( acc.y[1] );
	acc.z[1] = removeAccNoise( acc.z[1] );

//	acc.x[1] = 1;
//	acc.y[1] = 1;
//	acc.z[1] = 1;
}

void getFilteredRate(void){

	AxesRaw_t axesRaw;
	uint8_t sampleCnt = 0u;

	rate.x[1] = 0;
	rate.y[1] = 0;
	rate.z[1] = 0;

	for(sampleCnt = 0u; sampleCnt < GYRO_SAMPLES_PER_MEASURE; ++sampleCnt){

		L3G4200D_GetAngRateRaw(&axesRaw);

		rate.x[1] += axesRaw.AXIS_X;
		rate.y[1] += axesRaw.AXIS_Y;
		rate.z[1] += axesRaw.AXIS_Z;
	}

	rate.x[1] /= GYRO_SAMPLES_PER_MEASURE;
	rate.y[1] /= GYRO_SAMPLES_PER_MEASURE;
	rate.z[1] /= GYRO_SAMPLES_PER_MEASURE;

	rate.x[1] = removeGyroNoise( rate.x[1] );
	rate.y[1] = removeGyroNoise( rate.y[1] );
	rate.z[1] = removeGyroNoise( rate.z[1] );
}

void getPosition(void){

	getFilteredAcc();

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

//	ADXL345_RawAccToG(pos.x[1], &posF.x);
//	ADXL345_RawAccToG(pos.y[1], &posF.y);

	posF.x = posRatio * pos.x[1];
	posF.y = posRatio * pos.y[1];
	velF.x = velRatio * vel.x[1];
	velF.y = velRatio * vel.y[1];
}

uint8_t zeroAccXCnt;
uint8_t zeroAccYCnt;

void checkIfMotionEnded(void){

	if (0 == acc.x[1]){

		++zeroAccXCnt;
	}
	else {

		zeroAccXCnt = 0u;
	}

	if (zeroAccXCnt >= 25u){

		vel.x[1] = 0;
		vel.x[0] = 0;
	}

	if (0 == acc.y[1]){

		++zeroAccYCnt;
	}
	else {

		zeroAccYCnt = 0u;
	}

	if (zeroAccYCnt >= 25u){

		vel.y[1] = 0;
		vel.y[0] = 0;
	}
}

void getPosition2(void){

	getFilteredAcc();

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

void getAngle(void){

	getFilteredRate();

	ang.x[1] = ang.x[0] + rate.x[0] + ( (rate.x[1] - rate.x[0]) / 2);

	ang.y[1] = ang.y[0] + rate.y[0] + ( (rate.y[1] - rate.y[0]) / 2);

	rate.x[0] = rate.x[1];
	rate.y[0] = rate.y[1];

	//TODO: equivalent ???
	//checkIfMotionEnded();

	ang.x[0] = ang.x[1];
	ang.y[0] = ang.y[1];

	angF.x = angRatio * ang.x[1];
	angF.y = angRatio * ang.y[1];

	getFilteredAcc();

	angF2.x = atan2( acc.y[1], acc.z[1] ) * RADIANS_TO_DEG_COEF;
	angF2.y = atan2(-acc.x[1], hypot(acc.y[1], acc.z[1]) ) * RADIANS_TO_DEG_COEF;
}

int32_t removeAccNoise(short accSample){

	int32_t noNoise = accSample;

	if(accSample < ACC_NOISE_TRESHOLD && accSample > -ACC_NOISE_TRESHOLD){

		noNoise = 0;
	}

	return noNoise;
}

int32_t removeGyroNoise(short gyroSample){

	int32_t noNoise = gyroSample;

	if(gyroSample < GYRO_NOISE_TRESHOLD && gyroSample > -GYRO_NOISE_TRESHOLD){

		noNoise = 0;
	}

	return noNoise;
}

void timerInit(uint32_t freqHz){

	uint32_t tmPeriphClk;
	TIM_TIMERCFG_Type tmCfg;
	TIM_MATCHCFG_Type tmMatchCfg;

	tmPeriphClk = CLKPWR_GetPCLK (CLKPWR_PCLKSEL_TIMER1);

	tmCfg.PrescaleOption = TIM_PRESCALE_TICKVAL;
	tmCfg.PrescaleValue = tmPeriphClk / 2u / freqHz;

	TIM_Init(LPC_TIM1, TIM_TIMER_MODE, &tmCfg);

	tmMatchCfg.MatchChannel = 0u;//MATCH_CHANNEL;
	tmMatchCfg.IntOnMatch = ENABLE;
	tmMatchCfg.StopOnMatch = DISABLE;
	tmMatchCfg.ResetOnMatch = ENABLE;
	tmMatchCfg.ExtMatchOutputType = TIM_EXTMATCH_NOTHING;
	tmMatchCfg.MatchValue = 1u;

	TIM_ConfigMatch(LPC_TIM1, &tmMatchCfg);

	NVIC_SetPriority(TIMER1_IRQn, TIMER_PRIORITY);

	NVIC_EnableIRQ(TIMER1_IRQn);

	TIM_Cmd(LPC_TIM1, ENABLE);
}

void TIMER1_IRQHandler(void){

	tick = 1u;

	NVIC_ClearPendingIRQ(TIMER1_IRQn);

	TIM_ClearIntPending(LPC_TIM1, TIM_MR0_INT);
}
