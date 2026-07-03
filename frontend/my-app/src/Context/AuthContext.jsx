import { createContext, useContext, useEffect, useMemo, useState } from 'react'
import { loginUser, registerUser, logoutUser } from '../Services/authService'

const AuthContext = createContext(null)

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem('user')
    return JSON.parse(savedUser)
  })

  const [token, setToken] = useState(() => localStorage.getItem('access-token'))
  const [refreshtoken, setRefreshToken] = useState(() => localStorage.getItem('refresh-token'))

  const [loading, setLoading] = useState(false)

  // useEffect(() => {
  //   if (user) {
  //     localStorage.setItem('user', JSON.stringify(user))
  //   } else {
  //     localStorage.removeItem('user')
  //   }
  // }, [user])

  // useEffect(() => {
  //   if (token) {
  //     localStorage.setItem('job-decision-token', token)
  //   } else {
  //     localStorage.removeItem('job-decision-token')
  //   }
  // }, [token])

  const login = async (email, password) => {
    setLoading(true)
    try {
      const result = await loginUser(email, password)
      if (result.success) {
        setUser(result.user)
        setToken(result.accessToken)
        setRefreshToken(result.refreshToken)
        return { success: true, message: "Login successful." }
      }
      return { success: false, message: result.message }
    } finally {
      setLoading(false)
    }
  }

  const register = async (email, password, fullName) => {
    setLoading(true)
    try {
      const result = await registerUser(email, password, fullName)
      if (result.success) {
        setUser(result.user)
        setToken(result.accessToken)
        setRefreshToken(result.refreshToken)
        return { success: true, message: "Registration successful." }
      }
      return { success: false, message: result.message }
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    setUser(null)
    setToken(null)
    setRefreshToken(null)
    logoutUser()
  }

  const value = useMemo(
    () => ({
      user,
      token,
      refreshtoken,
      loading,
      isAuthenticated: Boolean(token && user),
      login,
      register,
      logout,
    }),
    [user, token, loading]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used inside an AuthProvider')
  }
  return context
}
