import { useState, useEffect, FC } from 'react'
import '../assets/WebSocketComponent.css'
import { ChatItem } from './ChatItem'

type Message = {
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

  useEffect(() => {
    // get WebSocket IP from localStorage
    const webSocketIp = localStorage.getItem('webSocketIp')

    // WebSocket  connection opened
    const ws = new WebSocket(`ws://${webSocketIp}`)

    // when the connection is open, send some data to the server
    ws.onopen = (): void => {
      console.log('WebSocket connection opened')
    }

    //  message received
    ws.onmessage = (event): void => {
      const receivedMessage = event.data
      const receivedMessageObject: Message = JSON.parse(receivedMessage)
      console.log(`Received: ${receivedMessage}`)
      setMessagesList((prevMessagesList) => [...prevMessagesList, receivedMessageObject])
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
    if (socket) {
      socket.send(`${username}: ${message}`)
      setMessage('')
    }
  }

  return (
    <div className="web-socket-container">
      <div className="chat-container">
        {messagesList.map((message, index) => (
          <ChatItem message={message.message} date={message.date} key={index} />
        ))}
      </div>
      <div className="input-container">
        <input
          onKeyPress={(e) => {
            e.key === 'Enter' && sendMessage()
          }}
          type="text"
          placeholder="text here"
          value={message}
          onChange={(e) => setMessage(e.target.value)}
        />
        <button type="submit" onClick={sendMessage}>
          Send
        </button>
      </div>
    </div>
  )
}

export default WebSocketComponent
