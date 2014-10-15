//#include <string.h>
#include <lpc17xx_clkpwr.h>
#include <lpc17xx_gpio.h>
#include <lpc17xx_pinsel.h>
#include <lpc17xx_timer.h>
#include <core_cm3.h>
#include <limits.h>
#include "Spi.h"

//#include <stdio.h>

//wiecej na temat znaczenia tych stalych w dokumentacji do LPC1768
//indeks timera
#define TIMER_INDEX					2u
//priorytet przerwania od timera 2
#define TIMER_2_PRIORITY		30u
//numer portu GPIO ktorego 4 piny sluza jako SPI
#define SPI_PORT_NUM				PINSEL_PORT_2
//indeks pinu (w porcie sluzacym jako SPI) GPIO sluzacego jako CS1
#define CS1_PIN_NUM					PINSEL_PIN_0
//indeks pinu (w porcie sluzacym jako SPI) GPIO sluzacego jako CS2
#define CS2_PIN_NUM					PINSEL_PIN_1
//indeks pinu (w porcie sluzacym jako SPI) GPIO sluzacego jako MOSI
#define MOSI_PIN_NUM				PINSEL_PIN_2
//indeks pinu (w porcie sluzacym jako SPI) GPIO sluzacego jako MISO
#define MISO_PIN_NUM				PINSEL_PIN_3
//indeks pinu (w porcie sluzacym jako SPI) GPIO sluzacego jako SCLK (zegar)
#define SCLK_PIN_NUM				PINSEL_PIN_4
//stala oznaczajaca tryb wyjsciowy pracy pinu
#define OUTPUT							1u
//stala oznaczajaca tryb wejsciowy pracy pinu
#define INPUT								0u
#define RISING_EDGE					0u
#define FALLING_EDGE				1u
//numer trybu GPIO (wiecej w dokumentacji)
#define GPIO_FUNC_NUM				PINSEL_FUNC_0
//flaga pinu maskowanego (wiecej w dokumentacji)
#define PINS_MASKED					1u
//flaga pinu niemaskowanego (wiecej w dokumentacji)
#define PINS_NOT_MASKED			0u
#define inline							__inline

/**
	@brief stan timera SPI
	@param SCLK_START stan startu - przez jeden cykl zegara, zegar jest w stanie nieaktywnym (przed SCLK_IN_PROGRESS)
	@param SCLK_IN_PROGRESS zegar co pol cyklu zmienia stan
	@param SCLK_STOP stan stop - przez jeden cykl zegara, zegar jest w stanie nieaktywnym (po SCLK_IN_PROGRESS)
**/
static enum{
	
	SCLK_START,
	SCLK_IN_PROGRESS,
	SCLK_STOP
} sclkState = SCLK_START;

static uint8_t stopSclk;
static uint8_t timerInt;
/**
		@brief funkcja zwraca maske bitowa dla bitu o zadanym indeksie
		@param bitPos pozycja bitu na ktorym ma byc 1
		@return maska z 1 na pozycji bitpos i 0 na pozostalych pozycjach
		**/
static inline uint32_t bitMaskWithBitPosition(uint8_t bitPos){
	
	return (uint32_t)1u << bitPos;
}
/**
		@brief funkcja zwraca wartosc bitu na zadanej pozycji w wartosci bedacej drugim argumentem
		@param value wartosc, ktorej bit nalezy odczytac
		@param bitPosition indeks bitu do odczytu
		@return wartosc bitu (1 lub 0) na zadanej pozycji w wartosci value
		**/
static inline uint8_t bitValueOnPosition(uint32_t value, uint8_t bitPosition){
	
	return (value >> bitPosition) & 1u; 
}
/**
		@brief funkcja inicjalizuje zegar SPI
		@param pSpiData struktura opisujaca parametry SPI
		@return sukces (1 - tak, 0 - nie)
		**/
