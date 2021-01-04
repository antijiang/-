
#include "general.h"
#include  "uart_protocol.h"
xdata char rev_timeout=0;
sys_msg_com_data_t recever_packet;
static uint8_t rev_state = REV_STATE_HEAD0;
static uint16_t length0 = 0;
static  uint16_t chk_sum0 = 0, chk_sum1 = 0;
static uint16_t data_rev_count = 0;

uint16_t packet_chk_sum;
//串口收到命令处理
void RecMsgProcess(sys_msg_com_data_t * msg)
{
    uint8_t i;
    uint8_t *buf = (uint8_t*)msg;
    DPRINTF(printf("Rmsg:"));
    for(i=0; i<msg->data_length + 6; i++)
    {
        DPRINTF(printf("%bx ",buf[i]));

    }
    switch(msg->msg_cmd)

    {
    case COMS_MSG_CMD_TIME_DELAY_ON:
    case COMS_MSG_CMD_TIME_DELAY_OFF:
    {
        WORD tempT;
        tempT=msg->msg_data[0]|msg->msg_data[1]<<8;

        if(msg->msg_cmd==COMS_MSG_CMD_TIME_DELAY_OFF)
        {
            //COMS_MSG_CMD_TIME_DELAY_OFF
            if(tempT==0xffff)
            {
							//立即关机
                TimerOff_Setup(0);
                goto LABEL_POWER_OFF;
            }
            TimerOff_Setup(tempT);

        }
        else
        {   // COMS_MSG_CMD_TIME_DELAY_ON

            TimerOn_Setup(tempT);
        }
    }

    break;
    case COMS_MSG_CMD_FEED_DOG:
    {   
				WORD tempT;
        tempT=msg->msg_data[0]|msg->msg_data[1]<<8;
        TimerWatchDog_Setup(tempT);
    }
    break;

    case COMS_MSG_CMD_POWER_OFF:
LABEL_POWER_OFF:
				F_STANDBY=0;
				Power_Switch(1 );
				Enter_StanbyStatus();
        break;

    }

}
//判断接收超时，会将接收状态 从零开始
BOOL coms_port_timeout_one_packet()
{
    if(rev_timeout==0)return 1;
    else  return 0;

}
//校验码计算
static uint16_t coms_port_checksum(uint16_t init_val, uint8_t * msgdata, uint16_t length)
{
    uint8_t i;
    uint16_t chk_sum = init_val;
    for(i=0; i<length; i++)
    {
        chk_sum += msgdata[i];
    }

    return chk_sum;
}
//接收一个字节
void coms_receive_packet(BYTE  receive_char1)
{

    BYTE receive_char=receive_char1;
    if(true == coms_port_timeout_one_packet())
    {
        rev_state = REV_STATE_HEAD0;

    }
    rev_timeout=5;
    switch(rev_state)
    {
    case REV_STATE_HEAD0:
        //if(COMS_MSG_HEAD_LOW == receive_char)  //
        if(COMS_MSG_HEAD_HIGH == receive_char)
        {
            rev_state = REV_STATE_HEAD1;
        }
        else
        {
            rev_state = REV_STATE_HEAD0;
        }
        break;
    case REV_STATE_HEAD1:
        //if(COMS_MSG_HEAD_HIGH == receive_char)
        if(COMS_MSG_HEAD_LOW == receive_char)  //
        {
            rev_state = REV_STATE_LENGTH0;
            recever_packet.header = COMS_MSG_HEAD;
        }
        else
        {

//        if(COMS_MSG_HEAD_LOW != receive_char)
//					if(COMS_MSG_HEAD_HIGH == receive_char)
//            {
//                rev_state = REV_STATE_HEAD0;
//            }
        }
        break;
    case REV_STATE_LENGTH0:
        length0 = receive_char;

        if(length0 <= COMS_MSG_DATA_MAX_SIZE)
        {
            recever_packet.data_length = length0;
            rev_state = REV_STATE_TYPE;
        }
        else
        {
            rev_state = REV_STATE_HEAD0;
        }
        break;
    case REV_STATE_TYPE:
        recever_packet.msg_type = receive_char;
        rev_state = REV_STATE_CMD;
        break;
    case REV_STATE_CMD:
        recever_packet.msg_cmd = receive_char;
        rev_state = REV_STATE_SEQ;
        break;
    case REV_STATE_SEQ:
        recever_packet.msg_seq = receive_char;
        if(length0 > 0)
        {
            rev_state = REV_STATE_DATA;
            data_rev_count = 0;
        }
        else
        {
            rev_state = REV_STATE_CKSUM0;
        }
        break;
    case REV_STATE_DATA:
        recever_packet.msg_data[data_rev_count++] = receive_char;
        if(data_rev_count == length0)
        {
            rev_state = REV_STATE_CKSUM0;
        }
        break;
    case REV_STATE_CKSUM0:
        chk_sum0 = receive_char;
        rev_state = REV_STATE_CKSUM1;
        break;
    case REV_STATE_CKSUM1:
    {
        uint16_t packet_chk_sum;
        chk_sum1 = receive_char;
        chk_sum1 <<= 8;
        chk_sum1 += chk_sum0;

        /*recever_packet->chksum = chk_sum1; just used as judgement*/
        packet_chk_sum = coms_port_checksum(0, (uint8_t*)&recever_packet.msg_type, 3);
        packet_chk_sum = coms_port_checksum(packet_chk_sum, recever_packet.msg_data, recever_packet.data_length);

        if(chk_sum1==packet_chk_sum)
        {
            rev_state = REV_STATE_TAIL;
        }
        else
        {
            rev_state = REV_STATE_HEAD0;

        }
    }
    break;

    case REV_STATE_TAIL:
        if(receive_char == COMS_MSG_TAIL)/*receive ok*/
        {
            /*recever_packet->tail = receive_char; just used as judgement*/
            RecMsgProcess((sys_msg_com_data_t*)&recever_packet);

        }
        else
        {
            data_rev_count = 0;

        }
        rev_state = REV_STATE_HEAD0;
        break;
    default:
        rev_state = REV_STATE_HEAD0;
        break;
    }

}
extern void  Uart2putchar(char c);
extern void  Uart1putchar(char c);

