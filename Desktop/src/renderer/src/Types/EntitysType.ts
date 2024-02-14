export type User = {
  userName: string
  password: string
  fullName: string
  bio: string
  id: number
}

export type Message = {
  senderId: number
  sender: User
  receiverId: number
  receiver: User
  date: string
  content: string
  id: number
}
