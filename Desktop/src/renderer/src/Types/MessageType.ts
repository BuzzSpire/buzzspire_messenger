export type Message = {
  senderId: number
  sender: null
  receiverId: number
  receiver: null
  date: string
  content: string
  id: number
}

export type LastMessage = {
  userName: string
  userId: number
  lastMessage: Message
}

export type GetAllLastMessagesResponse = LastMessage
