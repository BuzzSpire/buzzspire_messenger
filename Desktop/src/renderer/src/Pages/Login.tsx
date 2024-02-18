import { LockOutlined, UserOutlined } from '@ant-design/icons'
import { Alert, Button, Checkbox, Flex, Form, Input } from 'antd'
import { LoginForm } from '../Types/FormsType'
import { LoginRequest } from '../API/Auth'
import { useState } from 'react'

export const Login: React.FC = () => {
  const [isError, setIsError] = useState(false)

  const onFinish = (values: LoginForm): void => {
    LoginRequest(values.UserName, values.Password).then((res) => {
      const regex = /^[\w-]+\.[\w-]+\.[\w-]+$/g.test(res.token)
      if (regex) {
        localStorage.setItem('username', values.UserName)
        localStorage.setItem('token', res.token)
        window.location.href = '/home'
      } else {
        setIsError(true)
      }
    })
  }

  return (
    <Flex justify="center" vertical={true} align="center" style={{ height: '100vh' }}>
      <Form
        name="normal_login"
        className="login-form"
        initialValues={{ remember: true }}
        onFinish={onFinish}
        style={{ width: '50%' }}
      >
        <Form.Item
          name="UserName"
          rules={[{ required: true, message: 'Please input your Username!' }]}
        >
          <Input prefix={<UserOutlined className="site-form-item-icon" />} placeholder="Username" />
        </Form.Item>
        <Form.Item
          name="Password"
          rules={[{ required: true, message: 'Please input your Password!' }]}
        >
          <Input
            prefix={<LockOutlined className="site-form-item-icon" />}
            type="password"
            placeholder="Password"
          />
        </Form.Item>
        <Form.Item>
          <Form.Item name="Remember" valuePropName="checked" noStyle>
            <Checkbox>Remember me</Checkbox>
          </Form.Item>
        </Form.Item>

        {isError && (
          <Alert
            style={{ margin: '10px' }}
            message="Error"
            description="Username or Password is incorrect"
            type="error"
            showIcon
          />
        )}

        <Form.Item>
          <Flex gap="small" align="center">
            <Button type="primary" htmlType="submit" className="login-form-button">
              Log in
            </Button>
            Or <a href="/register">register now!</a>
          </Flex>
        </Form.Item>
      </Form>
    </Flex>
  )
}
