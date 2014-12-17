#ifndef UART_H
#define UART_H

void UART0_Init (void);
int  UART0_SendByte (int ucData);
int  UART0_GetChar (void);
void UART0_SendString (char *s);

void UART2_Init (void);
int  UART2_SendByte (int ucData);
int  UART2_GetChar (void);
void UART2_SendString (char *s);
void UART0_SendChar(uint16_t disp);  


void Delay (uint32_t Time);

#endif //UART_H
