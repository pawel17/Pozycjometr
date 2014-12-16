#include "LPC17xx_utils.h"

SPI_CFG_Type SPI_CFG_Make(uint32_t databit, 
													uint32_t CPHA, 
													uint32_t CPOL, 
														uint32_t mode, 
														uint32_t dataOrder, 
														uint32_t clockRate){

	SPI_CFG_Type spiConfig;
	
	spiConfig.Databit = databit;
	spiConfig.CPHA = CPHA;
	spiConfig.CPOL = CPOL;
	spiConfig.Mode = mode;
	spiConfig.DataOrder = dataOrder;
	spiConfig.ClockRate = clockRate;
	
	return spiConfig;
}

SSP_CFG_Type SSP_CFG_Make(uint32_t databit, 
													uint32_t CPHA, 
													uint32_t CPOL, 
													uint32_t mode, 
													uint32_t frameFormat, 
													uint32_t clockRate){
														
  SSP_CFG_Type sspCfg;
	
	sspCfg.Databit = databit;
	sspCfg.CPHA = CPHA;
	sspCfg.CPOL = CPOL;
	sspCfg.Mode = mode;
	sspCfg.FrameFormat = frameFormat;
	sspCfg.ClockRate = clockRate;
	
	return sspCfg;
}

PINSEL_CFG_Type PINSEL_CFG_Make(	uint8_t portNum, 
																	uint8_t pinNum, 
																	uint8_t funcNum, 
																	uint8_t pinMode, 
																	uint8_t openDrain){

	PINSEL_CFG_Type pinselConfig;
	
	pinselConfig.Portnum = portNum;
	pinselConfig.Pinnum = pinNum;
	pinselConfig.Funcnum = funcNum;
	pinselConfig.Pinmode = pinMode;
	pinselConfig.OpenDrain = openDrain;
	
	return pinselConfig;
}

SPI_DATA_SETUP_Type SPI_DATA_SETUP_Make(void * txData, 
																				void * rxData, 
																				uint32_t length, 
																				uint32_t counter, 
																				uint32_t status){
																					
  SPI_DATA_SETUP_Type spiData;
	
	spiData.tx_data = txData;
	spiData.rx_data = rxData;
	spiData.length = length;
	spiData.counter = counter;
	spiData.status = status;
	
	return spiData;
}

SSP_DATA_SETUP_Type SSP_DATA_SETUP_Make(void *txData,
																				uint32_t txCnt,
																				void *rxData,
																				uint32_t rxCnt,
																				uint32_t length,
																				uint32_t status){
																					
  SSP_DATA_SETUP_Type sspData;
	
	sspData.tx_data = txData;
	sspData.tx_cnt = txCnt;
	sspData.rx_data = rxData;
	sspData.rx_cnt = rxCnt;
	sspData.length = length;
	sspData.status = status;
	
	return sspData;
}
