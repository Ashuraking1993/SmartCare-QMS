import { BrowserRouter, Routes, Route } from "react-router-dom";
import Kiosk from "./pages/Kiosk";
import Display from "./pages/Display";
import Agent from "./pages/Agent";
import AgentLogin from "./pages/AgentLogin";
import AdminDashboard from "./pages/admin/AdminDashboard";
import AdminLogin from "./pages/AdminLogin";
import QueueMonitoringPage from "./pages/admin/QueueMonitoringPage";
import UserManagementPage from "./pages/admin/UserManagementPage";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/kiosk" element={<Kiosk />} />
        <Route path="/display" element={<Display />} />
        <Route path="/agent" element={<Agent />} />
        <Route path="/agent-login"element={<AgentLogin />}/>
        <Route path="/admin/dashboard"element={<AdminDashboard />}/>
        <Route path="/admin-login" element={<AdminLogin />} />
        <Route path="/admin/queue-monitoring" element={<QueueMonitoringPage />} />
        <Route path="/admin/users" element={<UserManagementPage />} />
        
               
      </Routes>
    </BrowserRouter>
  );
}

export default App;