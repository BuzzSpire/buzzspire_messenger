import { FC, ReactNode } from 'react'
import { Badge, Flex, Layout, Menu } from 'antd'
import {
  MessageOutlined,
  UsergroupAddOutlined,
  SettingFilled,
  LogoutOutlined
} from '@ant-design/icons'

interface MainLayoutProps {
  children?: ReactNode
  key?: string
}

export const MainLayout: FC<MainLayoutProps> = ({ children, key = '1' }): JSX.Element => {
  const { Sider, Content } = Layout

  const logOut = (): void => {
    localStorage.removeItem('token')
    localStorage.removeItem('username')
    window.location.href = '/'
  }

  return (
    <Layout>
      <Sider
        trigger={null}
        collapsible
        collapsed={true}
        style={{ background: 'white', color: 'black' }}
      >
        <Menu
          theme="light"
          mode="inline"
          defaultSelectedKeys={[key]}
          items={[
            {
              key: '1',
              title: 'Messages',
              icon: (
                <div>
                  <Badge dot={false}>
                    <MessageOutlined />
                  </Badge>
                </div>
              ),
              label: 'Messages',
              onClick: () => (window.location.href = '/messages')
            },
            {
              key: '2',
              title: 'Setting',
              icon: <SettingFilled />,
              label: 'Setting'
            },
            {
              key: '3',
              title: 'Logout',
              icon: <LogoutOutlined />,
              label: 'Logout',
              danger: true,
              onClick: () => logOut()
            }
          ]}
        />
      </Sider>
      <Layout>
        <Content
          style={{
            minHeight: '100vh'
          }}
        >
          {children}
        </Content>
      </Layout>
    </Layout>
  )
}
