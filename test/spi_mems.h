#ifndef SPI_MEMS_H

#define SPI_MEMS_H

#include "l3g4200d_driver.h"

u8_t SPI_Mems_Init(void);
u8_t SPI_Mems_Write_Reg(u8_t Reg, u8_t Data);
u8_t SPI_Mems_Read_Reg(u8_t Reg );

#endif
