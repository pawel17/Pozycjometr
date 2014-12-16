#include "L3G4200D.h"
#include "spi_mems.h"

#define L3G4200D_WHO_AM_I	0x0F

status_t L3G4200D_Init(void){

	status_t initSuccess = MEMS_SUCCESS;
	u8_t receivedByte = 0;

	SPI_Mems_Init();

	L3G4200D_ReadReg(L3G4200D_WHO_AM_I, &receivedByte);

	if( receivedByte != L3G4200D_I_AM_L3G4200D ){

		initSuccess = MEMS_ERROR;
	}

	return initSuccess;
}

void L3G4200D_GetAngRateShort(short * pXRate, short * pYRate, short * pZRate){

	AxesRaw_t axesRaw;

	L3G4200D_GetAngRateRaw(&axesRaw);

	*pXRate = axesRaw.AXIS_X;
	*pYRate = axesRaw.AXIS_Y;
	*pZRate = axesRaw.AXIS_Z;
}

float L3G4200D_GetSensivityDPS(L3G4200D_Fullscale_t fullscale){

	float sensivityInDegPerSec = 0.0f;

	switch(fullscale){

		case L3G4200D_FULLSCALE_250:

			sensivityInDegPerSec = 8.75e-3f;
			break;

		case L3G4200D_FULLSCALE_500:

			sensivityInDegPerSec = 17.5e-3f;
			break;

		case L3G4200D_FULLSCALE_2000:

			sensivityInDegPerSec = 70.0e-3f;
			break;
	}

	return sensivityInDegPerSec;
}
