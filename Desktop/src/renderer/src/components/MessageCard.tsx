import { FC } from 'react'
import { Card, Avatar, Typography, Flex, Badge } from 'antd'

interface MessageCardProps {
  message: string
  date: string
  userName: string
  notificationCount: number
}

export const MessageCard: FC<MessageCardProps> = ({
  message,
  userName,
  date,
  notificationCount
}): JSX.Element => {
  const { Text } = Typography

  return (
    <Card hoverable style={{ margin: '3px', width: '100%', position: 'relative' }}>
      <Flex align="center" justify="space-between">
        <Flex gap="small">
          <Avatar style={{ background: 'skyblue' }}>{userName[0]}</Avatar>
          <Flex vertical={true}>
            <Typography>{userName}</Typography>
            <Text style={{ color: 'gray' }}>
              {message.length > 15 ? message.slice(0, 15) + '...' : message}
            </Text>
          </Flex>
        </Flex>

        <Flex gap="middle">
          <Typography style={{ color: 'gray' }}>
            {new Date(date).toLocaleTimeString().slice(0, 5)}
          </Typography>
          <Badge count={notificationCount} />
        </Flex>
      </Flex>
    </Card>
  )
}
