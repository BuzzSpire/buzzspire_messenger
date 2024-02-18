import { GetAllLastMessagesResponse } from '../Types/MessageType'
import { Message } from '../Types/EntitysType'

const API = import.meta.env.VITE_API_URL

export const GetAllLastMessages = async (token: string): Promise<GetAllLastMessagesResponse[]> => {
  const respoense = await fetch(`${API}/Message/getAllLastMessages`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      token: token
    }
  })
  return respoense.json()
}

export const GetMessages = async (token: string, username: string): Promise<Message[]> => {
  const respoense = await fetch(`${API}/Message/${username}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      token: token
    }
  })
  return respoense.json()
}

export const SendMessage = async (
  token: string,
  message: string,
  receiverUsername: string
): Promise<boolean> => {
  const respoense = await fetch(`${API}/Message/send`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      token: token
    },
    body: JSON.stringify({
      receiverUsername: receiverUsername,
      content: message
    })
  })
  return respoense.ok
}
