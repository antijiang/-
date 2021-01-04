



#ifndef _UART_PROTOCOL_
#define _UART_PROTOCOL_

/********************************************************************
                     receive function
********************************************************************/
/*receive use state machine method, so two char can arrive different time*/
typedef enum
{
    REV_STATE_HEAD0   = 0x00,
    REV_STATE_HEAD1   = 0x01,
    REV_STATE_LENGTH0 = 0x02,
    REV_STATE_TYPE    = 0x04,
    REV_STATE_CMD     = 0x05,
    REV_STATE_SEQ     = 0x06,
    REV_STATE_DATA    = 0x07,
    REV_STATE_CKSUM0  = 0x08,
    REV_STATE_CKSUM1  = 0x09,
    REV_STATE_TAIL    = 0x0a,
}coms_receive_state_t;





#define COMS_MSG_DATA_MAX_SIZE (8)
typedef struct
{
    uint16_t header;
    uint8_t data_length;
    uint8_t msg_type;
    uint8_t msg_cmd;
    uint8_t msg_seq;
    uint8_t msg_data[COMS_MSG_DATA_MAX_SIZE];
    /*uint16_t chksum; send add auto*/
    /*uint8_t tail; send add auto*/
}sys_msg_com_data_t;


/*header*/
#define COMS_MSG_HEAD_LOW  (0x5A)
#define COMS_MSG_HEAD_HIGH (0xA5)  //FC
#define COMS_MSG_HEAD   ((COMS_MSG_HEAD_HIGH<<8)|COMS_MSG_HEAD_LOW)


/*tail*/
#define COMS_MSG_TAIL   (0xFB)


/*msg_type*/
#define COMS_MSG_TYPE_CMD_UP   (0xA0)
#define COMS_MSG_TYPE_CMD_DOWN (0xA2)
#define COMS_MSG_TYPE_ACK      (0xA1)
#define COMS_MSG_TYPE_NOTIFY   (0xA3)


/*msg_cmd*/
#define COMS_MSG_CMD_TIME_DELAY_ON    (0x91)
#define COMS_MSG_CMD_TIME_DELAY_OFF    (0x92)
#define COMS_MSG_CMD_FEED_DOG    			(0x93)

#define COMS_MSG_CMD_POWER_OFF   		(0x94)

#define COMS_MSG_CMD_CLOCK_CAL   		(0xA0)
#define COMS_MSG_CMD_TIME_ON   		  (0xA1)
#define COMS_MSG_CMD_TIME_OFF   		  (0xA2)

#define COMS_MSG_CMD_NOTIFY_STATUS 		(0x9A)
#define COMS_MSG_CMD_ACK_COMMON    		(0xAA)



/*ACK error code*/
#define COMS_MSG_ACK_ERR_NONE          (0x0)
#define COMS_MSG_ACK_ERR_CHECKSUM      (0xff)
#define COMS_MSG_ACK_ERR_NOSUPPORT     (0xfe)


void coms_receive_packet(BYTE  receive_char);
void coms_communicate_init(void);
void coms_send_notify(BYTE  notify_event);
void userapp_deal_cmd(sys_msg_com_data_t *msg);
void userapp_deal_com_msg(sys_msg_com_data_t *msg);
void userapp_send_com_test(void);







#endif