static uint8_t initSclk(const Spi_t * pSpiData);
/**
		@brief funkcja inicjalizuje piny GPIO uzywane do programowej emulacji SPI
		@param pSpiData struktura opisujaca parametry SPI
		**/
static void initPins(const Spi_t * pSpiData);
/**
		@brief funkcja inicjalizuje pin GPIO uzywany jako jeden z pinow SPI
		@param pinNum numer pinu
		@param pinMode tryb pracy pinu
		@param openDrain czy otwarty dren (tu nie jestem pewien)
		@param pinDir czy pin pracuje jako wejscie czy jako wyjscie
		**/
static void initPin(uint8_t pinNum, uint8_t pinMode, uint8_t openDrain, uint8_t pinDir);
/**
		@brief funkcja wysyla bajt poprzez SPI
		@param toWrite bajt do wyslania
		**/
static void spiWriteByte(uint8_t toWrite);
/**
		@brief funkcja startuje zegar SPI
		**/
static void sclkStart(void);
/**
		@brief funkcja zatrzymuje zegar SPI
		**/
static void sclkStop(void);
/**
		@brief funkcja zmienia wartosc sygnalu zegara SPI na przeciwny
		**/
static void toggleSclk(void);
/**
		@brief funkcja pobiera okreslony bit liczac od najstarszego bitu
		@param pData tablica, ktorej bit nalezy odczytac
		@param bytesNum rozmiar tablicy pData
		@param bitNum indeks bitu do odczytu (liczac od najstarszego bitu)
		@return wartosc bitu (1 lub 0) na zaden pozycji
		**/
static uint8_t getBitCountingFromMsb(const Byte_t * pData, uint8_t bytesNum, uint16_t bitNum);
/**
		@brief funkcja pobiera okreslony bit liczac od najmlodszego bitu
		@param pData tablica, ktorej bit nalezy odczytac
		@param bytesNum rozmiar tablicy pData
		@param bitNum indeks bitu do odczytu (liczac od najmlodszego bitu)
		@return wartosc bitu (1 lub 0) na zaden pozycji
		**/
static uint8_t getBitCountingFromLsb(const Byte_t * pData, uint8_t bytesNum, uint16_t bitNum);
/**
		@brief funkcja ustawia okreslony bit liczac od najstarszego bitu
		@param pData tablica, ktorej bit nalezy ustawic
		@param bytesNum rozmiar tablicy pData
		@param bitNum indeks bitu do zapisu (liczac od najstarszego bitu)
		@param newValue nowa wartosc bitu (1 lub 0) na zaden pozycji
		**/
static void setBitCountingFromMsb(Byte_t * pData, uint8_t bytesNum, uint16_t bitNum, uint8_t newValue);
/**
		@brief funkcja ustawia okreslony bit liczac od najmlodszego bitu
		@param pData tablica, ktorej bit nalezy ustawic
		@param bytesNum rozmiar tablicy pData
		@param bitNum indeks bitu do zapisu (liczac od najmlodszego bitu)
		@param newValue nowa wartosc bitu (1 lub 0) na zaden pozycji
		**/
static void setBitCountingFromLsb(Byte_t * pData, uint8_t bytesNum, uint16_t bitNum, uint8_t newValue);
/**
		@brief funkcja ustawia bit MOSI
		@param outValue zadana wartosc bitu (1 lub 0)
		**/
static void setMosi(uint8_t outValue);
/**
		@brief funkcja pobiera bit MISO
		@return aktualna wartosc bitu MISO (1 lub 0)
		**/
static uint8_t getMiso(void);
/**
		@brief funkcja pobiera wartosc sygnalu zegarowe SPI
		@return aktualna wartosc sygnalu zegarowego (1 lub 0)
		**/
static uint8_t sclkValue(void);
/**
		@brief funkcja informuje, czy zegar SPI jest w stanie nieaktywnym (bezczynnosci - idle)
		@return flaga mowiaca, czy sygnal zegara jest w stanie aktywnym (1 - tak lub 0 - nie)
		**/
