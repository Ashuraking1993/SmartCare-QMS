import "./styles/Kiosk.css";

import { useRef, useState } from "react";
import { useNavigate } from "react-router-dom";

import TicketPrint from "../components/TicketPrint";
import logo from "../assets/logo.png";

import {
  FiActivity,
  FiAlertCircle,
  FiArrowLeft,
  FiArrowRight,
  FiCheck,
  FiClock,
  FiHeart,
  FiMoreHorizontal,
  FiPlus,
  FiUsers,
} from "react-icons/fi";

import { MdOutlineBiotech } from "react-icons/md";
import { FaTooth } from "react-icons/fa";

type ServiceItem = {
  id: string;
  name: string;
  description: string;
  icon: React.ReactNode;
  color: string;
  badge?: string;
};

export default function Kiosk() {
  const navigate = useNavigate();

  const [ticketNumber, setTicketNumber] = useState("");
  const [serviceName, setServiceName] = useState("");

  const [showTicketModal, setShowTicketModal] =
    useState(false);

  const [isGenerating, setIsGenerating] =
    useState(false);

  const [error, setError] = useState("");

  const printRef = useRef<HTMLDivElement>(null);

  /*
   * IMPORTANT:
   * Current IDs are preserved from your existing backend
   * so we don't break ticket generation.
   *
   * Later, when we migrate / seed SmartCare Hospital data,
   * we can replace these IDs with the actual hospital service IDs.
   */
 const services: ServiceItem[] = [
  {
    id: "10000000-0000-0000-0000-000000000001",
    name: "General Consultation",
    description:
      "General check-up and common health concerns.",
    icon: <FiActivity />,
    color: "green",
  },

  {
    id: "10000000-0000-0000-0000-000000000002",
    name: "Emergency",
    description:
      "Priority service for urgent medical concerns.",
    icon: <FiAlertCircle />,
    color: "red",
    badge: "Priority",
  },

  {
    id: "10000000-0000-0000-0000-000000000003",
    name: "Specialist",
    description:
      "Consult with our specialized medical doctors.",
    icon: <FiHeart />,
    color: "purple",
  },

  {
    id: "10000000-0000-0000-0000-000000000004",
    name: "Laboratory",
    description:
      "Laboratory tests and diagnostic procedures.",
    icon: <MdOutlineBiotech />,
    color: "orange",
  },

  {
    id: "10000000-0000-0000-0000-000000000005",
    name: "Dental Care",
    description:
      "Dental consultation, check-up and treatment.",
    icon: <FaTooth />,
    color: "blue",
  },

  {
    id: "10000000-0000-0000-0000-000000000006",
    name: "Other Services",
    description:
      "Medical records, billing and other concerns.",
    icon: <FiMoreHorizontal />,
    color: "teal",
  },
];

  const generateTicket = async (
    serviceId: string,
    selectedServiceName: string
  ) => {
    if (isGenerating) return;

    try {
      setError("");
      setIsGenerating(true);

      const response = await fetch(
        "http://localhost:5025/api/Queue/generate",
        {
          method: "POST",

          headers: {
            "Content-Type": "application/json",
          },

          body: JSON.stringify({
            serviceId,
          }),
        }
      );

      if (!response.ok) {
        throw new Error(
          `Ticket generation failed (${response.status})`
        );
      }

      const data = await response.json();

      setTicketNumber(data.ticketNumber);
      setServiceName(selectedServiceName);
      setShowTicketModal(true);

      setTimeout(() => {
        window.print();
      }, 350);

      setTimeout(() => {
        setShowTicketModal(false);
        setTicketNumber("");
      }, 5000);
    } catch (err) {
      console.error(err);

      setError(
        "Unable to generate your queue ticket. Please try again."
      );
    } finally {
      setIsGenerating(false);
    }
  };

  return (
    <div className="smartcare-kiosk">
      {/* ================= BACKGROUND ================= */}

      <div className="kiosk-bg-circle circle-one" />
      <div className="kiosk-bg-circle circle-two" />
      <div className="kiosk-bg-dots" />

      {/* ================= TOP BAR ================= */}

      <header className="kiosk-topbar">
        <button
          className="kiosk-back-btn"
          onClick={() => navigate("/")}
        >
          <FiArrowLeft />
          <span>Back to Home</span>
        </button>

            <button
        type="button"
        className="kiosk-brand"
        onClick={() => navigate("/")}
        aria-label="SmartCare Home"
      >
        <img
          src={logo}
          alt="SmartCare Hospital"
        />
      </button>

              <div className="kiosk-status">
          <span className="online-dot" />

          System Online
        </div>
      </header>

      {/* ================= MAIN ================= */}

      <main className="kiosk-main">
        {/* HEADER */}

        <section className="kiosk-intro">
          <div className="kiosk-ai-label">
            ✦ SMARTCARE QUEUE
          </div>

          <h1>
            How can we help you
            <span> today?</span>
          </h1>

          <p>
            Select the hospital service you need and
            SmartCare will generate your queue number.
          </p>

          <div className="kiosk-info-row">
            <div>
              <FiClock />

              <span>
                <strong>18 mins</strong>
                Average wait
              </span>
            </div>

            <div>
              <FiUsers />

              <span>
                <strong>58</strong>
                Patients waiting
              </span>
            </div>

            <div>
              <FiCheck />

              <span>
                <strong>Live</strong>
                Queue updates
              </span>
            </div>
          </div>
        </section>

        {/* SERVICES */}

        <section className="kiosk-services">
          <div className="services-heading">
            <div>
              <span className="section-eyebrow">
                SELECT A SERVICE
              </span>

              <h2>
                Get Your Queue
              </h2>
            </div>

            <p>
              Tap a service below to continue.
            </p>
          </div>

          {error && (
            <div className="kiosk-error">
              <FiAlertCircle />

              <span>{error}</span>
            </div>
          )}

          <div className="kiosk-service-grid">
            {services.map((service) => (
              <button
                type="button"
                key={service.id}
                className={`kiosk-service-card ${service.color}`}
                disabled={isGenerating}
                onClick={() =>
                  generateTicket(
                    service.id,
                    service.name
                  )
                }
              >
                {service.badge && (
                  <span className="service-badge">
                    {service.badge}
                  </span>
                )}

                <div
                  className={`kiosk-service-icon ${service.color}`}
                >
                  {service.icon}
                </div>

                <div className="kiosk-service-content">
                  <h3>
                    {service.name}
                  </h3>

                  <p>
                    {service.description}
                  </p>

                  <span className="service-action">
                    Get Queue
                    <FiArrowRight />
                  </span>
                </div>
              </button>
            ))}
          </div>
        </section>

        {/* ASSISTANCE */}

        <section className="kiosk-assistance">
          <div className="assistance-icon">
            ?
          </div>

          <div>
            <strong>
              Not sure which service to choose?
            </strong>

            <span>
              Our hospital staff can assist you at
              the information desk.
            </span>
          </div>
        </section>
      </main>

      {/* ================= FOOTER ================= */}

      <footer className="kiosk-footer">
        <span>
          SmartCare Hospital
        </span>

        <span className="footer-divider">
          •
        </span>

        <span>
          AI-Powered Smart Queue System
        </span>

        <span className="footer-divider">
          •
        </span>

        <span>
          Compassionate Care, Smarter Solutions.
        </span>
      </footer>

      {/* ================= TICKET MODAL ================= */}

      {showTicketModal && (
        <div className="ticket-modal-overlay">
          <div className="smart-ticket-modal">
            <div className="ticket-success-icon">
              <FiCheck />
            </div>

            <span className="ticket-success-label">
              QUEUE CREATED SUCCESSFULLY
            </span>

            <h3>
              Your Queue Number
            </h3>

            <div className="ticket-number">
              {ticketNumber}
            </div>

            <div className="ticket-service">
              {serviceName}
            </div>

            <div className="ticket-divider" />

            <div className="ticket-message">
              <FiClock />

              <div>
                <strong>
                  Please wait for your turn.
                </strong>

                <span>
                  Your ticket will be printed
                  automatically.
                </span>
              </div>
            </div>

            <div className="ticket-printing">
              Printing your ticket...
            </div>
          </div>
        </div>
      )}

      {/* ================= PRINT ================= */}

      <div
        ref={printRef}
        className="print-only"
      >
        <TicketPrint
          ticketNumber={ticketNumber}
          serviceName={serviceName}
        />
      </div>
    </div>
  );
}