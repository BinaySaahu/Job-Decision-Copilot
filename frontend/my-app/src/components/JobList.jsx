import React from 'react'
import JobCard from './JobCard'

export default function JobList({ jobs }) {
  if (!jobs) return null

  if (jobs.length === 0) {
    return <div className="auth-card">No job recommendations available right now.</div>
  }

  return (
    <section className="jobs-grid">
      {jobs.map(job => (
        <JobCard key={job.id} job={job} />
      ))}
    </section>
  )
}
