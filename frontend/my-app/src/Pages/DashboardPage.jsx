import { useNavigate } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";
import { useProfile } from "../Context/ProfileContext";
import { useEffect, useState } from "react";
import sampleJobs from "../data/sampleJobs.json";
import ProfileSummary from "../components/ProfileSummary";
import JobList from "../components/JobList";
import "./css/Dashboard.css";
import { getProfile } from "../Services/dasboardService";

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const { profile, loadProfile, clearProfile } = useProfile();

  const [loadingProfile, setLoadingProfile] = useState(false);
  const [profileData, setProfileData] = useState(null);
  const [jobs, setJobs] = useState([]);
  const [jobsLoading, setJobsLoading] = useState(false);
  const [error, setError] = useState(null);

  const handleLogout = () => {
    clearProfile();
    logout();
  };

  const getProfileData = async () => {
    const profData = await loadProfile(user.id);
    if (!profData) {
      setError("Failed to load profile data.");
      return;
    }
    setProfileData(profile);
  };

  useEffect(() => {
    if (user?.id) {
      getProfileData();
    }
    setJobs(sampleJobs || []);
    setProfileData(null);
  }, [user?.id]);

  return (
    <div className="app-shell container">
      <div
        style={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          marginBottom: 12,
        }}
      >
        <h1>Dashboard</h1>
        <button className="btn btn-ghost" onClick={handleLogout}>
          Logout
        </button>
      </div>

      {error && (
        <div className="auth-card" style={{ color: "var(--danger)" }}>
          {error}
        </div>
      )}

      <ProfileSummary user={user} profile={profile} />

      <section className="card" style={{ marginTop: 16 }}>
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          <h2 style={{ margin: 0 }}>Recommended Jobs</h2>
          <div className="muted">{jobs.length} results</div>
        </div>

        <div style={{ marginTop: 12 }}>
          {jobsLoading ? <div>Loading jobs...</div> : <JobList jobs={jobs} />}
        </div>
      </section>
    </div>
  );
}
