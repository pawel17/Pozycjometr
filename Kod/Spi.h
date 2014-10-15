#ifndef SPI_H

	#define SPI_H
	
	//dla typow o rozmiarach niezaleznych od implementacji (np. uint8_t, int32_t itd.)
	#include <stdint.h>
		
	//#define MAX_SEND_DATA_LENGTH	4u
	//#define MAX_READ_DATA_LENGTH	4u
	
	/**
	@brief alias dla bajtu
	**/
	typedef uint8_t Byte_t;
	
	/**
	@brief typ oznaczajacy polaryzacje zegara spi (odpowiada za nia czesto bit nazywany CPOL)
	@link http://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus
	@param IDLE_LOW stan nieaktywny zegara = stan niski
	@param IDLE_HIGH stan nieaktywny zegara = stan wysoki
	**/
	typedef enum{
		
		IDLE_LOW,
		IDLE_HIGH,
	}ClkPol_t;
	
	/**
	@brief typ oznaczajacy faze zegara spi (odpowiada za nia czesto bit nazywany CPHA)
	@link http://en.wikipedia.org/wiki/Serial_Peripheral_Interface_Bus
	@param SAMPLE_ON_LEADING odczyt nastepuje przy pierwszym zboczu, zapis przy drugim
	@param SAMPLE_ON_TRAILING odczyt przy drugim zboczu, zapis przy pierwszym
	**/
	typedef enum{
		
		SAMPLE_ON_LEADING,
		SAMPLE_ON_TRAILING
	} ClkPh_t;
	
	/**
	@brief typ oznaczajacy kolejnosc bajtow
	@param LSB_FIRST kolejnosc rozpoczynajaca sie od mlodszego bajtu
	@param MSB_FIRST kolejnosc rozpoczynajaca sie od starszego bajtu
	**/
	typedef enum{
		
		MSB_FIRST,
		LSB_FIRST
	} BOrder_t;
	
	/**
	@brief typ przechowujacy informacje o SPI
	**/
	typedef struct{
		
		/**
		@param clkFreq_ czestotliwosc zegara w Hz
		**/
		uint32_t clkFreq_;
		/**
		@param clkFreq_ polaryzacja zegara SPI w Hz
		**/
		ClkPol_t sclkIdleState_;
		/**
		@param clkPh_ faza zegara SPI w Hz
		**/
		ClkPh_t clkPh_;
		/**
		@param bitOrd_ kolejnosc bitow
		**/
		BOrder_t bitOrd_;
		/**
		@brief wskaznik na funkcje pobierajaca bit z tablicy
		@param pData tablica, z ktorej pobierany jest bit
		@param bytesNum dlugosc tablicy
		@param bitNum indeks bitu do pobrania
		@return wartosc bitu o indeksie bitNum z tablicy pData
		**/
		uint8_t (* fpGetBit)(const Byte_t * pData, uint8_t bytesNum, uint16_t bitNum);
		/**
		@brief wskaznik na funkcje ustawiajaca bit w tablicy
		@param pData tablica, ktorej bit jest ustawiany
		@param bytesNum dlugosc tablicy
		@param bitNum indeks bitu do ustawienia
		@param newValue nowa wartosc bitu (0 lub 1)
		**/
		void (* fpSetBit)(Byte_t * pData, uint8_t bytesNum, uint16_t bitNum, uint8_t newValue);
		/**
		@param dataWritten_ poki co niepotrzebne pole...
		**/
		uint8_t dataWritten_;
		/**
		@param prevSclkState_ poprzednia wartosc sygnalu zegara SPI
		**/
		uint8_t prevSclkState_;
	} Spi_t;
	
	/**
		@brief funkcja inicjalizujaca SPI
		@param pSpiData wskaznik na strukture Spi_t, przechowujaca informacje o SPI
		@param clkFreq czestotliwosc zegara SPI w Hz
		@param sclkIdleState polaryzacja zegara SPI
		@param clkPh polaryzacja zegara SPI
		@param bitOrd kolejnosc bajtow
		@return sukces (1 - tak, 0 - nie)
		**/
	uint8_t spiInit(Spi_t * pSpiData, uint32_t clkFreq, ClkPol_t sclkIdleState, ClkPh_t clkPh, BOrder_t bitOrd);
	/**
		@brief funkcja wysylajaca dane przy pomocy SPI
		@param pSpiData wskaznik na strukture Spi_t, przechowujaca informacje o SPI
		@param pData wskaznik na dane do wyslania
		@param dataLen ilosc bajtow do wyslania
		**/
	uint8_t spiWrite(const Spi_t * pSpiData, const Byte_t * pData, uint8_t dataLen);
	/**
		@brief funkcja odbierajaca dane przy pomocy SPI
		@param pSpiData wskaznik na strukture Spi_t, przechowujaca informacje o SPI
		@param pDataBuffer bufor odbiorczy
		@param dataLen ilosc bajtow do odebrania
		**/
	uint8_t spiRead(const Spi_t * pSpiData, Byte_t * pDataBuffer, uint8_t dataLen);
	
#endif
