const API = import.meta.env.VITE_API_URL

export const useAuth = async (token: string): Promise<boolean> => {
  const response = await fetch(`${API}/auth`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      token: token
    }
  })

  return response.status === 200
}
