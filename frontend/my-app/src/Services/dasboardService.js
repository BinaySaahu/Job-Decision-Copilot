import api from "../config/axiosConfig"

export async function getProfile(userId) {
    let response = null
    try{
        const token = localStorage.getItem('access-token')
    
        response = await api.get(`/onboarding/getDetails/${userId}`, {
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
