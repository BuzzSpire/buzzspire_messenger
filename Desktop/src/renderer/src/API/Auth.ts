import { LoginResponse, RegisterResponse } from '../Types/AuthTypes'

const API = import.meta.env.VITE_API_URL

export const RegisterRequest = async (
  username: string,
  password: string,
  fullName: string
): Promise<RegisterResponse> => {
  return await fetch(`${API}/auth/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ username, password, fullName })
  }).then((response) => response.json())
}

export const LoginRequest = async (username: string, password: string): Promise<LoginResponse> => {
  return await fetch(`${API}/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ username, password })
  }).then((response) => response.json())
}
