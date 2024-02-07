import { useState, useEffect, FC } from 'react'
import '../assets/WebSocketComponent.css'
import { ChatItem } from './ChatItem'
import {
  Layout,
  Menu,
  Flex,
  Input,
  Button,
  MenuProps,
  Typography,
  message as antdMessage
} from 'antd'
import { UserOutlined } from '@ant-design/icons'

type MenuItem = Required<MenuProps>['items'][number]

type ReceivedMessage = {
  userName: string
  message: string
  date: string
  onlineUsers?: string[]
}

type Message = {
  userName: string
  message: string
  date: string
}

interface WebSocketComponentProps {
  username: string
}

const WebSocketComponent: FC<WebSocketComponentProps> = ({ username }) => {
  const [socket, setSocket] = useState<WebSocket | null>(null)
  const [message, setMessage] = useState<string>('')
  const [messagesList, setMessagesList] = useState<Message[]>([])
  const [collapsed, setCollapsed] = useState(false)
  const [onlineUsers, setOnlineUsers] = useState<MenuItem[]>([])

  const [messageApi, contextHolder] = antdMessage.useMessage()
  const { Sider, Content } = Layout

  useEffect(() => {
    // get WebSocket IP from localStorage
    const webSocketIp = localStorage.getItem('webSocketIp')

    // WebSocket  connection opened
    const ws = new WebSocket(`ws://${webSocketIp}`)

    // when the connection is open, send some data to the server
    ws.onopen = (): void => {
      console.log('WebSocket connection opened')
      ws.send(JSON.stringify({ userName: username, message: 'joined the chat' }))
    }

    //  message received
    ws.onmessage = (event): void => {
      const receivedMessage = event.data
      const receivedMessageObject: ReceivedMessage = JSON.parse(receivedMessage)
      setMessagesList((prevMessages) => [...prevMessages, receivedMessageObject])

      if (receivedMessageObject.onlineUsers) {
        setOnlineUsers(
          receivedMessageObject.onlineUsers.map((user) => getItem(user, user, <UserOutlined />))
        )
      }

      console.log(`Received: ${receivedMessage}`)
    }

    // WebSocket connection closed
    ws.onclose = (): void => {
      console.log('WebSocket connection closed')
    }

    setSocket(ws)

    // Component unmount and WebSocket connection closed
    return () => {
      ws.close()
    }
  }, [])

  const sendMessage = (): void => {
    if (message === '') {
      messageApi.open({
        type: 'error',
        content: 'Please enter a message'
      })
      return
    }

    if (socket) {
      const data = {
        userName: username,
        message: message
      }
      socket.send(JSON.stringify(data))
      setMessage('')
    }
  }

  function getItem(
    label: React.ReactNode,
    key: React.Key,
    icon?: React.ReactNode,
    children?: MenuItem[]
  ): MenuItem {
    return {
      key,
      icon,
      children,
      label
    } as MenuItem
  }

  const exitHandler = (): void => {
    localStorage.removeItem('webSocketIp')
    localStorage.removeItem('username')
    window.location.reload()
  }

  return (
    <Layout style={{ minHeight: '100vh' }}>
      <Sider
        style={{ background: 'white' }}
        collapsible
        collapsed={collapsed}
        onCollapse={(value) => setCollapsed(value)}
      >
        <Flex vertical={true} justify="center" gap="middle">
          <Button
            danger
            onClick={exitHandler}
            type="primary"
            style={{ width: '100%', borderRadius: '0' }}
          >
            Exit
          </Button>
          <Flex justify="center">
            <Typography.Title level={5} style={{ color: 'gray' }}>
              {collapsed ? '' : 'Online Users'}
            </Typography.Title>
          </Flex>
        </Flex>
        <Menu theme="light" defaultSelectedKeys={['1']} mode="inline" items={onlineUsers} />
      </Sider>
      <Layout>
        <Content style={{ margin: '0 16px' }}>
          {contextHolder}
          <Flex
            vertical={true}
            style={{ height: '90vh', overflowY: 'auto', width: '100%' }}
            justify="end"
          >
            <Flex vertical={true} gap="middle" style={{ overflowY: 'auto' }}>
              {messagesList.map((message, index) => (
                <Flex
                  key={index}
                  gap="middle"
                  justify={message.userName === username ? 'end' : 'start'}
                >
                  <ChatItem
                    key={index}
                    usernName={username === message.userName ? 'You' : message.userName}
                    message={message.message}
                    date={message.date}
                    color={message.userName === username ? '#1890ff' : '#6cb8fb'}
                  />
                </Flex>
              ))}
            </Flex>
          </Flex>
          <Flex justify="center" align="center" style={{ height: '10vh' }} gap="middle">
            <Input
              placeholder="Type a message"
              type="text"
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              onKeyPress={(e) => {
                if (e.key === 'Enter') {
                  sendMessage()
                }
              }}
            />
            <Button type="primary" onClick={sendMessage}>
              Send
            </Button>
          </Flex>
        </Content>
      </Layout>
    </Layout>
  )
}

export default WebSocketComponent
