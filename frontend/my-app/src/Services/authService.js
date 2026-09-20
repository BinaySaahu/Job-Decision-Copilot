import axios from 'axios'
// import {useProfile} from '../Context/ProfileContext'

export async function loginUser(email, password) {
  // await delay(700)
  const URL = import.meta.env.VITE_BASE_URL + "/auth/login"
  // const { clearProfile } = useProfile()
  
  try{
    const response = await axios.post(URL, {email, password}, {withCredentials: true})
    console.log("Login response:", response.data)
    if(response.data.success === false){
      throw new Error(response.data.message || "Login failed.")
    }
    localStorage.setItem('access-token', response.data.accessToken)
    localStorage.setItem('user', JSON.stringify(response.data.user))
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
    const response = await axios.post(URL, {email, password, fullName}, { withCredentials: true })
    console.log("Registration response:", response.data)
    if(response.data.success === false){
      throw new Error(response.data.message || "Registration failed.")
    }
    localStorage.setItem('access-token', response.data.accessToken)
    localStorage.setItem('user', JSON.stringify(response.data.user))
    return response.data
  }catch (error) {
    console.error("Registration error:", error)
    return {
      success: false,
      message: error.message
    }
  }
}

export async function refresh() {
  const URL = import.meta.env.VITE_BASE_URL + "/auth/refresh"

  try {
    const response = await axios.post(URL,{}, { withCredentials: true })
    console.log("Refresh token response:", response.data)
    if (response.data.success === false) {
      throw new Error(response.data.message || "Failed to refresh token.")
    }
    localStorage.setItem('access-token', response.data.accessToken)
    return true
  } catch (error) {
    console.error("Refresh token error:", error)
    return {
      success: false,
      message: error.message
    }
  }
}
