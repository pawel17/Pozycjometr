#include <limits.h>
#include <string.h>
#include "spihal_ssp.h"
#include "lpc17xx_clkpwr.h"
#include "lpc17xx_pinsel.h"
#include "lpc17xx_gpio.h"
#include "lpc17xx_ssp.h"
#include "lpc17xx_utils.h"

#define SPI_PORT_NUM				PINSEL_PORT_0
#define MISO_PIN_NUM				PINSEL_PIN_8
#define MOSI_PIN_NUM				PINSEL_PIN_9
#define SCLK_PIN_NUM				PINSEL_PIN_7
#define CS_ADXL362_PIN_NUM	PINSEL_PIN_24
#define CS_L3G4200D_PIN_NUM	PINSEL_PIN_23
#define OUTPUT							1u
#define PINS_NOT_MASKED			0u
#define GPIO_FUNC_NUM				PINSEL_FUNC_0
#define BUFFER_LENGTH				32u
#define SPI_STATUS_RESET		0u
//#define inline _inline

static void CSIdle(unsigned char slaveDeviceId);
static void CSActive(unsigned char slaveDeviceId);
static unsigned char SSPReadWriteStatusToUChar(int32_t readWriteStatus);

static inline uint32_t bitMaskWithBitPosition(uint8_t bitPos){
	
	return (uint32_t)1u << bitPos;
}

static void initPin(uint8_t pinNum,
										uint8_t funcNum,
										uint8_t pinMode, 
										uint8_t openDrain);
static void initGPIOPinForCS(	uint8_t pinNum, 
															uint8_t pinMode, 
															uint8_t openDrain);
static void initPins(void);

//Using legacy SPI peripheral for SPI communication
unsigned char SPI_HAL_Init(unsigned char lsbFirst,
                       unsigned long clockFreq,
                       unsigned char clockPol,
                       unsigned char clockEdg){
		
		static uint8_t initialized = 0u;
		
		if(!initialized){

			SSP_CFG_Type sspConfig;

			//!!! in SSP MSB is always first
			//TODO: check if lsbFirst == 0 means MSB first !!!
			//uint8_t bitOrder = (0u == lsbFirst) ? SPI_DATA_MSB_FIRST : SPI_DATA_LSB_FIRST;
			//TODO: check if clockPol == 0 means SCLK idle state == low !!!
			uint8_t polarity = (0u == clockPol) ? SSP_CPOL_HI : SSP_CPOL_LO;
			//TODO: check if clockEdg == 0 means sampling on second edge !!!
			uint8_t phase = (0u == clockEdg) ? SSP_CPHA_SECOND : SSP_CPHA_FIRST;

			CLKPWR_ConfigPPWR (CLKPWR_PCONP_PCSSP1, ENABLE);

			CLKPWR_SetPCLKDiv (CLKPWR_PCLKSEL_SSP1, CLKPWR_PCLKSEL_CCLK_DIV_4);

			initPins();

			//TODO:
			//verify if Clock rate doesn't not exceed (SPI peripheral clock)/8 !!!
			sspConfig = SSP_CFG_Make(SSP_DATABIT_8, phase, polarity, SSP_MASTER_MODE, SSP_FRAME_SPI, clockFreq);

			SSP_Init(LPC_SSP1, &sspConfig);

			SSP_Cmd(LPC_SSP1, ENABLE);
			//SSP_LoopBackCmd(LPC_SSP1, ENABLE);

			initialized = 1u;
		}
		
		return 0u;
}

unsigned char SPI_HAL_Write(unsigned char  slaveDeviceId,
                        unsigned char* data,
                        unsigned char  bytesNumber){

	//static uint8_t receivedData[BUFFER_LENGTH];
	//static uint8_t transmittedData[BUFFER_LENGTH];
  static SSP_DATA_SETUP_Type SSPData;
	int32_t writeResult;

	//clear SPIData before write operation ???
  SSPData = SSP_DATA_SETUP_Make(data, 0u, NULL, 0u, bytesNumber, SPI_STATUS_RESET);

  CSActive(slaveDeviceId);
	
	writeResult = SSP_ReadWrite(LPC_SSP1, &SSPData, SSP_TRANSFER_POLLING);
	
	CSIdle(slaveDeviceId);
	
	return SSPReadWriteStatusToUChar(writeResult);
}

