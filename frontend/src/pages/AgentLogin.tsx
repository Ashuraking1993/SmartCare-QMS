import { useState } from "react";
import "./styles/AgentLogin.css";
import logo from "../assets/logo.png";
export default function AgentLogin() {

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const login = async () => {

    const response = await fetch(
      "http://localhost:5025/api/Auth/login",
      {
        method: "POST",
        headers: {
          "Content-Type": "application/json"
        },
        body: JSON.stringify({
          email,
          password
        })
      }
    );

    if (!response.ok) {
      alert("Invalid credentials");
      return;
    }

    const data = await response.json();

    console.log("LOGIN DATA:", data);

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
    data.counterName
    );


    window.location.href = "/agent";
  };

  return (
  <div className="agent-login-page">

    <div className="agent-login-card">

        <img
        src={logo}
        alt="Bank Logo"
        className="login-logo"
        />

      <h1 className="login-title">
        BANKO DE FILIPINO
      </h1>

      <p className="login-subtitle">
        Agent Log In
      </p>

      <input
        className="login-input"
        placeholder="Email"
        value={email}
        onChange={(e) =>
          setEmail(e.target.value)
        }
      />

      <input
        className="login-input"
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) =>
          setPassword(e.target.value)
        }
      />

      <button
        className="login-btn"
        onClick={login}
      >
        LOGIN
      </button>

    </div>

  </div>
  );
}