static uint8_t sclkIdle(const Spi_t * pSpiData);
/**
		@brief funkcja ustawia bit CS w stan aktywny
		**/
static inline void csActive(void){
		
	GPIO_ClearValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS1_PIN_NUM) );
}
/**
		@brief funkcja ustawia bit CS w stan nieaktywny
		**/
static inline void csIdle(void){
	
	GPIO_SetValue(SPI_PORT_NUM, bitMaskWithBitPosition(CS1_PIN_NUM) );
}
/**
		@brief funkcja obslugi przerwania timera 2
		**/
void TIMER2_IRQHandler(void);

/**
	@param timerInt flaga mowiaca, czy timer wygenerowal przerwanie
**/
static uint8_t timerInt;

uint8_t spiInit(Spi_t * pSpiData, uint32_t clkFreq, ClkPol_t sclkIdleState, ClkPh_t clkPh, BOrder_t bitOrd){
	
	uint8_t initSuccess = 10;
	
	pSpiData->clkFreq_ = clkFreq;
	pSpiData->sclkIdleState_ = sclkIdleState;
	pSpiData->clkPh_ = clkPh;
	pSpiData->bitOrd_ = bitOrd;
	pSpiData->dataWritten_ = 0u;
	
	if(IDLE_LOW == sclkIdleState){
		
		pSpiData->prevSclkState_ = 1u;
	}
	else{
		
		pSpiData->prevSclkState_ = 0u;
	}
	
	if(MSB_FIRST == bitOrd){
		
		pSpiData->fpGetBit = getBitCountingFromMsb;
		
		pSpiData->fpSetBit = setBitCountingFromMsb;
	}
	else{
		
		pSpiData->fpGetBit = getBitCountingFromLsb;
		
		pSpiData->fpSetBit = setBitCountingFromLsb;
	}
	
	initSclk(pSpiData);
	
	return initSuccess;
}

uint8_t spiWrite(const Spi_t * pSpiData, const Byte_t * pData, uint8_t dataLen){
	
	uint16_t bitsCnt = 0u;
	uint16_t bitsNum = dataLen * sizeof(*pData) * CHAR_BIT;
	
	if(dataLen > 0u){
		
		csActive();
	
		sclkStart();
	}

	while(bitsCnt < bitsNum){
		
		if(1u == timerInt){
			
			if(1u == stopSclk){
				
				sclkState = SCLK_STOP;
				
				stopSclk = 0u;
			}
			else{
				extern uint32_t cntr;
				uint8_t bitValueToWrite = pSpiData->fpGetBit(pData, dataLen, bitsCnt);
				uint8_t wasSclkIdle = sclkIdle(pSpiData);
				uint8_t isSclkActive;
				
				toggleSclk();
				
				isSclkActive = !sclkIdle(pSpiData);
				
				if( wasSclkIdle && isSclkActive ){
					
					if(SAMPLE_ON_TRAILING == pSpiData->clkPh_){
						
						++bitsCnt;
						
						setMosi(bitValueToWrite);
					}
				}
				else{
					
					if(SAMPLE_ON_LEADING == pSpiData->clkPh_){
						
						++bitsCnt;
						
						setMosi(bitValueToWrite);
					}
				}
				
				++cntr;
			}
			
			timerInt = 0u;
		}
	}
	
	if(dataLen > 0u){
		
		sclkStop();
		
		csIdle();
	}
	
	return 0u;
}

