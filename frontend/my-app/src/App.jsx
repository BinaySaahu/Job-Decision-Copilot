import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { AuthProvider, useAuth } from './Context/AuthContext'
import LoginPage from './Pages/LoginPage'
import RegisterPage from './Pages/RegisterPage'
// import onboardPage from './onboardPage'
import DashboardPage from './DashboardPage'
import ProtectedRoute from './ProtectedRoute'
import './App.css'
import OnBoardingPage from './Pages/OnBoardingPage'

function AppRoutes() {
  const { isAuthenticated, isOnboarded } = useAuth()

  return (
    <Routes>
      <Route path="/login" element={isAuthenticated ? <Navigate to={isOnboarded ? '/dashboard' : '/onboard'} replace /> : <LoginPage />} />
      <Route path="/register" element={isAuthenticated ? <Navigate to={isOnboarded ? '/dashboard' : '/onboard'} replace /> : <RegisterPage />} />
      <Route
        path="/onboard"
        element={
          <ProtectedRoute>
            {isOnboarded ? <Navigate to="/dashboard" replace /> : <OnBoardingPage />}
          </ProtectedRoute>
        }
      />
      <Route path="/dashboard" element={<ProtectedRoute><DashboardPage/></ProtectedRoute>} />
      <Route path="*" element={<Navigate to={isAuthenticated ? (isOnboarded ? '/dashboard' : '/onboard') : '/login'} replace />} />
    </Routes>
  )
}

function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </AuthProvider>
  )
}

export default App
