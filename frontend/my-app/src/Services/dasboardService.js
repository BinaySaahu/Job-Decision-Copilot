import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_BASE_URL

export async function getProfile(userId) {
    let response = null
    try{
        const token = localStorage.getItem('access-token')
    
        response = await axios.get(`${API_BASE_URL}/onboarding/getDetails/${userId}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        })
        
        return response.data

    }catch(error){
        console.error('Error fetching profile data:', error.response)
        return error.response
    }finally{
        console.log("Server Response: ", response)
        console.log("Profile function completed");
    }
    
}
