import { useNavigate } from "react-router-dom";
import "../styles/LandingPage.css";

import logo from "../assets/logo.png";
import bgImage from "../assets/bg.png";
import androidImage from "../assets/android.png";

function LandingPage() {
  const navigate = useNavigate();

  const goToKiosk = () => {
    navigate("/kiosk");
  };

  const scrollTo = (id: string) => {
    document.getElementById(id)?.scrollIntoView({
      behavior: "smooth",
    });
  };

  return (
    <div className="smartcare-page">

      {/* =====================================================
          NAVBAR
      ===================================================== */}
      <header className="smartcare-navbar">

        <button
          className="brand"
          type="button"
          onClick={() => navigate("/")}
        >
          <img src={logo} alt="SmartCare Hospital" />
        </button>

        <nav className="main-nav">
          <button
            className="active"
            onClick={() => navigate("/")}
          >
            Home
          </button>

          <button onClick={() => scrollTo("departments")}>
            Departments
          </button>

          <button onClick={() => scrollTo("doctors")}>
            Find Doctor
          </button>

          <button onClick={() => scrollTo("about")}>
            About Us
          </button>

          <button onClick={() => scrollTo("contact")}>
            Contact
          </button>
        </nav>

        <button
          className="queue-display-btn"
          onClick={() => navigate("/display")}
        >
          <span>▣</span>
          Queue Display
        </button>

      </header>


      {/* =====================================================
          HERO
      ===================================================== */}
      <section
        className="hero"
        style={{
          backgroundImage: `url(${bgImage})`,
        }}
      >

        <div className="hero-overlay" />

        <div className="hero-content">

          <div className="hero-badge">
            <span>✦</span>
            AI-POWERED HEALTHCARE
          </div>

          <h1>
            Compassionate Care,
            <strong>Smarter Solutions.</strong>
          </h1>

          <p className="hero-subtitle">
            Experience a better way to manage your hospital visit
            with our AI-powered smart queue system.
          </p>

          <div className="hero-features">

            <Feature
              icon="♟"
              title="Smart Queue"
              description="Real-time updates and tracking"
            />

            <Feature
              icon="AI"
              title="AI Insights"
              description="Estimated waiting time & predictions"
            />

            <Feature
              icon="✓"
              title="Better Experience"
              description="Less waiting, more care"
            />

            <Feature
              icon="♡"
              title="Patient First"
              description="Prioritized services for your needs"
            />

          </div>

          <div className="ai-notice">

            <div className="ai-robot">
              🤖
            </div>

            <div className="ai-notice-text">
              <strong>
                Our AI system helps prioritize patients
              </strong>

              <span>
                Based on urgency and waiting time to serve
                you better.
              </span>
            </div>

            <button
              onClick={() => scrollTo("about")}
              className="learn-more-link"
            >
              Learn more →
            </button>

          </div>

          {/* AVAILABLE ON */}
<div className="hero-platform">

  <div className="hero-platform-title">
    <span>AVAILABLE ON</span>
    <strong>Web and Mobile</strong>
    <small>Access SmartCare anytime, anywhere.</small>
  </div>

  <div className="hero-platform-card">
    <div className="hero-platform-item">
      <div className="hero-platform-icon">◎</div>

      <div>
        <strong>Web Version</strong>
        <span>
          Full features for staff
          <br />
          and administrators
        </span>
      </div>
    </div>

    <div className="hero-platform-divider" />

    <div className="hero-platform-item">
      <div className="hero-platform-icon">▯</div>

      <div>
        <strong>Mobile App</strong>
        <span>
          Stay updated on your
          <br />
          queue and notifications
        </span>
      </div>
    </div>
  </div>

  <div className="hero-store-badges">
    <img
      src={androidImage}
      alt="Download SmartCare on Google Play and App Store"
    />
  </div>

</div>

        </div>

      </section>

      


      {/* =====================================================
          QUEUE CTA
      ===================================================== */}
      <section
        className="queue-section"
        id="departments"
      >

        <div className="queue-cta">

        

          <span className="section-label">
            SMARTCARE QUEUE SYSTEM
          </span>

          <h2>
            Your Visit Starts
            <span> Smarter.</span>
          </h2>

          <p className="queue-description">
            Avoid unnecessary waiting. Select the service you
            need and receive your queue number instantly through
            the SmartCare queue system.
          </p>

          <button
            className="queue-kiosk-button"
            onClick={goToKiosk}
          >

            <span className="queue-button-icon">
              ▣
            </span>

            <span className="queue-button-content">
              <strong>Get Your Queue</strong>
              <small>Select a hospital service</small>
            </span>

            <span className="queue-arrow">
              →
            </span>

          </button>

          <div className="queue-benefits">

            <div>
              <span>✓</span>
              No registration required
            </div>

            <div>
              <span>◷</span>
              Real-time queue
            </div>

            <div>
              <span>✦</span>
              AI-assisted waiting time
            </div>

          </div>

        </div>

      </section>


    

      {/* =====================================================
          QUICK FEATURES
      ===================================================== */}
      <section className="quick-features">

        <QuickFeature
          icon="◷"
          title="Save Time"
          text="Real-time queue updates"
        />

        <QuickFeature
          icon="⌖"
          title="Plan Better"
          text="Estimated waiting time"
        />

        <QuickFeature
          icon="♢"
          title="Stay Informed"
          text="Instant notifications"
        />

        <QuickFeature
          icon="♡"
          title="Smarter Care"
          text="A better patient experience"
        />

      </section>


      {/* =====================================================
          LIVE QUEUE STATS
      ===================================================== */}
      <section className="stats-section">

        <div className="queue-overview stat-panel">

          <div className="panel-title">

            <div>
              <span className="panel-eyebrow">
                LIVE
              </span>

              <strong>
                Queue Overview
              </strong>
            </div>

            <button
              onClick={() => navigate("/display")}
            >
              View All →
            </button>

          </div>

          <QueueRow
            name="General Consultation"
            number="23 waiting"
            color="green"
          />

          <QueueRow
            name="Emergency"
            number="5 waiting"
            color="red"
          />

          <QueueRow
            name="Specialist"
            number="18 waiting"
            color="purple"
          />

          <QueueRow
            name="Laboratory"
            number="12 waiting"
            color="orange"
          />

        </div>


        <div className="waiting-card stat-panel">

          <div className="stat-icon">
            ◷
          </div>

          <span className="stat-label">
            Average Waiting Time
          </span>

          <strong>
            18
            <small>mins</small>
          </strong>

          <span className="updated-text">
            Updated just now
          </span>

        </div>


        <div className="insight-card stat-panel">

          <div className="stat-icon blue">
            ✦
          </div>

          <span className="card-label">
            AI INSIGHTS
          </span>

          <strong>
            Smarter Queue Prediction
          </strong>

          <p>
            High patient volume expected between
            <b> 2:00 PM - 4:00 PM.</b>
          </p>

          <button>
            More Insights →
          </button>

        </div>


        <div className="help-card stat-panel">

          <div className="stat-icon teal">
            ♡
          </div>

          <span className="card-label">
            PATIENT SUPPORT
          </span>

          <strong>
            Need Help?
          </strong>

          <p>
            Our SmartCare staff are ready to assist you.
          </p>

          <button
            onClick={() => scrollTo("contact")}
          >
            Contact Support
          </button>

        </div>

      </section>


      {/* =====================================================
          ABOUT
      ===================================================== */}
      <section
        className="about-section"
        id="about"
      >

        <div className="about-content">

          <span className="section-label">
            ABOUT SMARTCARE
          </span>

          <h2>
            Healthcare made
            <span> smarter.</span>
          </h2>

          <p>
            SmartCare Hospital combines compassionate healthcare
            with intelligent technology. Our smart queue platform
            helps reduce waiting times, improve patient flow and
            create a more convenient hospital experience.
          </p>

          <div className="about-points">

            <div>
              <span>✓</span>
              Intelligent patient flow
            </div>

            <div>
              <span>✓</span>
              Real-time queue visibility
            </div>

            <div>
              <span>✓</span>
              Web and mobile access
            </div>

            <div>
              <span>✓</span>
              AI-assisted predictions
            </div>

          </div>

        </div>


        <div className="about-ai-card">

          <div className="about-logo">
            <img
              src={logo}
              alt="SmartCare"
            />
          </div>

          <div>

            <span>
              SMARTCARE TECHNOLOGY
            </span>

            <strong>
              Care Beyond Waiting.
            </strong>

            <p>
              Connecting patients and hospital services
              through smarter digital healthcare.
            </p>

          </div>

        </div>

      </section>


      {/* =====================================================
          DOCTORS
      ===================================================== */}
      <section
        className="doctors-section"
        id="doctors"
      >

        <div className="section-heading">

          <span className="section-label">
            OUR MEDICAL TEAM
          </span>

          <h2>
            Find the right
            <span> doctor.</span>
          </h2>

          <p>
            Connect with the right specialist and plan your
            hospital visit before you arrive.
          </p>

        </div>


        <div className="doctor-placeholder">

          <div className="doctor-icon">
            ♡
          </div>

          <strong>
            SmartCare Doctor Directory
          </strong>

          <span>
            Coming soon
          </span>

        </div>

      </section>


      {/* =====================================================
          FOOTER
      ===================================================== */}
      <footer
        className="footer"
        id="contact"
      >

        <div className="footer-main">

          <div className="footer-brand">

            <img
              src={logo}
              alt="SmartCare Hospital"
            />

            <p>
              Compassionate Care,
              <br />
              Smarter Solutions.
            </p>

          </div>


          <div className="footer-contact">

            <span>
              <b>☎</b>
              (02) 1234 5678
            </span>

            <span>
              <b>✉</b>
              info@smartcarehospital.com
            </span>

            <span>
              <b>⌖</b>
              123 Health St., Wellness City, PH
            </span>

          </div>

        </div>


        <div className="footer-bottom">
          © 2026 SmartCare Hospital. All rights reserved.
        </div>

      </footer>

    </div>
  );
}


