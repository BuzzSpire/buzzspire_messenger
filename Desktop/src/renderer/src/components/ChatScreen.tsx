import { FC, useEffect, useState } from 'react'
import { Avatar, Flex, Typography } from 'antd'
import InfiniteScroll from 'react-infinite-scroll-component'
import { ChatItem } from './ChatItem'
import { GetMessages } from '../API/Messages'
import { Message } from '../Types/EntitysType'
import { Notification } from '../Types/NotificationType'

interface ChatScreenProps {
  username: string
  notification: Notification[]
}

export const ChatScreen: FC<ChatScreenProps> = ({ username, notification }): JSX.Element => {
  const [defaultUserName, setDefaultUserName] = useState<string>('')
  const [messages, setMessages] = useState<Message[]>([])
  const [visitMessages, setVisitMessages] = useState<Message[]>([])
  const [hasMore, setHasMore] = useState<boolean>(true)

  useEffect(() => {
    const username = localStorage.getItem('username')

    if (username) {
      setDefaultUserName(username)
      getMessage()
    }

    const chatScreen = document.querySelector('.chatScreen')
    if (chatScreen) {
      chatScreen.scrollTop = chatScreen.scrollHeight
    }
  }, [username])

  const getMessage = (): void => {
    const token = localStorage.getItem('token')

    if (token) {
      GetMessages(token, username).then((res) => {
        if (res) {
          setMessages(res)
          setVisitMessages(res.reverse().slice(0, 10))
        }
      })
    }
  }

  const fetchMoreData = (): void => {
    if (visitMessages.length >= messages.length) {
      setHasMore(false)
    } else {
      setVisitMessages((prev) => [
        ...messages.slice(visitMessages.length, visitMessages.length + 10),
        ...prev
      ])
    }
  }

  useEffect(() => {
    console.log(notification)
    if (notification.find((n) => n.senderUserName === username)) {
      getMessage()
    }
  }, [notification])

  return (
    <Flex vertical={true} style={{ minHeight: '90vh' }}>
      <Flex style={{ background: 'white', padding: '10px' }} gap="small">
        <Avatar size="default">{username[0]}</Avatar>
        <Typography.Title level={4}>{username}</Typography.Title>
      </Flex>
      <Flex
        id="chatScreen"
        className="chatScreen"
        vertical={true}
        style={{ height: '100%', overflowY: 'auto' }}
      >
        <div
          id="scrollableDiv"
          style={{
            height: '80vh',
            overflow: 'auto',
            display: 'flex',
            flexDirection: 'column-reverse'
          }}
        >
          {/*Put the scroll bar always on the bottom*/}
          <InfiniteScroll
            dataLength={visitMessages.length}
            next={fetchMoreData}
            style={{ display: 'flex', flexDirection: 'column-reverse' }} //To put endMessage and loader to the top.
            inverse={true} //
            hasMore={hasMore}
            loader={<h4>Loading...</h4>}
            scrollableTarget="scrollableDiv"
          >
            {visitMessages.map((m, index) => (
              <ChatItem
                key={index}
                message={m}
                username={username}
                defaultUserName={defaultUserName}
              />
            ))}
          </InfiniteScroll>
        </div>
      </Flex>
    </Flex>
  )
}
