import WebSocketComponent from './components/WebSocketComponent'
import { Login } from './components/Login'
import { useState } from 'react'
import './assets/main.css'
import image from './assets/loading.gif'

const App = (): JSX.Element => {
  const [username, setUsername] = useState('')

  const handleUsernameChange = (name: string): void => {
    setUsername(name)
  }

  return (
    <div>
      <img className="image" src={image} alt="loading" />
      {username === '' ? (
        <Login setUsername={handleUsernameChange} />
      ) : (
        <WebSocketComponent username={username} />
      )}
    </div>
  )
}

export default App
