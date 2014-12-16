#ifndef SPIHAL_SSP_H

#define SPIHAL_SSP_H

#include "ADXL345.h"

#define L3G4200D_SLAVE_ID	2


/*!< Initializes the SPI communication peripheral. */
unsigned char SPI_HAL_Init(unsigned char lsbFirst,
                       unsigned long clockFreq,
                       unsigned char clockPol,
                       unsigned char clockEdg);

/*!< Writes data to SPI. */
unsigned char SPI_HAL_Write(unsigned char  slaveDeviceId,
                        unsigned char* data,
                        unsigned char  bytesNumber);

/*!< Reads data from SPI. */
unsigned char SPI_HAL_Read(unsigned char  slaveDeviceId,
                       unsigned char* data,
                       unsigned char  bytesNumber);

#endif
