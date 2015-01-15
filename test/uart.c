/**********************************************************************
 * $Id$ uart_interrupt_test.c 2010-05-21
 *//**
 * @file uart_interrupt_test.c
 * @brief This example describes how to using UART in interrupt mode
 * @version 2.0
 * @date 21. May. 2010
 * @author NXP MCU SW Application Team
 *
 * Copyright(C) 2010, NXP Semiconductor
 * All rights reserved.
 *
 ***********************************************************************
 * Software that is described herein is for illustrative purposes only
 * which provides customers with programming information regarding the
 * products. This software is supplied "AS IS" without any warranties.
 * NXP Semiconductors assumes no responsibility or liability for the
 * use of the software, conveys no license or title under any patent,
 * copyright, or mask work right to the product. NXP Semiconductors
 * reserves the right to make changes in the software without
 * notification. NXP Semiconductors also make no representation or
 * warranty that such application will be suitable for the specified
 * use without further testing or modification.
 **********************************************************************/
#include "uart.h"

/*----------------- INTERRUPT SERVICE ROUTINES --------------------------*/
/*********************************************************************//**
 * @brief UART2 interrupt handler sub-routine
 * @param[in] None
 * @return None
 **********************************************************************/
void UART2_IRQHandler(void) {
	uint32_t intsrc, tmp, tmp1;
	/* Determine the interrupt source */
	intsrc = UART_GetIntId(LPC_UART2);
	tmp = intsrc & UART_IIR_INTID_MASK;
// Receive Line Status
	if (tmp == UART_IIR_INTID_RLS) {
// Check line status
		tmp1 = UART_GetLineStatus(LPC_UART2);
// Mask out the Receive Ready and Transmit Holding empty status
		tmp1 &= (UART_LSR_OE | UART_LSR_PE | UART_LSR_FE | UART_LSR_BI
				| UART_LSR_RXFE);
// If any error exist
		if (tmp1) {
			UART_IntErr(tmp1);
		}
	}
// Receive Data Available or Character time-out
	if ((tmp == UART_IIR_INTID_RDA) || (tmp == UART_IIR_INTID_CTI)) {
		UART_IntReceive();
	}
// Transmit Holding Empty
	if (tmp == UART_IIR_INTID_THRE) {
		UART_IntTransmit();
	}
}
/********************************************************************//**
 * @brief UART receive function (ring buffer used)
 * @param[in] None
 * @return None
 *********************************************************************/
void UART_IntReceive(void) {
	uint8_t tmpc;
	uint32_t rLen;
	while (1) {
// Call UART read function in UART driver
		rLen = UART_Receive((LPC_UART_TypeDef *) LPC_UART2, &tmpc, 1,
				NONE_BLOCKING);
// If data received
		if (rLen) {
			/* Check if buffer is more space
			 * If no more space, remaining character will be trimmed out
			 */
			if (!__BUF_IS_FULL(rb.rx_head,rb.rx_tail)) {
				rb.rx[rb.rx_head] = tmpc;
				__BUF_INCR(rb.rx_head);
			}
		}
// no more data
		else{
			break;
		}

		if(!strcmp(rb.rx, "RESET")) {
			ResetData();
			memset(rb.rx, 0, sizeof(rb.rx));//strcpy(rb.rx, "");
						rb.rx_head = 0;
						rb.rx_tail = 0;
		}

	}
}
/********************************************************************//**
 * @brief UART transmit function (ring buffer used)
 * @param[in] None
 * @return None
 *********************************************************************/
