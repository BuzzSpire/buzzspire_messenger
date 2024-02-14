import { Flex, Spin } from 'antd'
import { useEffect, useState } from 'react'
import { useAuth } from '../hooks/useAuth'
import { MainLayout } from '../layout/MainLayout'

export const Home = (): JSX.Element => {
  const [isLogin, setIsLogin] = useState(true)

  useEffect(() => {
    const token = localStorage.getItem('token')
    if (token) {
      useAuth(token).then((res) => {
        if (res) {
          setIsLogin(false)
        } else {
          window.location.href = '/login'
        }
      })
    } else {
      window.location.href = '/login'
    }
  }, [])

  return (
    <Flex style={{ minHeight: '100vh', height: '100%' }}>
      {isLogin && <Spin fullscreen />}

      <MainLayout></MainLayout>
    </Flex>
  )
}
