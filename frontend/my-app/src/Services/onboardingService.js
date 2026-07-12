import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_BASE_URL

export async function submitOnboarding(payload) {
  const token = localStorage.getItem('access-token')
  const formData = new FormData()

  formData.append('userId', payload.userId)
  formData.append('experienceYears', payload.experienceYears || '')
  formData.append('employmentType', payload.employmentType || '')

  if (payload.interestedRoles?.length) {
    payload.interestedRoles.forEach((role) => {
      formData.append('interestedRoles', role)
    })
  }

  if (payload.resume) {
    formData.append('resume', payload.resume)
  }

  const response = await axios.post(`${API_BASE_URL}/onboarding`, formData, {
    headers: {
      Authorization: `Bearer ${token}`,
      'Content-Type': 'multipart/form-data',
    },
  })

  return response.data
}

export async function getOnboardingProfile(userId) {
    const token = localStorage.getItem('access-token')

    const response = await axios.get(`${API_BASE_URL}/onboarding/getDetails/${userId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    })

    return response.data
}
