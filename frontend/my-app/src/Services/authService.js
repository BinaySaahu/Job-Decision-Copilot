import axios from 'axios'

function readStoredUsers() {
  try {
    return JSON.parse(localStorage.getItem('user') || '[]')
  } catch {
    return []
  }
}

function writeStoredUsers(users) {
  localStorage.setItem('user', JSON.stringify(users))
}

export async function loginUser(email, password) {
  // await delay(700)
  const URL = import.meta.env.VITE_BASE_URL + "/auth/login"
  
  try{
    const response = await axios.post(URL, {email, password})
    console.log("Login response:", response.data)
    if(response.data.success === false){
      throw new Error(response.data.message || "Login failed.")
    }
    localStorage.setItem('access-token', response.data.accessToken)
    localStorage.setItem('user', JSON.stringify(response.data.user))
    localStorage.setItem('refresh-token', response.data.refreshToken)
    return response.data
  } catch (error) {
    console.error("Login error:", error)
    return {
      success: false,
      message: error.message
    }
  }

}

export async function registerUser(email, password, fullName) {
  const URL = import.meta.env.VITE_BASE_URL + "/auth/register"

  try{
    const response = await axios.post(URL, {email, password, fullName})
    console.log("Registration response:", response.data)
    if(response.data.success === false){
      throw new Error(response.data.message || "Registration failed.")
    }
    localStorage.setItem('access-token', response.data.accessToken)
    localStorage.setItem('user', JSON.stringify(response.data.user))
    localStorage.setItem('refresh-token', response.data.refreshToken)
    return response.data
  }catch (error) {
    console.error("Registration error:", error)
    return {
      success: false,
      message: error.message
    }
  }
}

export function logoutUser() {
  localStorage.removeItem('access-token')
  localStorage.removeItem('user')
  localStorage.removeItem('refresh-token')
}
