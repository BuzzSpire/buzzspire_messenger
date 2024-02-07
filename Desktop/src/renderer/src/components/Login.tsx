import '../assets/Login.css'
import { FC, useState } from 'react'
import { Input, Button, Flex, Typography, message } from 'antd'

interface LoginProps {
  setUsername: (username: string) => void
}

export const Login: FC<LoginProps> = ({ setUsername }) => {
  const [messageApi, contextHolder] = message.useMessage()
  const [getUsername, setGetUsername] = useState<string>('')
  const [webSocketIp, setWebSocketIp] = useState<string>('')

  const handleLogin = (): void => {
    if (getUsername === '' || webSocketIp === '') {
      error()
      return
    }
    setUsername(getUsername)
    localStorage.setItem('webSocketIp', webSocketIp)
  }

  const error = (): void => {
    messageApi.open({
      type: 'error',
      content: 'Please enter a valid username and WebSocket IP'
    })
  }

  return (
    <Flex justify="center" align="center" style={{ height: '100vh' }}>
      {contextHolder}
      <Flex vertical={true} gap="middle">
        <h2>Login</h2>
        <Typography.Text>Enter your username and WebSocket IP</Typography.Text>
        <Input
          type="text"
          value={getUsername}
          onChange={(e) => setGetUsername(e.target.value)}
          placeholder="Enter Username"
          name="uname"
          required
        />

        <Input
          type="text"
          addonBefore="ws://"
          value={webSocketIp}
          onChange={(e) => setWebSocketIp(e.target.value)}
          placeholder="Enter WebSocket IP"
          name="webSocketIp"
          required
        />

        <Button type="primary" onClick={handleLogin}>
          Login
        </Button>
      </Flex>
    </Flex>
  )
}
