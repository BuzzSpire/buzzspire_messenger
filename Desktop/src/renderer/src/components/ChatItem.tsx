import '../assets/ChatItem.css'
import { FC } from 'react'

interface ChatItemProps {
  message: string
  date: string
}

export const ChatItem: FC<ChatItemProps> = ({ message, date }) => {
  return (
    <div className="container">
      <p>{message}</p>
      <p>{date}</p>
    </div>
  )
}
