import '../assets/Login.css'
import { FC, useState } from 'react'

interface LoginProps {
  setUsername: (username: string) => void
}

export const Login: FC<LoginProps> = ({ setUsername }) => {
  const [getUsername, setGetUsername] = useState<string>('')
  const [webSocketIp, setWebSocketIp] = useState<string>('')

  const handleLogin = (): void => {
    if (getUsername === '' || webSocketIp === '') {
      return
    }
    setUsername(getUsername)
    localStorage.setItem('webSocketIp', webSocketIp)
  }

  return (
    <div className="login-container">
      <div className="forum-container">
        <label htmlFor="uname">
          <b>Username</b>
        </label>
        <input
          type="text"
          value={getUsername}
          onChange={(e) => setGetUsername(e.target.value)}
          placeholder="Enter Username"
          name="uname"
          required
        />

        <label htmlFor="webSocketIp">
          <b>WebSocket IP</b>
        </label>
        <input
          type="text"
          value={webSocketIp}
          onChange={(e) => setWebSocketIp(e.target.value)}
          placeholder="Enter WebSocket IP"
          name="webSocketIp"
          required
        />

        <button onClick={handleLogin} type="submit">
          Login
        </button>
      </div>
    </div>
  )
}
