import { useState } from "react";
import "./styles/AdminLogin.css";

import logo from "../assets/logo.png";

import {
  FiEye,
  FiEyeOff,
  FiLock,
  FiMail,
  FiShield,
} from "react-icons/fi";

export default function AgentLogin() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [showPassword, setShowPassword] =
    useState(false);

  const [loading, setLoading] =
    useState(false);

  const login = async () => {
    if (!email || !password) {
      alert("Please enter your email and password.");
      return;
    }

    try {
      setLoading(true);

      const response = await fetch(
        "http://localhost:5025/api/Auth/login",
        {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
          },

          body: JSON.stringify({
            email,
            password,
          }),
        }
      );

      if (!response.ok) {
        alert("Invalid credentials");
        return;
      }

      const data = await response.json();

      console.log("LOGIN DATA:", data);

      if (data.role !== "Admin") {
        alert("Unauthorized");
        return;
      }

      localStorage.setItem(
        "token",
        data.token
      );

      localStorage.setItem(
        "firstName",
        data.firstName
      );

      localStorage.setItem(
        "lastName",
        data.lastName
      );

      localStorage.setItem(
        "role",
        data.role
      );

      localStorage.setItem(
        "counterName",
        data.counterName ?? ""
      );

      window.location.href =
        "/admin/dashboard";
    } catch (error) {
      console.error(
        "Admin login error:",
        error
      );

      alert(
        "Unable to connect to SmartCare server."
      );
    } finally {
      setLoading(false);
    }
  };

  const handleKeyDown = (
    e: React.KeyboardEvent<HTMLInputElement>
  ) => {
    if (e.key === "Enter") {
      login();
    }
  };

  return (
    <div className="agent-login-page">

      {/* DECORATIONS */}
      <div className="login-circle login-circle-one" />
      <div className="login-circle login-circle-two" />

      <div className="login-dots" />

      {/* LOGIN CARD */}
      <div className="agent-login-card">

        {/* LOGO */}
        <div className="login-logo-wrap">
          <img
            src={logo}
            alt="SmartCare Hospital"
            className="login-logo"
          />
        </div>

        {/* BADGE */}
        <div className="login-admin-badge">
          <FiShield />
          SMARTCARE ADMIN
        </div>

        {/* TITLE */}
        <h1 className="login-title">
          Welcome Back
        </h1>

        <p className="login-subtitle">
          Sign in to access the SmartCare
          administration portal.
        </p>

        {/* EMAIL */}
        <div className="login-field">

          <label>
            Email Address
          </label>

          <div className="login-input-wrap">

            <FiMail
              className="login-input-icon"
            />

            <input
              className="login-input"
              type="email"
              placeholder="admin@smartcare.com"
              value={email}
              autoComplete="email"
              onChange={(e) =>
                setEmail(e.target.value)
              }
              onKeyDown={handleKeyDown}
            />

          </div>
        </div>

        {/* PASSWORD */}
        <div className="login-field">

          <div className="login-password-label">
            <label>
              Password
            </label>
          </div>

          <div className="login-input-wrap">

            <FiLock
              className="login-input-icon"
            />

            <input
              className="login-input login-password-input"
              type={
                showPassword
                  ? "text"
                  : "password"
              }
              placeholder="Enter your password"
              value={password}
              autoComplete="current-password"
              onChange={(e) =>
                setPassword(e.target.value)
              }
              onKeyDown={handleKeyDown}
            />

            {/* EYE BUTTON */}
            <button
              type="button"
              className="password-eye-btn"
              onClick={() =>
                setShowPassword(
                  (current) => !current
                )
              }
              aria-label={
                showPassword
                  ? "Hide password"
                  : "Show password"
              }
            >
              {showPassword ? (
                <FiEyeOff />
              ) : (
                <FiEye />
              )}
            </button>

          </div>
        </div>

        {/* LOGIN BUTTON */}
        <button
          className="login-btn"
          onClick={login}
          disabled={loading}
        >
          {loading
            ? "SIGNING IN..."
            : "SIGN IN"}
        </button>

        {/* SECURITY */}
        <div className="login-security">
          <FiShield />

          <span>
            Secure access for authorized
            SmartCare administrators only.
          </span>
        </div>

      </div>

      {/* FOOTER */}
      <div className="login-footer">
        SmartCare Hospital • Queue Management System
      </div>

    </div>
  );
}