uint8_t spiRead(const Spi_t * pSpiData, Byte_t * pDataBuffer, uint8_t dataLen){
	
	uint16_t bitsCnt = 0u;
	uint16_t bitsNum = dataLen * sizeof(*pDataBuffer) * CHAR_BIT;
	uint16_t bytesCnt = 0u;
	
	if(dataLen > 0u){
		
		csActive();
	
		sclkStart();
	}

	while(bitsCnt < bitsNum){
		
		if(1u == timerInt){
			
			if(1u == stopSclk){
				
				sclkState = SCLK_STOP;
				
				stopSclk = 0u;
			}
			else{
				extern uint32_t cntr;
				uint8_t wasSclkIdle = sclkIdle(pSpiData);
				uint8_t isSclkActive;
				
				toggleSclk();
				
				isSclkActive = !sclkIdle(pSpiData);
				
				if( wasSclkIdle && isSclkActive ){
					
					if(SAMPLE_ON_LEADING == pSpiData->clkPh_){
						//TODO: add setBit function (byte index, bit index in byte, value to set)
						bytesCnt = bitsCnt / CHAR_BIT;
						
						pSpiData->fpSetBit( pDataBuffer, dataLen, bitsCnt, getMiso() );
						
						++bitsCnt;
					}
				}
				else{
					
					if(SAMPLE_ON_TRAILING == pSpiData->clkPh_){
						
						bytesCnt = bitsCnt / CHAR_BIT;
						
						pSpiData->fpSetBit( pDataBuffer, dataLen, bitsCnt, getMiso() );
						
						++bitsCnt;
					}
				}
				
				++cntr;
			}
			
			timerInt = 0u;
		}
	}
	
	if(dataLen > 0u){
		
		sclkStop();
		
		csIdle();
	}
	
	return 0u;
}

static void spiWriteByte(uint8_t toWrite){
	
	
}

//static uint8_t sclkLeadingEdge(const Spi_t * pSpiData){
//	
//	uint8_t isSclkActive = !sclkIdle(pSpiData);
//}

static void toggleSclk(void){
	extern uint32_t cntr;
	uint32_t sclkBitMask = bitMaskWithBitPosition(SCLK_PIN_NUM);
	uint32_t sclkBitState = GPIO_ReadValue(SPI_PORT_NUM) & sclkBitMask;
	
	if(sclkBitState != 0u){
	//++cntr;	
		GPIO_ClearValue(SPI_PORT_NUM, sclkBitMask);
	}
	else{
		
		GPIO_SetValue(SPI_PORT_NUM, sclkBitMask);
	}
}

static uint8_t sclkValue(void){
	
	uint32_t spiPortState = GPIO_ReadValue(SPI_PORT_NUM);
	uint32_t maskedSpiPortState = spiPortState & bitMaskWithBitPosition(SCLK_PIN_NUM);
	
	return maskedSpiPortState >> SCLK_PIN_NUM;
}

static uint8_t sclkIdle(const Spi_t * pSpiData){
	
	uint8_t spiSclkState = sclkValue();
	uint8_t sclkValueOnIdle = 1u;
	
	if(IDLE_LOW == pSpiData->sclkIdleState_){
		
		sclkValueOnIdle = 0u;
	}
	
	return spiSclkState == sclkValueOnIdle;
}

/** Start SPI SCLK and wait for Css time;
**	because for ADXL362 and L3G4200D max
** Css is equal to 100 ns (= 10 MHz) and
** max SCLK frequency is equal to 5 MHz
** (1 - 5 MHz for ADXL362 and <= 10 MHz
** for L3G4200D), we can wait for one
** period of SCLK (SCLK = PCLK / PC)
*/

static void sclkStart(void){
	
	static uint8_t timIrqCntr = 0u;
	
	TIM_Cmd(LPC_TIM2, ENABLE);
	
	while(SCLK_START == sclkState){
		
		if(1u == timerInt){
			
			if(timIrqCntr < 2u){
				
				++timIrqCntr;
			}
			else{
				
				timIrqCntr = 0u;
				
				sclkState = SCLK_IN_PROGRESS;
			}
			
			timerInt = 0u;
		}
	}
}

static void sclkStop(void){
	
	static uint8_t timIrqCntr = 0u;
	
	while(SCLK_STOP == sclkState){
		
		if(1u == timerInt){
			
			if(timIrqCntr < 2u){
				
				++timIrqCntr;
			}
			else{
				
				timIrqCntr = 0u;
				
				sclkState = SCLK_START;
			}
			
			timerInt = 0u;
		}
	}
	
	TIM_Cmd(LPC_TIM2, DISABLE);
}

