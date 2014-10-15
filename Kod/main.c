#include <LPC17xx.h>
#include "Spi.h"

#define DLEN	2u

extern uint32_t cntr;
Byte_t dummy[DLEN] = {150, }; // 1001 0110

int main(void){

	Spi_t spiData;

	spiInit(&spiData, 64u, IDLE_LOW, SAMPLE_ON_TRAILING, LSB_FIRST);

	spiRead(&spiData, dummy, DLEN);

	while(1){
		
		
	}
	
	return 0;
}
