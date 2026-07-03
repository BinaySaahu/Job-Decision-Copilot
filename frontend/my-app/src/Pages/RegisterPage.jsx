import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../Context/AuthContext'
import AuthComponet from '../components/AuthComponet'

export default function RegisterPage() {

  return (
    <div className="page-shell">
      <div className="auth-card">
        <h1>Create account</h1>
        <p>Start your job matching journey.</p>

        <AuthComponet isLogin={false} />

        <p className="auth-link">
          Already have an account? <button onClick={() => navigate('/login')}>Sign in</button>
        </p>
      </div>
    </div>
  )
}
