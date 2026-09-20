import React from 'react'

export default function JobCard({ job }) {
  if (!job) return null

  return (
    <article className="job-card">
      <header className="job-card-header">
        <div>
          <h3 className="job-title">{job.title}</h3>
          <div className="job-company">{job.company}</div>
        </div>
        <div>
          <span className="match-badge">{job.score}% match</span>
        </div>
      </header>

      <ul className="job-summary">
        {job.summary && job.summary.map((line, idx) => (
          <li key={idx}>{line}</li>
        ))}
      </ul>

      <div className="job-skills">
        <div>
          <strong>Matched</strong>
          <ul>
            {job.matchedSkills && job.matchedSkills.length ? job.matchedSkills.map((s, i) => <li key={i}>{s}</li>) : <li>None</li>}
          </ul>
        </div>
        <div>
          <strong>Missing</strong>
          <ul>
            {job.missingSkills && job.missingSkills.length ? job.missingSkills.map((s, i) => <li key={i}>{s}</li>) : <li>None</li>}
          </ul>
        </div>
      </div>
      <div className="job-apply-wrap">
        {job.applyUrl ? (
          <a href={job.applyUrl} target="_blank" rel="noopener noreferrer" className="job-apply-btn">Apply</a>
        ) : (
          <button className="job-apply-btn" disabled>Apply</button>
        )}
      </div>
    </article>
  )
}
