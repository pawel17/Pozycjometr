#ifndef I2CHAL_H

#define I2CHAL_H

#include <stdint.h>

uint8_t I2C_HAL_Init(uint32_t frequency);
uint8_t I2C_HAL_Write(uint8_t slaveAddress, uint8_t * pWriteData, uint8_t bytesNum, uint8_t stopConditionCtrl);
uint8_t I2C_HAL_Read(uint8_t slaveAddress, uint8_t * pReceiveBuff, uint8_t bytesNum, uint8_t stopConditionCtrl); 

#endif
