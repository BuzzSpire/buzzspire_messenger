import { MainLayout } from '../layout/MainLayout'
import { Flex, Button, Tooltip, Typography, message, Input, Drawer } from 'antd'
import { MessageCard } from '../components/MessageCard'
import { GetAllLastMessagesResponse } from '../Types/MessageType'
import { useEffect, useState } from 'react'
import { GetAllLastMessages, GetMessages, SendMessage } from '../API/Messages'
import { FileAddOutlined, SendOutlined } from '@ant-design/icons'
import { ChatScreen } from '../components/ChatScreen'
import { Notification } from '../Types/NotificationType'
import { User } from '../Types/EntitysType'
import { SearchUserByUserName } from '../API/User'

export const Messages = (): JSX.Element => {
  const [lastMessagesResponse, setLastMessagesResponse] = useState<GetAllLastMessagesResponse[]>([])
  const [toogleMessage, setToogleMessage] = useState<boolean>(false)
  const [selectedUser, setSelectedUser] = useState<string>('')
  const [isModalOpen, setIsModalOpen] = useState(false)
  const [webSocket, setWebSocket] = useState<WebSocket | null>(null)
  const [notification, setNotification] = useState<Notification[]>([])
  const [text, setText] = useState<string>('')
  const [foundUser, setFoundUser] = useState<User>()
  const [lastmessageFilter, setLastMessageFilter] = useState<GetAllLastMessagesResponse[]>([])

  const [messageApi, contextHolder] = message.useMessage()
  const { Search } = Input

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (token) {
      GetAllLastMessages(token).then((res) => {
        if (res) {
          setLastMessagesResponse(res)
          setLastMessageFilter(res)
        }
      })
    }
    const ws = new WebSocket(`${import.meta.env.VITE_WS_URL}/message/ws`)

    // open connection
    ws.onopen = (): void => {
      setWebSocket(ws)
      console.log('connected')
      const request = {
        receiverUsername: '',
        token: token
      }
      ws.send(JSON.stringify(request))
    }

    // listen for messages
    ws.onmessage = (msg): void => {
      console.log(msg.data)
      const data: Notification = JSON.parse(msg.data)

      setNotification((prev) => [...prev, data])

      if (token) {
        GetAllLastMessages(token).then((res) => {
          if (res) {
            setLastMessagesResponse(res)
            setLastMessageFilter(res)
          }
        })
      }
    }

    // close connection
    ws.onclose = (): void => {
      console.log('disconnected')
    }
  }, [])

  const handleToogleMessage = (username: string): void => {
    const token = localStorage.getItem('token')
    localStorage.setItem('selectedUser', username)

    if (!token) return

    GetMessages(token, username).then((res) => {
      if (res) {
        setToogleMessage(true)
        setSelectedUser(username)
      }
    })

    setToogleMessage(false)
    setNotification((prev) => prev.filter((not) => not.senderUserName !== username))
  }

  const handlerSearch = (value: string): void => {
    if (value === '') {
      setLastMessageFilter(lastMessagesResponse)
      return
    }

    const filter = lastMessagesResponse.filter((message) => {
      return message.userName.toLowerCase().includes(value.toLowerCase())
    })

    setLastMessageFilter(filter)
  }

  const handlerSendMessage = (): void => {
    if (text === '' || selectedUser === '') return
    const token = localStorage.getItem('token')
    if (!token) return
    SendMessage(token, text, selectedUser).then((res) => {
      if (!res) {
        messageApi.open({
          type: 'error',
          content: 'Error to send message'
        })
      } else {
        setText('')
        handleToogleMessage(selectedUser)
        GetAllLastMessages(token).then((res) => {
          if (res) {
            setLastMessagesResponse(res)
            setLastMessageFilter(res)
          }
        })
      }
    })

    if (webSocket) {
      const request = {
        receiverUsername: selectedUser,
        token: token
      }

      webSocket.send(JSON.stringify(request))
    }
  }

  const handleSearchUser = (value: string): void => {
    const token = localStorage.getItem('token')
    if (token) {
      SearchUserByUserName(token, value).then((res) => {
        if (res) {
          console.log(res)
          setFoundUser(res)
        }
      })
    }
  }

  const showModal = (): void => {
    setIsModalOpen(true)
  }

  const handleCancel = (): void => {
    setIsModalOpen(false)
  }

  const handleFoundUser = (): void => {
    if (foundUser) {
      handleToogleMessage(foundUser.userName)
      setIsModalOpen(false)
    }
  }

  return (
    <MainLayout key="1">
      {contextHolder}
      <Drawer title="new Message" open={isModalOpen} onClose={handleCancel}>
        <Search placeholder="Search user" allowClear onSearch={(e) => handleSearchUser(e)} />
        {foundUser && (
          <div onClick={handleFoundUser}>
            <MessageCard
              message={''}
              date={''}
              userName={foundUser.userName}
              notificationCount={0}
            />
          </div>
        )}
      </Drawer>
      <Flex>
        <Flex
          vertical={true}
          style={{ minWidth: '25vw', height: '100vh', padding: '10px', background: 'white' }}
        >
          <Flex justify="space-between">
            <Typography.Title level={4}>Messages</Typography.Title>
            <div>
              <Tooltip title="Create a new message">
                <Button onClick={showModal} shape="circle">
                  <FileAddOutlined />
                </Button>
              </Tooltip>
            </div>
          </Flex>
          <Search placeholder="Search" onSearch={(e) => handlerSearch(e)} />
          <Flex vertical={true} align="center" style={{ marginTop: '10px' }}>
            {lastmessageFilter.map((message, index) => {
              return (
                <div
                  key={index}
                  onClick={() => handleToogleMessage(message.userName)}
                  style={{ width: '100%' }}
                >
                  <MessageCard
                    notificationCount={
                      notification.filter((not) => not.senderUserName === message.userName).length
                    }
                    message={message.lastMessage.content}
                    date={message.lastMessage.date}
                    userName={message.userName}
                  />
                </div>
              )
            })}
          </Flex>
        </Flex>

        <Flex vertical={true} style={{ width: '100%' }}>
          {toogleMessage && (
            <>
              <ChatScreen username={selectedUser} notification={notification} />
              <Flex style={{ padding: '10px' }} gap="small" align="center">
                <Input
                  allowClear
                  onChange={(e) => setText(e.target.value)}
                  placeholder="Type a message"
                  onKeyPress={(e) => {
                    if (e.key === 'Enter') {
                      handlerSendMessage()
                    }
                  }}
                />
                <Button shape="circle" onClick={handlerSendMessage}>
                  <SendOutlined />
                </Button>
              </Flex>
            </>
          )}
        </Flex>
      </Flex>
    </MainLayout>
  )
}
