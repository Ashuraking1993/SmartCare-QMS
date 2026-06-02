import "./styles/Kiosk.css";
import logo from "../assets/logo.png";
import { useState, useRef } from "react";
import TicketPrint from "../components/TicketPrint";


import {
  FiMessageCircle,
  FiUserPlus,
  FiDownload
} from "react-icons/fi";

import { MdPayment } from "react-icons/md";
import { FaCrown } from "react-icons/fa";
import { BsPersonHeart } from "react-icons/bs";

export default function Kiosk() {

 const [ticketNumber, setTicketNumber] = useState("");
 const [showTicketModal, setShowTicketModal] = useState(false);
 const [serviceName, setServiceName] = useState("");
 const printRef = useRef<HTMLDivElement>(null);
 const generateTicket = async (
  serviceId: string,
  serviceName: string
) => {
  try {
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

    const data = await response.json();

    setTicketNumber(data.ticketNumber);
    setServiceName(serviceName);
    setShowTicketModal(true);


    setTimeout(() => {
      setShowTicketModal(false);
      setTicketNumber("");
    }, 5000);

    setTimeout(() => {
    window.print();
    }, 300);

  } catch (error) {
    console.error(error);
  }
};


  return (
    <div className="kiosk-page">

      
        {
        showTicketModal && (
            <div className="ticket-modal">

            <div className="ticket-card">

                <h3>Your Queue Number</h3>

                <div className="ticket-number">
                {ticketNumber}
                </div>

                <p>
                Please wait for your turn.
                </p>

            </div>

            </div>
        )
        }

      <div className="background-glow glow-left"></div>
      <div className="background-glow glow-right"></div>

      <div className="kiosk-header">

        <div className="logo-container">
          <img
            src={logo}
            alt="Banko De Filipino"
            className="bank-logo"
          />
        </div>

        <div className="header-content">

          <div className="baybayin-banner">
            ᜋᜄᜈ᜔ᜇᜅ᜔ ᜀᜇᜏ᜔
          </div>

          <h1 className="bank-title">
            BANKO DE FILIPINO
          </h1>

          <p className="bank-subtitle">
            Modern Queue Management System By Digital Ronin
          </p>

        </div>

      </div>

      <div className="services-grid">

        {/* GENERAL INQUIRY */}
        <div
            className="service-card"
            onClick={() =>
                generateTicket(
            "8B746602-0209-4E42-AF06-6B7870CB69BD",
            "GENERAL INQUIRY"
            )
            }
            >

          <div className="service-icon">
            <FiMessageCircle />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            GENERAL INQUIRY
          </div>

        </div>

        {/* BILLS PAYMENT */}
        <div
            className="service-card"
            onClick={() =>
                generateTicket(
                "CA6AD9BE-54EB-4706-AAC6-93256C5610CF",
                "BILLS PAYMENT"
                )
            }
            >

          <div className="service-icon">
            <MdPayment />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            BILLS PAYMENT
          </div>

        </div>

        {/* OPEN ACCOUNT */}
         <div
            className="service-card"
            onClick={() =>
                generateTicket(
                "8740E138-B984-49B5-BE28-CEC86996E594",
                "OPEN ACCOUNT"
                )
            }
            >

          <div className="service-icon">
            <FiUserPlus />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            OPEN ACCOUNT
          </div>

        </div>

        {/* VIP */}
        <div
            className="service-card"
            onClick={() =>
                generateTicket(
                "EE9C8F2C-85D1-40D2-9E57-CF17A1CB28AE",
                "VIP EXPRESS"
)
            }
            >

          <div className="service-icon">
            <FaCrown />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            VIP EXPRESS
          </div>

        </div>

        {/* PWD */}
        <div
            className="service-card"
            onClick={() =>
                generateTicket(
                "50BBFB12-49A0-441A-A06D-13DC5890631C","PWD/SENIOR"
                )
            }
            >

          <div className="service-icon">
            <BsPersonHeart />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            PWD / SENIOR
          </div>

        </div>

        {/* DEPOSIT */}
        <div
            className="service-card"
            onClick={() =>
                generateTicket(
                "35827DD6-25B5-4D5B-BCE2-F77785A4B80F","DEPOSIT"
                )
            }
            >

          <div className="service-icon">
            <FiDownload />
          </div>

          <div className="service-divider"></div>

          <div className="service-title">
            DEPOSIT / ENCASHMENT
          </div>

        </div>

      </div>
      
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