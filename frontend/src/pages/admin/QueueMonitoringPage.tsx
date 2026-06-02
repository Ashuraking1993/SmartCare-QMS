import { useEffect, useState } from "react";
import {
  getWaitingTickets,
  getServingTickets,
  getCompletedTickets,
} from "../../services/adminService";
import logo from "../../assets/logo.png";
import "./QueueMonitoringPage.css";

export default function QueueMonitoringPage() {
  const [waitingTickets, setWaitingTickets] = useState<any[]>([]);
  const [servingTickets, setServingTickets] = useState<any[]>([]);
  const [completedTickets, setCompletedTickets] = useState<any[]>([]);

  const load = async () => {
    const [w, s, c] = await Promise.all([
      getWaitingTickets(),
      getServingTickets(),
      getCompletedTickets(),
    ]);
    setWaitingTickets(w);
    setServingTickets(s);
    setCompletedTickets(c);
  };

  useEffect(() => {
    load();
    const interval = setInterval(load, 3000);
    return () => clearInterval(interval);
  }, []);

  const getPrefix = (ticketNumber: string) =>
    ticketNumber?.charAt(0).toUpperCase() ?? "?";

  return (
    <div className="qm-page">

      {/* Watermark */}
      <img src={logo} alt="" className="qm-watermark" />

      {/* Header */}
      <div className="qm-header">
        <h1 className="qm-title">Queue Monitoring</h1>
        <div className="qm-live-badge">
          <span className="qm-live-dot" />
          <span className="qm-live-label">LIVE</span>
        </div>
      </div>

      {/* 3-Column Grid */}
      <div className="qm-grid">

        {/* Waiting */}
        <div className="qm-col">
          <div className="qm-col-head">
            <div className="qm-col-top">
              <span className="qm-col-label qm-label-wait">Waiting</span>
              <span className="qm-col-count qm-count-wait">{waitingTickets.length}</span>
            </div>
            <div className="qm-accent qm-accent-wait" />
          </div>
          <div className="qm-col-body">
            {waitingTickets.length === 0 && (
              <div className="qm-empty">No tickets waiting</div>
            )}
            {waitingTickets.map((ticket) => (
              <div key={ticket.id} className="qm-ticket-item">
                <div className={`qm-prefix qm-prefix-${getPrefix(ticket.ticketNumber)}`}>
                  {getPrefix(ticket.ticketNumber)}
                </div>
                <span className="qm-ticket-num">{ticket.ticketNumber}</span>
              </div>
            ))}
          </div>
        </div>

        {/* Serving */}
        <div className="qm-col">
          <div className="qm-col-head">
            <div className="qm-col-top">
              <span className="qm-col-label qm-label-serve">Serving</span>
              <span className="qm-col-count qm-count-serve">{servingTickets.length}</span>
            </div>
            <div className="qm-accent qm-accent-serve" />
          </div>
          <div className="qm-col-body">
            {servingTickets.length === 0 && (
              <div className="qm-empty">No active serving</div>
            )}
            {servingTickets.map((ticket) => (
              <div key={ticket.id} className="qm-serve-item">
                <div className={`qm-prefix qm-prefix-${getPrefix(ticket.ticketNumber)}`}>
                  {getPrefix(ticket.ticketNumber)}
                </div>
                <span className="qm-ticket-num">{ticket.ticketNumber}</span>
                {ticket.counterName && (
                  <span className="qm-counter-name">{ticket.counterName}</span>
                )}
              </div>
            ))}
          </div>
        </div>

        {/* Completed */}
        <div className="qm-col">
          <div className="qm-col-head">
            <div className="qm-col-top">
              <span className="qm-col-label qm-label-done">Completed</span>
              <span className="qm-col-count qm-count-done">{completedTickets.length}</span>
            </div>
            <div className="qm-accent qm-accent-done" />
          </div>
          <div className="qm-col-body">
            {completedTickets.length === 0 && (
              <div className="qm-empty">No completed tickets</div>
            )}
            {completedTickets.map((ticket) => (
              <div key={ticket.id} className="qm-done-item">
                <div className={`qm-prefix qm-prefix-${getPrefix(ticket.ticketNumber)} qm-prefix-dim`}>
                  {getPrefix(ticket.ticketNumber)}
                </div>
                <span className="qm-done-num">{ticket.ticketNumber}</span>
                <div className="qm-check">✓</div>
              </div>
            ))}
          </div>
        </div>

      </div>

      {/* Footer */}
      <div className="qm-footer">
        <span className="qm-credit">Forged by Digital Ronin</span>
        <span className="qm-tagline">TRUST · SERVICE · PROGRESS</span>
      </div>

    </div>
  );
}
