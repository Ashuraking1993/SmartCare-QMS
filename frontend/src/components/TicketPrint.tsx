import "./TicketPrint.css";
import logo from "../assets/logo.png";

type TicketPrintProps = {
  ticketNumber: string;
  serviceName: string;
};

function TicketPrint({
  ticketNumber,
  serviceName,
}: TicketPrintProps) {
  return (
    <div className="print-ticket">
      <div className="ticket-header">
        <img
            src={logo}
            alt="Bank Logo"
            className="ticket-logo"
            />

        <div className="ticket-bank-name">
          BANKO DE
          <br />
          FILIPINO
        </div>
      </div>

      <div className="ticket-number">
        {ticketNumber}
      </div>

      <div className="ticket-service">
        {serviceName}
      </div>

      <div className="ticket-waiting">
        WAITING TIME APPROX 20 MINUTES
      </div>
    </div>
  );
}

export default TicketPrint;