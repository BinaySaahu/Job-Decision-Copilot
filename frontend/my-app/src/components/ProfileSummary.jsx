import React from 'react'

export default function ProfileSummary({ user, profile }) {
  if (!user && !profile) return null

  return (
    <section className="card profile-summary" style={{display: 'block'}}>
      <h2 style={{marginBottom: 8}}>Profile</h2>
      <dl style={{display: 'grid', gridTemplateColumns: '160px 1fr', gap: '8px 16px', alignItems: 'center'}}>
        <dt className="muted">Name</dt>
        <dd>{user?.fullName || profile?.userName || '—'}</dd>

        <dt className="muted">Email</dt>
        <dd>{user?.email || profile?.userEmail || '—'}</dd>

        <dt className="muted">Experience</dt>
        <dd>{profile?.experienceYears ?? 'Not provided'}</dd>

        <dt className="muted">Employment type</dt>
        <dd>{profile?.employmentType ?? 'Not provided'}</dd>

        <dt className="muted">Interested roles</dt>
        <dd>{(profile?.interestedRoles && profile.interestedRoles.length) ? profile.interestedRoles.join(', ') : '—'}</dd>
      </dl>
    </section>
  )
}
