import WebSocketComponent from './components/WebSocketComponent'
import { Login } from './components/Login'
import { useEffect, useState } from 'react'
import './assets/main.css'

const App = (): JSX.Element => {
  const [username, setUsername] = useState('')

  useEffect(() => {
    const username = localStorage.getItem('username')
    if (username) {
      setUsername(username)
    }
  }, [])

  const handleUsernameChange = (name: string): void => {
    setUsername(name)
    localStorage.setItem('username', name)
  }

  return (
    <div>
      {username === '' ? (
        <Login setUsername={handleUsernameChange} />
      ) : (
        <WebSocketComponent username={username} />
      )}
    </div>
  )
}

export default App
