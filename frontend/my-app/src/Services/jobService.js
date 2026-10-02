import api from "../config/axiosConfig"
import sampleJobs from '../data/sampleJobs.json'



function normalizeSkills(skills = []) {
  return (skills || []).map(s => String(s).trim().toLowerCase())
}

export async function getRecommendedJobs(userId, parsedResume) {
  const token = localStorage.getItem('access-token')

  // Try backend endpoint first
  if (API_BASE_URL && userId) {
    try {
      const resp = await api.get(`/jobs/recommended/${userId}`, {
        headers: {
          Authorization: token ? `Bearer ${token}` : undefined,
        },
      })

      if (resp?.data && Array.isArray(resp.data)) {
        return resp.data
      }
    } catch (err) {
      // fallback to local matching; do not leak token or sensitive info in logs
      // console.debug('Job recommendation endpoint failed, using local sample jobs')
    }
  }

  // Local fallback: match against sampleJobs
  const jobs = Array.isArray(sampleJobs) ? sampleJobs : []
  return matchJobsLocally(parsedResume, jobs)
}

export function matchJobsLocally(parsedResume, jobs = []) {
  const resumeSkills = normalizeSkills(parsedResume?.skills || [])

  return jobs.map(job => {
    const jobSkills = normalizeSkills(job.requiredSkills || [])
    const matched = jobSkills.filter(s => resumeSkills.includes(s))
    const missing = jobSkills.filter(s => !resumeSkills.includes(s))
    const score = jobSkills.length > 0 ? Math.round((matched.length / jobSkills.length) * 100) : 0

    // build a short bullet summary from description
    const summary = (job.description || '')
      .split(/[\.\n]+/)
      .map(s => s.trim())
      .filter(Boolean)
      .slice(0, 4)

    return {
      ...job,
      matchedSkills: matched,
      missingSkills: missing,
      score,
      summary,
    }
  }).sort((a, b) => b.score - a.score)
}

async function getProfileData(userId){
    try{
        const token = localStorage.getItem('access-token')
        const resp = await api.get(`/onboarding/getDetails/${userId}`, {
            headers: {
                Authorization: token ? `Bearer ${token}` : undefined,
            },
        })
        localStorage.setItem('profileData', JSON.stringify(resp.data))
        return resp.data;
    }catch(err){
        console.error('Error fetching profile data:', err)
        return null
    }
}

export default { getRecommendedJobs, matchJobsLocally }
