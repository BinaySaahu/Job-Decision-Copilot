import { useNavigate } from "react-router-dom";
import { useAuth } from "./Context/AuthContext";
import { useEffect, useState } from "react";
import { getOnboardingProfile } from "./Services/onboardingService";

export default function DashboardPage() {
  const navigate = useNavigate();
  const { user, logout, isAuthenticated, isOnboarded } = useAuth();
  const [loading, setLoading] = useState(false);
  const [profileData, setProfileData] = useState(null);

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  const getData = async () => {
    setLoading(true);
    try {
      const res = await getOnboardingProfile(user?.id);
      if (!res.message) {
        console.log("Onboarding profile data:", res);
        setProfileData(res);
      }
    } catch (err) {
      console.error("Error fetching onboarding profile:", err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    // if (isAuthenticated && isOnboarded) {
      getData();
    // }
  }, []);
  return (
    <div className="page-shell">
      <div className="auth-card">
        <h1>Dashboard</h1>
        <p>Welcome, {user?.fullName || "there"}.</p>
        <p>You are now signed in.</p>

        <button onClick={handleLogout}>Logout</button>
      </div>
    </div>
  );
}
