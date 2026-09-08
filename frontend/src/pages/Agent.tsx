import { useEffect, useState } from "react";
import "./styles/Agent.css";
import logo from "../assets/logo.png";
import { useNavigate } from "react-router-dom";

export default function Agent() {
  const [nowServing, setNowServing] = useState("---");
  const [waitingCount, setWaitingCount] = useState(0);
  const [completedCount, setCompletedCount] = useState(0);
  const navigate = useNavigate();
  const [toast, setToast] = useState<{
        message: string;
        type: "error" | "success" | "warning";
      } | null>(null);


  const token = localStorage.getItem("token");
  if (!token) {
    window.location.href = "/agent-login";
  }

  const loadDashboard = async () => {
    try {
      const token = localStorage.getItem("token");
      const response = await fetch(
        "http://localhost:5025/api/Queue/dashboard",
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      const data = await response.json();
      setNowServing(data.nowServing ?? "---");
      setWaitingCount(data.waitingCount);
      setCompletedCount(data.completedCount);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
  const token = localStorage.getItem("token");

  if (!token) {
    navigate("/agent-login");
  }
}, []);

  useEffect(() => {
    loadDashboard();
    const interval = setInterval(loadDashboard, 2000);
    return () => clearInterval(interval);
  }, []);

const callNext = async () => {
  try {
    const token = localStorage.getItem("token");

    const response = await fetch(
      "http://localhost:5025/api/Queue/next",
      {
        method: "POST",
        headers: {
          Authorization: `Bearer ${token}`,
        },
      }
    );

    // May current patient pa
    if (response.status === 409) {
      const data = await response.json();

      showToast(
        data.message ||
          "Please complete the current ticket before calling the next patient.",
        "warning"
      );

      return;
    }

    // Walang waiting
    if (response.status === 404) {
      showToast(
        "No waiting patients in the queue.",
        "warning"
      );

      return;
    }

    if (!response.ok) {
      showToast(
        "Unable to call the next patient. Please try again.",
        "error"
      );

      return;
    }

    const data = await response.json();

    setNowServing(data.ticketNumber);

    showToast(
      `Ticket ${data.ticketNumber} is now being served.`,
      "success"
    );

    loadDashboard();

  } catch (error) {
    console.error(error);

    showToast(
      "Unable to connect to the server.",
      "error"
    );
  }
};

  const completeTicket = async () => {
    try {
      if (!nowServing || nowServing === "---") return;
      const token = localStorage.getItem("token");
      const response = await fetch(
        `http://localhost:5025/api/Queue/complete/${nowServing}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );
      if (!response.ok) return;
      setNowServing("---");
      loadDashboard();
    } catch (error) {
      console.error(error);
    }
  };

  const firstName = localStorage.getItem("firstName");
  const lastName = localStorage.getItem("lastName");
  const userRole = localStorage.getItem("role");
  const counterName = localStorage.getItem("counterName");

  const logout = () => {
    localStorage.clear();
    window.location.href = "/agent-login";
  };

  const showToast = (
  message: string,
  type: "error" | "success" | "warning" = "error"
) => {
  setToast({ message, type });

  setTimeout(() => {
    setToast(null);
  }, 3500);
};

  return (
    <div className="agent-page">
        {toast && (
  <div className={`agent-toast ${toast.type}`}>
    <div className="agent-toast-icon">
      {toast.type === "success"
        ? "✓"
        : toast.type === "warning"
        ? "!"
        : "×"}
    </div>

    <div className="agent-toast-content">
      <span className="agent-toast-title">
        {toast.type === "success"
          ? "Success"
          : toast.type === "warning"
          ? "Action Required"
          : "Error"}
      </span>

      <p>{toast.message}</p>
    </div>

    <button
      className="agent-toast-close"
      onClick={() => setToast(null)}
    >
      ×
    </button>
  </div>
)}

      {/* Watermark */}
      <img src={logo} alt="" className="agent-bg-logo" />

      {/* ── Sidebar ── */}
      <div className="agent-sidebar">
        <div className="agent-user-card">
          <p className="agent-user-label">CURRENT USER</p>
          <h2 className="agent-user-name">{firstName} {lastName}</h2>
          <p className="agent-user-role">{userRole}</p>
        </div>

        <div className="agent-sidebar-spacer" />

        <button className="logout-btn" onClick={logout}>
          LOGOUT
        </button>
      </div>

      {/* ── Main Panel ── */}
      <div className="agent-main">

        {/* Top header */}
        <div className="agent-top-header">
          <img src={logo} alt="Logo" className="agent-logo" />
          <div className="agent-header-brand">
            <h1>SMART CARE</h1>
            <span>{counterName}</span>
          </div>
        </div>

        {/* Content */}
        <div className="agent-content">

          {/* Now Serving */}
          <div className="serving-card">
            <div className="serving-label">NOW SERVING</div>
            <div className="serving-number">{nowServing}</div>
          </div>

          {/* Stats */}
          <div className="agentstats-grid">
            <div className="stat-card">
              <h3>WAITING</h3>
              <span>{waitingCount}</span>
            </div>
            <div className="stat-card">
              <h3>COMPLETED</h3>
              <span>{completedCount}</span>
            </div>
          </div>

          {/* Actions */}
          <div className="agent-actions">
            <button className="call-btn" onClick={callNext}>
              CALL NEXT
            </button>
            <button className="complete-btn" onClick={completeTicket}>
              COMPLETE
            </button>
          </div>

        </div>
      </div>

    </div>
    
  );
}
