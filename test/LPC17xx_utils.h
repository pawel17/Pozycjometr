#ifndef LPC17XX_UTILS_H

#define LPC17XX_UTILS_H

//#include "lpc17xx_clkpwr.h"
#include "lpc17xx_pinsel.h"
//#include "lpc17xx_gpio.h"
#include "lpc17xx_spi.h"
#include "lpc17xx_ssp.h"

SPI_CFG_Type SPI_CFG_Make(uint32_t databit, 
													uint32_t CPHA, 
													uint32_t CPOL, 
														uint32_t mode, 
														uint32_t dataOrder, 
														uint32_t clockRate);
SSP_CFG_Type SSP_CFG_Make(uint32_t databit, 
													uint32_t CPHA, 
													uint32_t CPOL, 
													uint32_t mode, 
													uint32_t frameFormat, 
													uint32_t clockRate);
PINSEL_CFG_Type PINSEL_CFG_Make(	uint8_t portNum, 
																	uint8_t pinNum, 
																	uint8_t funcNum, 
																	uint8_t pinMode, 
																	uint8_t openDrain);
SPI_DATA_SETUP_Type SPI_DATA_SETUP_Make(void * txData, 
																				void * rxData, 
																				uint32_t length, 
																				uint32_t counter, 
																				uint32_t status);
SSP_DATA_SETUP_Type SSP_DATA_SETUP_Make(void *tx_data,
																				uint32_t tx_cnt,
																				void *rx_data,
																				uint32_t rx_cnt,
																				uint32_t length,
																				uint32_t status);

#endif
