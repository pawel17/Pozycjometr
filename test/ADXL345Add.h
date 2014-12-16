#ifndef ADXL345_ADD_H
#define ADXL345_ADD_H

#include "ADXL345.h"

#define ADXL345_STANDBY_MODE	0x0
#define ADXL345_MEASURE_MODE	0x1

void ADXL345_RawAccToG(short rawAccData, float * pGAcc);
float ADXL345_RawAccToGRatio(void);

#endif
