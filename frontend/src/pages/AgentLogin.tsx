import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  FiEye,
  FiEyeOff,
  FiLock,
  FiMail,
  FiShield,
} from "react-icons/fi";

import "./styles/AgentLogin.css";
import logo from "../assets/logo.png";

export default function AgentLogin() {
  const navigate = useNavigate();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [showPassword, setShowPassword] =
    useState(false);

  const [loading, setLoading] =
    useState(false);

  const login = async () => {
    if (!email.trim() || !password) {
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
        alert("Invalid email or password.");
        return;
      }

      const data = await response.json();

      console.log("AGENT LOGIN DATA:", data);

      /*
        Prevent Admin from entering the Agent portal.
        Adjust this if your backend uses another role name.
      */
      if (data.role === "Admin") {
        alert(
          "Admin accounts must use the Admin Portal."
        );
        return;
      }

      localStorage.setItem(
        "token",
        data.token
      );

      localStorage.setItem(
        "firstName",
        data.firstName ?? ""
      );

      localStorage.setItem(
        "lastName",
        data.lastName ?? ""
      );

      localStorage.setItem(
        "role",
        data.role ?? ""
      );

      localStorage.setItem(
        "counterName",
        data.counterName ?? ""
      );

      navigate("/agent");
    } catch (error) {
      console.error(
        "Agent login error:",
        error
      );

      alert(
        "Unable to connect to the SmartCare server."
      );
    } finally {
      setLoading(false);
    }
  };

  const handleKeyDown = (
    event: React.KeyboardEvent<HTMLInputElement>
  ) => {
    if (event.key === "Enter") {
      login();
    }
  };

  return (
    <div className="agent-login-page">

      {/* Background decorations */}
      <div className="agent-login-circle agent-login-circle-one" />
      <div className="agent-login-circle agent-login-circle-two" />
      <div className="agent-login-dots" />

      {/* Login Card */}
      <div className="agent-login-card">

        {/* Logo */}
        <div className="agent-login-logo-wrap">
          <img
            src={logo}
            alt="SmartCare Hospital"
            className="agent-login-logo"
          />
        </div>

        {/* Portal Badge */}
        <div className="agent-login-badge">
          <FiShield />
          SMARTCARE STAFF
        </div>

        {/* Title */}
        <h1 className="agent-login-title">
          Welcome Back
        </h1>

        <p className="agent-login-subtitle">
          Sign in to access your SmartCare
          queue counter.
        </p>

        {/* Email */}
        <div className="agent-login-field">
          <label htmlFor="agent-email">
            Email Address
          </label>

          <div className="agent-login-input-wrap">
            <FiMail className="agent-login-input-icon" />

            <input
              id="agent-email"
              className="agent-login-input"
              type="email"
              placeholder="agent@smartcare.com"
              value={email}
              autoComplete="email"
              onChange={(e) =>
                setEmail(e.target.value)
              }
              onKeyDown={handleKeyDown}
            />
          </div>
        </div>

        {/* Password */}
        <div className="agent-login-field">
          <label htmlFor="agent-password">
            Password
          </label>

          <div className="agent-login-input-wrap">
            <FiLock className="agent-login-input-icon" />

            <input
              id="agent-password"
              className="agent-login-input agent-password-input"
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

            <button
              type="button"
              className="agent-password-eye"
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
              title={
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

        {/* Login */}
        <button
          type="button"
          className="agent-login-btn"
          onClick={login}
          disabled={loading}
        >
          {loading
            ? "SIGNING IN..."
            : "SIGN IN"}
        </button>

        {/* Security */}
        <div className="agent-login-security">
          <FiShield />

          <span>
            Secure access for authorized
            SmartCare staff only.
          </span>
        </div>
      </div>

      {/* Footer */}
      <div className="agent-login-footer">
        SmartCare Hospital • Queue Management System
      </div>
    </div>
  );
}