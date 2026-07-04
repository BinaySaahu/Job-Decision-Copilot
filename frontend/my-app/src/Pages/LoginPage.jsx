import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../Context/AuthContext'
import AuthComponet from '../components/AuthComponet'

export default function LoginPage() {
  const navigate = useNavigate()

  return (
    <div className="page-shell">
      <div className="auth-card">
        <h1>Sign in</h1>
        <p>Welcome back to your job decision workspace.</p>

        <AuthComponet isLogin={true} />

        <p className="auth-link">
          New here? <button onClick={() => navigate('/register')}>Create an account</button>
        </p>
      </div>
    </div>
  )
}
