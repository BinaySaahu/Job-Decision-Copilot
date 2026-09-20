import { createContext, useContext, useMemo, useState } from 'react'
import { getProfile } from '../Services/dasboardService'
import { useAuth } from './AuthContext'

const ProfileContext = createContext(null)

export function ProfileProvider({ children }) {
  const { logout, refreshToken } = useAuth()
  const [profile, setProfile] = useState(() => {
    try {
      const saved = localStorage.getItem('profile')
      return saved ? JSON.parse(saved) : null
    } catch {
      return null
    }
  })

  const saveProfile = (p) => {
    setProfile(p)
    try {
      if (p) localStorage.setItem('profile', JSON.stringify(p))
      else localStorage.removeItem('profile')
    } catch {}
  }

  const clearProfile = () => saveProfile(null)

  async function loadProfile(userId) {
    // if (profile) return profile;
    let res = null
    if (!userId) return null
    try {
      res = await getProfile(userId)
      if(res.status === 401 || res.status === 403){
        if(await refreshToken()){
          res = await getProfile(userId)
        }else{
          clearProfile()
          logout()

        }
      }
      if (!res || res.message) return null
      saveProfile(res)
      return res
    } catch (err) {
      return null
    }
  }

  const value = useMemo(() => ({ profile, setProfile: saveProfile, clearProfile, loadProfile }), [profile])

  return <ProfileContext.Provider value={value}>{children}</ProfileContext.Provider>
}

export function useProfile() {
  const ctx = useContext(ProfileContext)
  if (!ctx) throw new Error('useProfile must be used inside a ProfileProvider')
  return ctx
}

export default ProfileContext
