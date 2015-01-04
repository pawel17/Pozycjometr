#include <lpc17xx_gpio.h>
#include <lpc17xx_pinsel.h>
#include "LED.h"

#define LED_PORT_NUM	2
#define GPIO_FUNC_NUM	PINSEL_FUNC_0
#define OUTPUT			1
#define LED_PIN_NUM		0
#define PINS_NOT_MASKED	0u

static void initLEDPin(void);

static inline uint32_t bitMaskWithBitPosition(uint8_t bitPos){

	return (uint32_t)1u << bitPos;
}

void LEDInit(void){

	uint32_t bitsNotToMask = bitMaskWithBitPosition(LED_PIN_NUM);

	initLEDPin();

	FIO_SetMask(LED_PORT_NUM, bitsNotToMask, PINS_NOT_MASKED);

	GPIO_ClearValue( LED_PORT_NUM, bitMaskWithBitPosition(LED_PIN_NUM) );
}

void LEDOn(void){

	uint32_t ledBit = bitMaskWithBitPosition(LED_PIN_NUM);

	GPIO_SetValue(LED_PORT_NUM, ledBit);
}

void LEDOff(void){

	uint32_t ledBit = bitMaskWithBitPosition(LED_PIN_NUM);

	GPIO_ClearValue(LED_PORT_NUM, ledBit);
}

void LEDToggle(void){

	static uint8_t isOn = 0;

	if(isOn){

		LEDOff();

		isOn = 0;
	}
	else{

		LEDOn();

		isOn = 1;
	}
}

static void initLEDPin(void){

	PINSEL_CFG_Type pinCfg;

	pinCfg.Portnum = LED_PORT_NUM;
	pinCfg.Pinnum = LED_PIN_NUM;
	pinCfg.Funcnum = GPIO_FUNC_NUM;
	pinCfg.Pinmode = PINSEL_PINMODE_PULLUP;
	pinCfg.OpenDrain = PINSEL_PINMODE_NORMAL;

	PINSEL_ConfigPin(&pinCfg);

	GPIO_SetDir(LED_PORT_NUM, bitMaskWithBitPosition(LED_PIN_NUM), OUTPUT);
}
