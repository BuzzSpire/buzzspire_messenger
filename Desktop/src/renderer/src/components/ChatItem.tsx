import '../assets/ChatItem.css'
import { FC } from 'react'
import { Flex, Typography } from 'antd'

interface ChatItemProps {
  usernName?: string
  message: string
  date: string
  color: string
}

export const ChatItem: FC<ChatItemProps> = ({ color, usernName, message, date }) => {
  return (
    <Flex
      vertical={true}
      style={{ background: color, padding: '5px', borderRadius: '8px', maxWidth: '60%' }}
    >
      <Typography.Text style={{ color: 'white', padding: '5px' }}>{message} </Typography.Text>
      <Flex justify="space-between" style={{ padding: '5px' }}>
        <Typography.Text style={{ color: 'white', fontSize: '12px' }}>{date}</Typography.Text>

        <Typography.Text style={{ color: 'white', fontSize: '12px', marginLeft: '10px' }}>
          {usernName}
        </Typography.Text>
      </Flex>
    </Flex>
  )
}
