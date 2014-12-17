
#include "lpc17xx.h"
#include "uart.h"

#define FOSC                        12000000                            /*  Czestotliwosc oscylatora                  */

#define FCCLK                      (FOSC  * 8)                          /*  Czestotliwosc zegara<=100Mhz          */
                                                                        /*  Calkowite wielokrotnosci FOSC               */
#define FCCO                       (FCCLK * 3)                          /*  Czestotliwosci PLL ( 275MHz ~ 550MHz )      */
                                                                        /*  Taka sama jak FCCLK lub jej wielokrotnosc */
#define FPCLK                      (FCCLK / 4)                          /*  Czestotliwosc zegara , FCCLK 1/2, 1/4*/
                                                                        /*  Lub identyczne z FCCLK              */

#define UART0_BPS     115200                                             /* Szybkosc transmisji komunikacji szeregowej 0             */
#define UART2_BPS     115200                                             /* Szybkosc transmisji komunikacji szeregowej 2             */
/*********************************************************************************************************
** Function name:       UART0_Init
** Descriptions:        Domyslny pin i parametrow komunikacji szeregowej inicjowane na 0 . Jest ustawiona na 8 bitow danych , 1 bit stopu , bez parzystosci
** input parameters:    nie
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
void UART0_Init (void)
{
	uint16_t usFdiv;
    /* UART0 */
    LPC_PINCON->PINSEL0 |= (1 << 4);             /* Pin P0.2 used as TXD0 (Com0) */
    LPC_PINCON->PINSEL0 |= (1 << 6);             /* Pin P0.3 used as RXD0 (Com0) */

  	LPC_UART0->LCR  = 0x83;                      /* Umozliwia ustawienie szybkosci transmisji              */
    usFdiv = (FPCLK / 16) / UART0_BPS;           /* Ustaw szybkosc transmisji                   */
    LPC_UART0->DLM  = usFdiv / 256;
    LPC_UART0->DLL  = usFdiv % 256; 
    LPC_UART0->LCR  = 0x03;                      /*  zablokuj predkosc transmisji                  */
    LPC_UART0->FCR  = 0x06; 				   
}

/*********************************************************************************************************
** Function name:       UART0_SendByte
** Descriptions:        Wysylanie danych z portu szeregowego 0
** input parameters:    data: Transmisja danych
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
int UART0_SendByte (int ucData)
{
	while (!(LPC_UART0->LSR & 0x20));
    return (LPC_UART0->THR = ucData);
}

/*----------------------------------------------------------------------------
  Read character from Serial Port   (blocking read)
 *----------------------------------------------------------------------------*/
int UART0_GetChar (void) 
{
  	while (!(LPC_UART0->LSR & 0x01));
  	return (LPC_UART0->RBR);
}

/*********************************************************************************************************
Write character to Serial Port
** Function name:       UART0_SendString
** Descriptions:	    Wyslij lancuch do portu szeregowego
** input parameters:    s:   Aby wyslac wskaznik ciag
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
void UART0_SendString (char *s)
{
  	while (*s != 0) 
	{
   		UART0_SendByte(*s++);
   		Delay(10);
	}
}

/*********************************************************************************************************
** Function name:       UART2_Init
** Descriptions:        Domyslna wartoscia jest inicjowany przez parametrow komunikacji szeregowej pin i 2 . Jest ustawiona na 8 bitow danych , 1 bit stopu , bez parzystosci
** input parameters:    nie
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
void UART2_Init (void)
{
	uint16_t usFdiv;
    /* UART2 */
    LPC_PINCON->PINSEL0 |= (1 << 20);             /* Pin P0.10 used as TXD2 (Com2) */
    LPC_PINCON->PINSEL0 |= (1 << 22);             /* Pin P0.11 used as RXD2 (Com2) */

   	LPC_SC->PCONP = LPC_SC->PCONP|(1<<24);	      /*Otwarte UART2 nieco regulacja mocy	           */

    LPC_UART2->LCR  = 0x83;                       /* Pozwala ustawic szybkosc transmisji                */
    usFdiv = (FPCLK / 16) / UART2_BPS;            /* ustawic pr?dkosc transmisji                    */
    LPC_UART2->DLM  = usFdiv / 256;
    LPC_UART2->DLL  = usFdiv % 256; 
    LPC_UART2->LCR  = 0x03;                       /* zablokuj predkosc transmisji                    */
    LPC_UART2->FCR  = 0x06;
}

/*********************************************************************************************************
** Function name:       UART2_SendByte
** Descriptions:        2 Port szeregowy do wysylania danych z
** input parameters:    data: Transmisja danych
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
int UART2_SendByte (int ucData)
{
	while (!(LPC_UART2->LSR & 0x20));
	Delay(1);
    return (LPC_UART2->THR = ucData);
}

/*----------------------------------------------------------------------------
  Read character from Serial Port   (blocking read)
 *----------------------------------------------------------------------------*/
int UART2_GetChar (void) 
{
  	while (!(LPC_UART2->LSR & 0x01));
  	return (LPC_UART2->RBR);
}

/*********************************************************************************************************
** Write character to Serial Port
** Function name:       UART2_SendString
** Descriptions:	    Wyslij lancuch do portu szeregowego
** input parameters:    s:   Aby wyslac wskaznik ciag
** output parameters:   nie
** Returned value:      nie
*********************************************************************************************************/
void UART2_SendString (char *s)
{
  	while (*s != 0) 
	{
   		UART2_SendByte(*s++);
	}
}


void Delay (uint32_t Time){
    uint32_t i;

    i = 0;
    while (Time--) {
        for (i = 0; i < 5000; i++);
    }
}