void coms_port_send_char( char c  )
{

    //  Uart2putchar(c);
    Uart1putchar(c);
}

/**
 * 发送一个数据包
 * @param[in]    sys_msg_com_data_t * msg    数据包指针
 * @param[out]
 * @return       int
 * @ref
 * @see
 * @note
 */
static uint8_t coms_send_packet(sys_msg_com_data_t * msg)
{
    uint16_t chk_sum;
    uint8_t *buf = (uint8_t*)msg;
    int i;

    if(msg == NULL)
    {
        return 1;
    }

    /*header and data*/
    for(i = 0; i < msg->data_length + 6; i++)
    {
        coms_port_send_char( buf[i] );

    }

    /*check sum*/
    chk_sum = coms_port_checksum(0, (uint8_t*)&msg->msg_type, 3);
    chk_sum = coms_port_checksum(chk_sum, msg->msg_data, msg->data_length);
    coms_port_send_char( chk_sum&0xff );
    coms_port_send_char( (chk_sum>>8)&0xff );

    // DPRINTF(printf("CHKsum %bx %bx \n",(BYTE )(chk_sum&0xff),(BYTE )((chk_sum>>8)&0xff) ));

    /*tail*/
    coms_port_send_char( COMS_MSG_TAIL );



    return 0;
}



void coms_send_notify(uint8_t notify_event)
{
    sys_msg_com_data_t msg;

    msg.header = COMS_MSG_HEAD;
    msg.data_length = 2;
    msg.msg_type = COMS_MSG_TYPE_NOTIFY;
    msg.msg_cmd = COMS_MSG_CMD_NOTIFY_STATUS;
    msg.msg_seq = 0x11;
    msg.msg_data[0] = 0;
    msg.msg_data[1] = notify_event;
    coms_send_packet(&msg);

}

