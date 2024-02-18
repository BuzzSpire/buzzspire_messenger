import { User } from '../Types/EntitysType'

const API = import.meta.env.VITE_API_URL

export const SearchUserByUserName = async (token: string, username: string): Promise<User> => {
  const respoense = await fetch(`${API}/User/search/${username}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      token: token
    }
  })
  return respoense.json()
}