//static void handleTimer(void){
//	
//	static uint8_t timIrqCntr = 0u;
//	
//	switch(sclkState){
//		
//		case SCLK_START:
//			
//			if(timIrqCntr < 2u){
//				
//				++timIrqCntr;
//			}
//			else{
//				
//				sclkState = SCLK_IN_PROGRESS;
//				
//				timIrqCntr = 0u;
//			}
//			
//			break;
//			
//		case SCLK_IN_PROGRESS:
//				
//			if(1u == stopSclk){
//				
//				sclkState = SCLK_STOP;
//				
//				stopSclk = 0u;
//			}
//			else{
//				
//				toggleSclk();
//			}

//			break;
//			
//		case SCLK_STOP:
//			
//			if(timIrqCntr < 2u){
//				
//				++timIrqCntr;
//			}
//			else{
//				
//				sclkState = SCLK_START;
//				
//				timIrqCntr = 0u;
//			}
//			
//			break;
//	}
//}

static uint8_t initSclk(const Spi_t * pSpiData){
	
	uint8_t initSuccess = 1u;
	uint32_t tmPeriphClk;
	TIM_TIMERCFG_Type tmCfg;
	TIM_MATCHCFG_Type tmMatchCfg;
	uint32_t sclkBit;
	
	tmPeriphClk = CLKPWR_GetPCLK (CLKPWR_PCLKSEL_TIMER2);
	
	tmCfg.PrescaleOption = TIM_PRESCALE_TICKVAL;
	tmCfg.PrescaleValue = tmPeriphClk / pSpiData->clkFreq_ / 2u;
	
	TIM_Init(LPC_TIM2, TIM_TIMER_MODE, &tmCfg);
	
	tmMatchCfg.MatchChannel = 0u;//MATCH_CHANNEL;
	tmMatchCfg.IntOnMatch = ENABLE;
	tmMatchCfg.StopOnMatch = DISABLE;
	tmMatchCfg.ResetOnMatch = ENABLE;
	tmMatchCfg.ExtMatchOutputType = TIM_EXTMATCH_NOTHING;
	tmMatchCfg.MatchValue = 0u;
	
	TIM_ConfigMatch(LPC_TIM2, &tmMatchCfg);
	
	NVIC_SetPriority(TIMER2_IRQn, TIMER_2_PRIORITY);
	
	NVIC_EnableIRQ(TIMER2_IRQn);
	
	//TODO: it shouldn't be here...
	initPins(pSpiData);
	
	sclkBit = bitMaskWithBitPosition(SCLK_PIN_NUM);
	
	if(IDLE_LOW == pSpiData->sclkIdleState_){
		
		GPIO_ClearValue(SPI_PORT_NUM, sclkBit);
	}
	else{
		
		GPIO_SetValue(SPI_PORT_NUM, sclkBit);
	}
	
	//GPIO_IntCmd(SPI_PORT_NUM, bit(SCLK_PIN_NUM), RISING_EDGE);
	
	return initSuccess;
}

static void setMosi(uint8_t outValue){

	uint32_t mosiBit = bitMaskWithBitPosition(MOSI_PIN_NUM);

	if(1u == outValue){
		
		GPIO_SetValue(SPI_PORT_NUM, mosiBit);
	}
	else if(0u == outValue){
		
		GPIO_ClearValue(SPI_PORT_NUM, mosiBit);
	}
}

static uint8_t getMiso(void){
	
	uint32_t misoPortState = GPIO_ReadValue(SPI_PORT_NUM);
	
	return bitValueOnPosition(misoPortState, MISO_PIN_NUM);
}