/* =========================================================
   SMALL COMPONENTS
========================================================= */

function Feature({
  icon,
  title,
  description,
}: {
  icon: string;
  title: string;
  description: string;
}) {
  return (
    <div className="hero-feature">

      <div className="feature-icon">
        {icon}
      </div>

      <div>
        <strong>{title}</strong>
        <span>{description}</span>
      </div>

    </div>
  );
}


function PlatformFeature({
  icon,
  title,
  text,
}: {
  icon: string;
  title: string;
  text: string;
}) {
  return (
    <div className="platform-feature">

      <div className="platform-feature-icon">
        {icon}
      </div>

      <div>
        <strong>{title}</strong>
        <span>{text}</span>
      </div>

    </div>
  );
}


function QuickFeature({
  icon,
  title,
  text,
}: {
  icon: string;
  title: string;
  text: string;
}) {
  return (
    <div className="quick-feature">

      <div className="quick-icon">
        {icon}
      </div>

      <div>
        <strong>{title}</strong>
        <span>{text}</span>
      </div>

    </div>
  );
}


function QueueRow({
  name,
  number,
  color,
}: {
  name: string;
  number: string;
  color: string;
}) {
  return (
    <div className="queue-row">

      <div className="queue-name">

        <span
          className={`queue-dot ${color}`}
        />

        {name}

      </div>

      <span
        className={`queue-number ${color}`}
      >
        {number}
      </span>

    </div>
  );
}


export default LandingPage;