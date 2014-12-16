#include "ADXL345Add.h"

extern char fullResolutionSet;
extern char selectedRange;

void ADXL345_RawAccToG(short rawAccData, float * pGAcc){

	*pGAcc = (float)(fullResolutionSet ? (rawAccData * ADXL345_SCALE_FACTOR) : (rawAccData * ADXL345_SCALE_FACTOR * (selectedRange >> 1)));
}

float ADXL345_RawAccToGRatio(void){

	return (float)(fullResolutionSet ? (ADXL345_SCALE_FACTOR) : (ADXL345_SCALE_FACTOR * (selectedRange >> 1)));
}