void UART_IntTransmit(void) {
// Disable THRE interrupt
	UART_IntConfig((LPC_UART_TypeDef *) LPC_UART2, UART_INTCFG_THRE, DISABLE);
	/* Wait for FIFO buffer empty, transfer UART_TX_FIFO_SIZE bytes
	 * of data or break whenever ring buffers are empty */
	/* Wait until THR empty */
	while (UART_CheckBusy((LPC_UART_TypeDef *) LPC_UART2) == SET)
		;
	while (!__BUF_IS_EMPTY(rb.tx_head,rb.tx_tail)) {
		/* Move a piece of data into the transmit FIFO */
		if (UART_Send((LPC_UART_TypeDef *) LPC_UART2,
				(uint8_t *) &rb.tx[rb.tx_tail], 1, NONE_BLOCKING)) {
			/* Update transmit ring FIFO tail pointer */
			__BUF_INCR(rb.tx_tail);
		} else {
			break;
		}
	}
	/* If there is no more data to send, disable the transmit
	 interrupt - else enable it or keep it enabled */
	if (__BUF_IS_EMPTY(rb.tx_head, rb.tx_tail)) {
		UART_IntConfig((LPC_UART_TypeDef *) LPC_UART2, UART_INTCFG_THRE,
				DISABLE);
// Reset Tx Interrupt state
		TxIntStat = RESET;
	} else {
// Set Tx Interrupt state
		TxIntStat = SET;
		UART_IntConfig((LPC_UART_TypeDef *) LPC_UART2, UART_INTCFG_THRE,
				ENABLE);
	}
}
/*********************************************************************//**
 * @brief UART Line Status Error
 * @param[in] bLSErrType UART Line Status Error Type
 * @return None
 **********************************************************************/
void UART_IntErr(uint8_t bLSErrType) {
	uint8_t test;
// Loop forever
	while (1) {
// For testing purpose
		test = bLSErrType;
	}
}
/*-------------------------PRIVATE FUNCTIONS------------------------------*/
/*********************************************************************//**
 * @brief UART transmit function for interrupt mode (using ring buffers)
 * @param[in] UARTPort Selected UART peripheral used to send data,
 * should be UART2
 * @param[out] txbuf Pointer to Transmit buffer
 * @param[in] buflen Length of Transmit buffer
 * @return Number of bytes actually sent to the ring buffer
 **********************************************************************/
uint32_t UARTSend(LPC_UART_TypeDef *UARTPort, uint8_t *txbuf, uint8_t buflen) {
	uint8_t *data = (uint8_t *) txbuf;
	uint32_t bytes = 0;
	/* Temporarily lock out UART transmit interrupts during this
	 read so the UART transmit interrupt won't cause problems
	 with the index values */
	UART_IntConfig(UARTPort, UART_INTCFG_THRE, DISABLE);
	/* Loop until transmit run buffer is full or until n_bytes
	 expires */
	while ((buflen > 0) && (!__BUF_IS_FULL(rb.tx_head, rb.tx_tail))) {
		/* Write data from buffer into ring buffer */
		rb.tx[rb.tx_head] = *data;
		data++;
		/* Increment head pointer */
		__BUF_INCR(rb.tx_head);
		/* Increment data count and decrement buffer size count */
		bytes++;
		buflen--;
	}
	/*
	 * Check if current Tx interrupt enable is reset,
	 * that means the Tx interrupt must be re-enabled
	 * due to call UART_IntTransmit() function to trigger
	 * this interrupt type
	 */
	if (TxIntStat == RESET) {
		UART_IntTransmit();
	}
	/*
	 * Otherwise, re-enables Tx Interrupt
	 */
	else {
		UART_IntConfig(UARTPort, UART_INTCFG_THRE, ENABLE);
	}
	return bytes;
}
/*********************************************************************//**
 * @brief UART read function for interrupt mode (using ring buffers)
 * @param[in] UARTPort Selected UART peripheral used to send data,
 * should be UART2
 * @param[out] rxbuf Pointer to Received buffer
 * @param[in] buflen Length of Received buffer
 * @return Number of bytes actually read from the ring buffer
 **********************************************************************/
