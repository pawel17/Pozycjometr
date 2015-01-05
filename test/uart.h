#include "lpc17xx_uart.h"
#include "lpc17xx_libcfg_default.h"
#include "lpc17xx_pinsel.h"
#include <string.h>

/* buffer size definition */
#define UART_RING_BUFSIZE 256
/* Buf mask */
#define __BUF_MASK (UART_RING_BUFSIZE-1)
/* Check buf is full or not */
#define __BUF_IS_FULL(head, tail) ((tail&__BUF_MASK)==((head+1)&__BUF_MASK))
/* Check buf will be full in next receiving or not */
#define __BUF_WILL_FULL(head, tail) ((tail&__BUF_MASK)==((head+2)&__BUF_MASK))
/* Check buf is empty */
#define __BUF_IS_EMPTY(head, tail) ((head&__BUF_MASK)==(tail&__BUF_MASK))
/* Reset buf */
#define __BUF_RESET(bufidx) (bufidx=0)
#define __BUF_INCR(bufidx) (bufidx=(bufidx+1)&__BUF_MASK)

/************************** PRIVATE TYPES *************************/
/** @brief UART Ring buffer structure */
typedef struct {
	__IO uint32_t tx_head; /*!< UART Tx ring buffer head index */
	__IO uint32_t tx_tail; /*!< UART Tx ring buffer tail index */
	__IO uint32_t rx_head; /*!< UART Rx ring buffer head index */
	__IO uint32_t rx_tail; /*!< UART Rx ring buffer tail index */
	__IO uint8_t tx[UART_RING_BUFSIZE]; /*!< UART Tx data ring buffer */
	__IO uint8_t rx[UART_RING_BUFSIZE]; /*!< UART Rx data ring buffer */
} UART_RING_BUFFER_T;

/************************** PRIVATE VARIABLES *************************/
// UART Ring buffer
UART_RING_BUFFER_T rb;
// Current Tx Interrupt enable state
__IO FlagStatus TxIntStat;
//Pointer to reset function
void (*ResetData)(void);

/************************** PRIVATE FUNCTIONS *************************/
/* Interrupt service routines */
void UART2_IRQHandler(void);
void UART_IntErr(uint8_t bLSErrType);
void UART_IntTransmit(void);
void UART_IntReceive(void);
uint32_t UARTReceive(LPC_UART_TypeDef *UARTPort, uint8_t *rxbuf, uint8_t buflen);
uint32_t UARTSend(LPC_UART_TypeDef *UARTPort, uint8_t *txbuf, uint8_t buflen);
void Delay (uint32_t Time);