// why not const unsigned char * data ???
unsigned char SPI_HAL_Read(unsigned char  slaveDeviceId,
                       unsigned char* data,
                       unsigned char  bytesNumber){
	
  static uint8_t receivedData[BUFFER_LENGTH];
	//static uint8_t transmittedData[BUFFER_LENGTH];
  static SSP_DATA_SETUP_Type SSPData;
	int32_t readResult;

  //clear SPIData before read operation ???
  SSPData = SSP_DATA_SETUP_Make(data, 0u, receivedData, 0u, bytesNumber, SPI_STATUS_RESET);

  CSActive(slaveDeviceId);
	
	readResult = SSP_ReadWrite(LPC_SSP1, &SSPData, SSP_TRANSFER_POLLING);
	
	CSIdle(slaveDeviceId);

//TODO: check if ths copy is neccessary
  memcpy(data, receivedData, bytesNumber);

	return SSPReadWriteStatusToUChar(readResult);
}

static unsigned char SSPReadWriteStatusToUChar(int32_t readWriteStatus){
	
	unsigned char convertedStatus = UCHAR_MAX;
	
	if(readWriteStatus >= 0 && readWriteStatus <= UCHAR_MAX){
		
		convertedStatus = readWriteStatus;
	}
	
	return convertedStatus;
}

static void initPin(uint8_t pinNum, uint8_t funcNum, uint8_t pinMode, uint8_t openDrain){
	
	PINSEL_CFG_Type pinCfg = PINSEL_CFG_Make(SPI_PORT_NUM, pinNum, funcNum, pinMode, openDrain);
	
	PINSEL_ConfigPin(&pinCfg);
}

static void initGPIOPinForCS(uint8_t pinNum, uint8_t pinMode, uint8_t openDrain){
	
	initPin(pinNum, GPIO_FUNC_NUM, pinMode, openDrain);
	
	GPIO_SetDir(SPI_PORT_NUM, bitMaskWithBitPosition(pinNum), OUTPUT);
}

static void initPins(void){
	
	uint32_t sclkBitMask = bitMaskWithBitPosition(SCLK_PIN_NUM);
	uint32_t bitsNotToMask =	sclkBitMask | bitMaskWithBitPosition(CS_ADXL362_PIN_NUM) | bitMaskWithBitPosition(CS_L3G4200D_PIN_NUM);
	
	initPin(SCLK_PIN_NUM, PINSEL_FUNC_2, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL);
	initPin(MOSI_PIN_NUM, PINSEL_FUNC_2, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL);
	initPin(MISO_PIN_NUM, PINSEL_FUNC_2, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL);
	
	initGPIOPinForCS(CS_ADXL362_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL);
	initGPIOPinForCS(CS_L3G4200D_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL);
	
	FIO_SetMask(SPI_PORT_NUM, bitsNotToMask, PINS_NOT_MASKED);
	
	GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_ADXL362_PIN_NUM) );
	GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_L3G4200D_PIN_NUM) );
}

static void CSIdle(unsigned char slaveDeviceId){
	
	if(ADXL345_SLAVE_ID == slaveDeviceId){
		
		GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_ADXL362_PIN_NUM) );
	}
	else if(L3G4200D_SLAVE_ID == slaveDeviceId){
		
		GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_L3G4200D_PIN_NUM) );
	}
}

static void CSActive(unsigned char slaveDeviceId){
	
	if(ADXL345_SLAVE_ID == slaveDeviceId){
		
		GPIO_ClearValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_ADXL362_PIN_NUM) );
	}
	else if(L3G4200D_SLAVE_ID == slaveDeviceId){
		
		GPIO_ClearValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS_L3G4200D_PIN_NUM) );
	}
}
