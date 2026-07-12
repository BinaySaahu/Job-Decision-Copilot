import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../Context/AuthContext'
import { submitOnboarding } from '../Services/onboardingService'

const employmentTypes = ['Full Time', 'Part Time', 'Contract', 'Internship']

const OnBoardingPage = () => {
  const navigate = useNavigate()
  const { user, logout } = useAuth()
  const [form, setForm] = useState({
    experienceYears: '',
    interestedRoles: '',
    employmentType: '',
    resume: null,
  })
  const [loading, setLoading] = useState(false)
  const [message, setMessage] = useState('')

  const handleChange = (event) => {
    const { name, value } = event.target
    setForm((current) => ({ ...current, [name]: value }))
  }

  const handleFileChange = (event) => {
    const file = event.target.files?.[0] || null
    setForm((current) => ({ ...current, resume: file }))
  }

  const handleSubmit = async (event) => {
    event.preventDefault()
    setLoading(true)
    setMessage('')

    try {
      const payload = {
        userId: user?.id,
        experienceYears: form.experienceYears,
        interestedRoles: form.interestedRoles
          .split(',')
          .map((role) => role.trim())
          .filter(Boolean),
        employmentType: form.employmentType,
        resume: form.resume,
      }

      const response = await submitOnboarding(payload)
      if (response?.message) {
        localStorage.setItem('is-onboarded', 'true')
        navigate('/dashboard')
      }
    } catch (error) {
      setMessage(error?.response?.data?.message || 'Unable to complete onboarding.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="page-shell">
      <div className="auth-card">
        <h1>Complete your profile</h1>
        <p>Upload your resume and share your preferred roles so we can tailor job matches.</p>

        <form className="auth-form" onSubmit={handleSubmit}>
          <label>
            <span>Years of experience</span>
            <input
              type="number"
              name="experienceYears"
              min="0"
              value={form.experienceYears}
              onChange={handleChange}
              placeholder="e.g. 3"
            />
          </label>

          <label>
            <span>Preferred job roles</span>
            <input
              type="text"
              name="interestedRoles"
              value={form.interestedRoles}
              onChange={handleChange}
              placeholder="Backend Engineer, Data Analyst"
            />
          </label>

          <label>
            <span>Employment type</span>
            <select name="employmentType" value={form.employmentType} onChange={handleChange}>
              <option value="">Select an option</option>
              {employmentTypes.map((type) => (
                <option key={type} value={type}>
                  {type}
                </option>
              ))}
            </select>
          </label>

          <label>
            <span>Resume (PDF)</span>
            <input type="file" accept=".pdf" onChange={handleFileChange} />
          </label>

          {message ? <p className="form-message">{message}</p> : null}

          <button type="submit" disabled={loading}>
            {loading ? 'Saving...' : 'Save profile'}
          </button>
        </form>
      </div>
    </div>
  )
}

export default OnBoardingPage
