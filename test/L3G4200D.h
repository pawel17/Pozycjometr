#include "l3g4200d_driver.h"

status_t L3G4200D_Init(void);
void L3G4200D_GetAngRateShort(short * pXRate, short * pYRate, short * pZRate);
float L3G4200D_GetSensivityDPS(L3G4200D_Fullscale_t fullscale);
