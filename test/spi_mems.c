#include "spi_mems.h"
#include "spihal_ssp.h"

#define SENT_DATA_LENGTH	2

u8_t SPI_Mems_Init(void){

	return SPI_HAL_Init(0,        // Transfer format.
	                          1000000,  // SPI clock frequency.
	                          1,        // SPI clock polarity.
	                          0);       // SPI clock edge.
}

u8_t SPI_Mems_Write_Reg(u8_t Reg, u8_t Data){

	u8_t dataToSend[SENT_DATA_LENGTH];

	dataToSend[0] = Reg;
	dataToSend[1] = Data;

	return SPI_HAL_Write(L3G4200D_SLAVE_ID, dataToSend, SENT_DATA_LENGTH);
}

u8_t SPI_Mems_Read_Reg(u8_t Reg ){

	u8_t dataToSend[SENT_DATA_LENGTH];

	dataToSend[0] = Reg;
	dataToSend[1] = 0;

	SPI_HAL_Read(L3G4200D_SLAVE_ID, dataToSend, SENT_DATA_LENGTH);

	return dataToSend[1];
}
