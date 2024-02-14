import { Avatar, Flex, Typography } from 'antd'
import { FC } from 'react'
import { Message } from '../Types/EntitysType'

interface ChatItemProps {
  message: Message
  username: string
  defaultUserName: string
}

export const ChatItem: FC<ChatItemProps> = ({
  username,
  message,
  defaultUserName
}): JSX.Element => {
  return (
    <Flex
      style={{ padding: '10px' }}
      gap="small"
      align="end"
      justify={message.sender === null ? 'end' : 'start'}
    >
      <Flex vertical={true}>
        <Avatar>{message.receiver === null ? username[0] : defaultUserName[0]}</Avatar>
      </Flex>
      <Flex
        vertical={true}
        gap="small"
        style={{
          padding: '10px',
          maxWidth: '50%',
          backgroundColor: message.receiver === null ? '#dfe3ee' : 'white'
        }}
      >
        <Typography.Text>{message.content}</Typography.Text>
        <Typography.Text style={{ textAlign: 'end' }} type="secondary">
          {new Date(message.date).toLocaleTimeString().slice(0, 5)}
        </Typography.Text>
      </Flex>
    </Flex>
  )
}
