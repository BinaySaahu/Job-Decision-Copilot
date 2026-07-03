import { useNavigate } from 'react-router-dom'
import { useAuth } from './Context/AuthContext'

export default function DashboardPage() {
  const navigate = useNavigate()
  const { user, logout, isAuthenticated } = useAuth()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="page-shell">
      <div className="auth-card">
        <h1>Dashboard</h1>
        <p>Welcome, {user?.fullName || 'there'}.</p>
        <p>You are now signed in.</p>

        <button onClick={handleLogout}>Logout</button>
      </div>
    </div>
  )
}