static void initPins(const Spi_t * pSpiData){
	
	uint32_t sclkBitMask = bitMaskWithBitPosition(SCLK_PIN_NUM);
	uint32_t bitsNotToMask =	sclkBitMask | bitMaskWithBitPosition(MOSI_PIN_NUM) | bitMaskWithBitPosition(MISO_PIN_NUM) |
														bitMaskWithBitPosition(CS1_PIN_NUM) | bitMaskWithBitPosition(CS2_PIN_NUM);
	
	initPin(SCLK_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL, OUTPUT);
	initPin(MOSI_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL, OUTPUT);
	initPin(MISO_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL, INPUT);
	initPin(CS1_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL, OUTPUT);
	initPin(CS2_PIN_NUM, PINSEL_PINMODE_PULLUP, PINSEL_PINMODE_NORMAL, OUTPUT);
	
	FIO_SetMask(SPI_PORT_NUM, bitsNotToMask, PINS_NOT_MASKED);
	
//	if(IDLE_LOW == pSpiData->sclkIdleState_){
//		
//		GPIO_ClearValue(SPI_PORT_NUM, sclkBit);
//	}
//	else{
//		
//		GPIO_SetValue(SPI_PORT_NUM, sclkBit);
//	}
	
	GPIO_ClearValue( SPI_PORT_NUM, bitMaskWithBitPosition(MOSI_PIN_NUM) );
	GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS1_PIN_NUM) );
	GPIO_SetValue( SPI_PORT_NUM, bitMaskWithBitPosition(CS2_PIN_NUM) );
}

static uint8_t getBitCountingFromMsb(const Byte_t * pData, uint8_t bytesNum, uint16_t bitNum){
	
	uint8_t byteNum = bitNum / CHAR_BIT;
	uint8_t bitPosInByte = bitNum % CHAR_BIT;
	uint8_t maskedByte = pData[byteNum] & bitMaskWithBitPosition(CHAR_BIT - bitPosInByte - 1u);
	
	return maskedByte >> (CHAR_BIT - bitPosInByte - 1u);
}

static uint8_t getBitCountingFromLsb(const Byte_t * pData, uint8_t bytesNum, uint16_t bitNum){
	
	return getBitCountingFromMsb(pData, bytesNum, bytesNum * sizeof(*pData) * CHAR_BIT - 1u - bitNum);
}

static void setBitCountingFromMsb(Byte_t * pData, uint8_t bytesNum, uint16_t bitNum, uint8_t newValue){
	
	uint8_t byteIndex = bitNum / CHAR_BIT;
	uint8_t bitInByteIndex = bitNum % CHAR_BIT;
	uint8_t bitMask = bitMaskWithBitPosition(CHAR_BIT - bitInByteIndex - 1u);
	uint8_t byteValue = pData[byteIndex];
	
	if(newValue > 0u){
		
		byteValue |= bitMask;
	}
	else{
		
		byteValue &= ~bitMask;
	}
	
	pData[byteIndex] = byteValue;
}

static void setBitCountingFromLsb(Byte_t * pData, uint8_t bytesNum, uint16_t bitNum, uint8_t newValue){
	
	setBitCountingFromMsb(pData, bytesNum, bytesNum * sizeof(*pData) * CHAR_BIT - 1u - bitNum, newValue);
}

static void initPin(uint8_t pinNum, uint8_t pinMode, uint8_t openDrain, uint8_t pinDir){
	
	PINSEL_CFG_Type pinCfg;
	
	pinCfg.Portnum = SPI_PORT_NUM;
	pinCfg.Pinnum = pinNum;
	pinCfg.Funcnum = GPIO_FUNC_NUM;
	pinCfg.Pinmode = pinMode;
	pinCfg.OpenDrain = openDrain;
	
	PINSEL_ConfigPin(&pinCfg);
	
	GPIO_SetDir(SPI_PORT_NUM, bitMaskWithBitPosition(pinNum), pinDir);
}

uint32_t cntr;

void TIMER2_IRQHandler(void){
	
	//++cntr;
	
	timerInt = 1u;
	
	NVIC_ClearPendingIRQ(TIMER2_IRQn);
	
	TIM_ClearIntPending(LPC_TIM2, TIM_MR0_INT);
}
