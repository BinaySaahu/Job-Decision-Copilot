import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { loginUser, registerUser, refresh } from "../Services/authService";
import { useNavigate } from "react-router-dom";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const navigate = useNavigate();
  const [user, setUser] = useState(() => {
    const savedUser = localStorage.getItem("user");
    return JSON.parse(savedUser);
  });

  const [token, setToken] = useState(() =>
    localStorage.getItem("access-token"),
  );
  // const [refreshtoken, setRefreshToken] = useState(() =>
  //   localStorage.getItem("refresh-token"),
  // );
  const [isOnboarded, setIsOnboarded] = useState(() => {
    const savedOnboarded = localStorage.getItem("is-onboarded");
    return savedOnboarded === "true";
  });

  const [loading, setLoading] = useState(false);

  const login = async (email, password) => {
    setLoading(true);
    try {
      const result = await loginUser(email, password);
      if (result.success) {
        saveUser(result);
        return { success: true, message: "Login successful." };
      }
      return { success: false, message: result.message };
    } finally {
      setLoading(false);
    }
  };

  const register = async (email, password, fullName) => {
    setLoading(true);
    try {
      const result = await registerUser(email, password, fullName);
      if (result.success) {
        saveUser(result);
        return { success: true, message: "Registration successful." };
      }
      return { success: false, message: result.message };
    } finally {
      setLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    setIsOnboarded(false);
    localStorage.removeItem("access-token");
    localStorage.removeItem("user");
    localStorage.removeItem("refresh-token");
    localStorage.removeItem("is-onboarded");
    localStorage.removeItem("profile");
    navigate("/login");
  };

  const refreshToken = async () => {
    try{
      const resp = await refresh();

    }catch (error) {
      console.error("Refresh token error:", error)
      return false;
    }
    return true;
  };

  const saveUser = (result) => {
    setUser(result.user);
    setToken(result.accessToken);
    const onboarded = result.user?.isOnboarded || false;
    setIsOnboarded(onboarded);
    localStorage.setItem("is-onboarded", onboarded ? "true" : "false");
  };


  const value = useMemo(
    () => ({
      user,
      token,
      // refreshtoken,
      loading,
      isAuthenticated: Boolean(token && user),
      isOnboarded,
      login,
      register,
      logout,
      refreshToken,
    }),
    [user, token, loading],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used inside an AuthProvider");
  }
  return context;
}
