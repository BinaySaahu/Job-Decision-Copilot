import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../Context/AuthContext";

const AuthComponet = ({isLogin}) => {
  const navigate = useNavigate();
  const { register, loading, login } = useAuth();
  const [form, setForm] = useState({ fullName: "", email: "", password: "" });
  const [message, setMessage] = useState("");

  const handleChange = (event) => {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  };

  const handleSubmit = async (event) => {
    event.preventDefault();
    if(!form.email || !form.password || (!isLogin && !form.fullName)) {
      setMessage("Please fill in all required fields.");
      return;
    }
    const result = isLogin ? await login(form.email, form.password) : await register(form.email, form.password, form.fullName);
    

    if (result.success) {
      navigate("/dashboard");
    } else {
      setMessage(result.message || "Request failed.");
    }
  };
  return (
    <div>
      <form className="auth-form" onSubmit={handleSubmit}>
        {!isLogin && <label>
          <span>Full name</span>
          <input
            type="text"
            name="fullName"
            value={form.fullName}
            onChange={handleChange}
            required
          />
        </label>}

        <label>
          <span>Email</span>
          <input
            type="email"
            name="email"
            value={form.email}
            onChange={handleChange}
            required
          />
        </label>

        <label>
          <span>Password</span>
          <input
            type="password"
            name="password"
            value={form.password}
            onChange={handleChange}
            required
            minLength="6"
          />
        </label>

        {message ? <p className="form-message">{message}</p> : null}

        <button type="submit" disabled={loading}>
          {loading ? (isLogin ? "Logging in..." : "Creating account...") : isLogin ? "Login" : "Create account"}
        </button>
      </form>
    </div>
  );
};

export default AuthComponet;
