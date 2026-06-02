import "./styles/Display.css";
import logo from "../assets/logo.png";
import { useState, useEffect } from "react";

export default function Display() {

  const [currentTime, setCurrentTime] = useState("");

  const [serving, setServing] = useState<any[]>([]);
  const [upNext, setUpNext] = useState<string[]>([]);
  const [waitingCount, setWaitingCount] = useState(0);
  const [messageIndex, setMessageIndex] = useState(0);
  const [fadeIn, setFadeIn] = useState(true);

  const messages = [
    "HELLO WELCOME",
    "MAGANDANG ARAW",
    "ᜋᜄᜈ᜔ᜇᜅ᜔ ᜀᜇᜏ᜔",
    "TRUST • SERVICE • PROGRESS",
  ];

  // Clock
  useEffect(() => {
    const timer = setInterval(() => {
      setCurrentTime(
        new Date().toLocaleTimeString("en-US", {
          hour: "2-digit",
          minute: "2-digit",
          second: "2-digit",
        })
      );
    }, 1000);
    return () => clearInterval(timer);
  }, []);

  // Fetch display data
  useEffect(() => {
    const loadDisplay = async () => {
      try {
        const response = await fetch(
          "http://localhost:5025/api/Queue/display"
        );
        const data = await response.json();
        setServing(data.serving);
        setUpNext(data.upNext);
        setWaitingCount(data.waitingCount);
      } catch (error) {
        console.error(error);
      }
    };
    loadDisplay();
    const interval = setInterval(loadDisplay, 3000);
    return () => clearInterval(interval);
  }, []);

  // Rotating message with fade
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

      {/* Watermark */}
      <img src={logo} className="display-bg-logo" alt="" />

      {/* Top Header Bar */}
      <div className="display-header">
        <img src={logo} className="display-logo" alt="" />
        <h1 className="display-brand">BANKO DE FILIPINO</h1>
        <div className="display-clock">{currentTime}</div>
      </div>

      {/* Divider */}
      <div className="display-divider" />

      {/* Rotating Message Banner */}
      <div className="display-message-banner">
        <span className={`display-message-text ${fadeIn ? "fade-in" : "fade-out"}`}>
          {messages[messageIndex]}
        </span>
      </div>

      {/* Main Body */}
      <div className="display-body">

        {/* Counter Cards */}
        <div className="counter-grid">
          {serving.map((item, index) => (
            <div key={index} className="counter-card">
              <p className="counter-label">COUNTER {index + 1}</p>
              <div className="ticket-number">{item.ticketNumber}</div>
            </div>
          ))}
        </div>

        {/* Up Next Panel */}
        <div className="upnext-panel">
          <p className="upnext-label">UP NEXT</p>
          <div className="upnext-list">
            {upNext.map((ticket, index) => (
              <div key={index} className="upnext-item">
                {ticket}
              </div>
            ))}
          </div>
        </div>

      </div>

      {/* Footer */}
      <div className="display-footer">
        <span>Waiting: <strong>{waitingCount}</strong></span>
        <span className="display-credit">Forged by Digital Ronin</span>
      </div>

    </div>
  );
}
