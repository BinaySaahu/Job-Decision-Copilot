import api from "../config/axiosConfig"

export async function loginUser(email, password) {
  
  try{
    const response = await api.post("/auth/login", {email, password})
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

  try{
    const response = await api.post("/auth/register", {email, password, fullName})
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

  try {
    const response = await api.post("/auth/refresh", {})
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
