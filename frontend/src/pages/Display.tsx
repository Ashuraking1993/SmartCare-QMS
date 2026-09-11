import "./styles/Display.css";
import logo from "../assets/logo.png";
import { useState, useEffect } from "react";


type UpNextTicket = {
  ticketNumber: string;
  checkedIn: boolean;
  checkedInAt: string | null;
  queueState: "READY" | "CHECKED IN" | "NOT ARRIVED";
};
export default function Display() {
  const [currentTime, setCurrentTime] = useState("");

  const [serving, setServing] = useState<any[]>([]);
  const [upNext, setUpNext] = useState<UpNextTicket[]>([]);
  const [waitingCount, setWaitingCount] = useState(0);
  const [messageIndex, setMessageIndex] = useState(0);
  const [fadeIn, setFadeIn] = useState(true);

  const messages = [
    "WELCOME TO SMARTCARE HOSPITAL",
    "YOUR HEALTH, OUR PRIORITY",
    "SMARTER QUEUES • BETTER CARE",
    "PLEASE WAIT FOR YOUR NUMBER",
  ];

  // ================= CLOCK =================
  useEffect(() => {
    const updateClock = () => {
      setCurrentTime(
        new Date().toLocaleTimeString("en-US", {
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        })
      );
    };

    updateClock();

    const timer = setInterval(updateClock, 1000);

    return () => clearInterval(timer);
  }, []);

  // ================= FETCH DISPLAY DATA =================
  useEffect(() => {
    const loadDisplay = async () => {
      try {
        const response = await fetch(
          "http://localhost:5025/api/Queue/display"
        );

        if (!response.ok) {
          throw new Error(`Display request failed (${response.status})`);
        }

        const data = await response.json();

        setServing(data.serving ?? []);
        setUpNext(data.upNext ?? []);
        setWaitingCount(data.waitingCount ?? 0);
      } catch (error) {
        console.error("Failed to load queue display:", error);
      }
    };

    loadDisplay();

    const interval = setInterval(loadDisplay, 3000);

    return () => clearInterval(interval);
  }, []);

  // ================= ROTATING MESSAGE =================
  useEffect(() => {
    const interval = setInterval(() => {
      setFadeIn(false);

      setTimeout(() => {
        setMessageIndex((prev) => (prev + 1) % messages.length);
        setFadeIn(true);
      }, 500);
    }, 3200);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="display-page">

      {/* ================= WATERMARK ================= */}
      <img
        src={logo}
        className="display-bg-logo"
        alt=""
      />

      {/* ================= HEADER ================= */}
      <div className="display-header">

        <div className="display-brand-wrap">
          <img
            src={logo}
            className="display-logo"
            alt="SmartCare Hospital"
          />

          <div>
            <h1 className="display-brand">
              SMART<span>CARE</span>
            </h1>

            <small className="display-hospital">
              HOSPITAL
            </small>
          </div>
        </div>

        <div className="display-clock">
          {currentTime}
        </div>

      </div>

      <div className="display-divider" />

      {/* ================= MESSAGE ================= */}
      <div className="display-message-banner">
        <span
          className={`display-message-text ${
            fadeIn ? "fade-in" : "fade-out"
          }`}
        >
          {messages[messageIndex]}
        </span>
      </div>

      {/* ================= MAIN ================= */}
      <div className="display-body">

        {/* NOW SERVING */}
        <div className="counter-section">

          <div className="display-section-heading">
            <span className="section-dot" />

            <div>
              <small>LIVE QUEUE</small>
              <h2>Now Serving</h2>
            </div>
          </div>

          <div className="counter-grid">

            {serving.length > 0 ? (
              serving.map((item, index) => (
                <div
                  key={index}
                  className="counter-card"
                >
                 <p className="counter-label">
                {item.departmentName?.toUpperCase() ?? "HOSPITAL DEPARTMENT"}
              </p>

                  <div className="ticket-number">
                    {item.ticketNumber}
                  </div>

                  <span className="serving-status">
                    ● NOW SERVING
                  </span>
                </div>
              ))
            ) : (
              <div className="counter-card empty-counter">
                <p className="counter-label">
                  NOW SERVING
                </p>

                <div className="ticket-number">
                  ---
                </div>

                <span className="serving-status">
                  Waiting for next patient
                </span>
              </div>
            )}

          </div>
        </div>

        {/* ================= UP NEXT ================= */}
        <div className="upnext-panel">

          <div className="upnext-header">
            <div>
              <small>NEXT PATIENTS</small>
              <p className="upnext-label">
                UP NEXT
              </p>
            </div>

            <span className="live-indicator">
              ● LIVE
            </span>
          </div>

          <div className="upnext-list">
          {upNext.length > 0 ? (
            upNext.map((ticket, index) => (
              <div
                key={ticket.ticketNumber}
                className="upnext-item"
              >
                <span className="upnext-position">
                  {index + 1}
                </span>

                <strong>
                  {ticket.ticketNumber}
                </strong>

                <span
                  className={`queue-indicator ${
                    ticket.queueState === "READY"
                      ? "queue-ready"
                      : ticket.queueState === "CHECKED IN"
                        ? "queue-checked"
                        : "queue-not-arrived"
                  }`}
                >
                  {ticket.queueState === "READY"
                    ? "● READY"
                    : ticket.queueState === "CHECKED IN"
                      ? "✓ CHECKED IN"
                      : "○ NOT ARRIVED"}
                </span>
              </div>
            ))
          ) : (
            <div className="upnext-item empty-next">
              No patients waiting
            </div>
          )}
        </div>

        </div>

      </div>

      {/* ================= FOOTER ================= */}
      <div className="display-footer">

        <div className="footer-waiting">
          <span className="waiting-dot" />

          <span>
            Patients Waiting:
            <strong> {waitingCount}</strong>
          </span>
        </div>

        <span className="display-credit">
          SmartCare Hospital • Digital Ronin
        </span>

      </div>

    </div>
  );
}