uint32_t UARTReceive(LPC_UART_TypeDef *UARTPort, uint8_t *rxbuf, uint8_t buflen) {
	uint8_t *data = (uint8_t *) rxbuf;
	uint32_t bytes = 0;
	/* Temporarily lock out UART receive interrupts during this
	 read so the UART receive interrupt won't cause problems
	 with the index values */
	UART_IntConfig(UARTPort, UART_INTCFG_RBR, DISABLE);
	/* Loop until receive buffer ring is empty or
	 until max_bytes expires */
	while ((buflen > 0) && (!(__BUF_IS_EMPTY(rb.rx_head, rb.rx_tail)))) {
		/* Read data from ring buffer into user buffer */
		*data = rb.rx[rb.rx_tail];
		data++;
		/* Update tail pointer */
		__BUF_INCR(rb.rx_tail);
		/* Increment data count and decrement buffer size count */
		bytes++;
		buflen--;
	}
	/* Re-enable UART interrupts */
	UART_IntConfig(UARTPort, UART_INTCFG_RBR, ENABLE);
	return bytes;
}

/*-------------------------MAIN FUNCTION------------------------------*/
/*********************************************************************//**
 * @brief c_entry: Main UART program body
 * @param[in] None
 * @return int
 **********************************************************************/
void UART2_Init(void) {
// UART Configuration structure variable
	UART_CFG_Type UARTConfigStruct;
// UART FIFO configuration Struct variable
	UART_FIFO_CFG_Type UARTFIFOConfigStruct;
// Pin configuration for UART2
	PINSEL_CFG_Type PinCfg;
	uint32_t idx, len;
	__IO FlagStatus exitflag;
	uint8_t buffer[10];
	/*
	 * Initialize UART2 pin connect
	 */
	PinCfg.Funcnum = 1;
	PinCfg.OpenDrain = 0;
	PinCfg.Pinmode = 0;
	PinCfg.Pinnum = 10;
	PinCfg.Portnum = 0;
	PINSEL_ConfigPin(&PinCfg);
	PinCfg.Pinnum = 11;
	PINSEL_ConfigPin(&PinCfg);
	/* Initialize UART Configuration parameter structure to default state:
	 * Baudrate = 9600bps
	 * 8 data bit
	 * 1 Stop bit
	 * None parity
	 */
	UART_ConfigStructInit(&UARTConfigStruct);
	UARTConfigStruct.Baud_rate = 115200;

// Initialize UART2 peripheral with given to corresponding parameter
	UART_Init((LPC_UART_TypeDef *) LPC_UART2, &UARTConfigStruct);
	/* Initialize FIFOConfigStruct to default state:
	 * - FIFO_DMAMode = DISABLE
	 * - FIFO_Level = UART_FIFO_TRGLEV0
	 * - FIFO_ResetRxBuf = ENABLE
	 * - FIFO_ResetTxBuf = ENABLE
	 * - FIFO_State = ENABLE
	 */
	UART_FIFOConfigStructInit(&UARTFIFOConfigStruct);
// Initialize FIFO for UART2 peripheral
	UART_FIFOConfig((LPC_UART_TypeDef *) LPC_UART2, &UARTFIFOConfigStruct);
// Enable UART Transmit
	UART_TxCmd((LPC_UART_TypeDef *) LPC_UART2, ENABLE);
	/* Enable UART Rx interrupt */
	UART_IntConfig((LPC_UART_TypeDef *) LPC_UART2, UART_INTCFG_RBR, ENABLE);
	/* Enable UART line status interrupt */
	UART_IntConfig((LPC_UART_TypeDef *) LPC_UART2, UART_INTCFG_RLS, ENABLE);
	/*
	 * Do not enable transmit interrupt here, since it is handled by
	 * UART_Send() function, just to reset Tx Interrupt state for the
	 * first time
	 */
	TxIntStat = RESET;
// Reset ring buf head and tail idx
	__BUF_RESET(rb.rx_head);
	__BUF_RESET(rb.rx_tail);
	__BUF_RESET(rb.tx_head);
	__BUF_RESET(rb.tx_tail);
	/* preemption = 1, sub-priority = 1 */
	NVIC_SetPriority(UART2_IRQn, ((0x01 << 3) | 0x01));
	/* Enable Interrupt for UART2 channel */
	NVIC_EnableIRQ(UART2_IRQn);
}

void Delay (uint32_t Time)
{
    uint32_t i;

    i = 0;
    while (Time--) {
        for (i = 0; i < 5000; i++);
    }
}
