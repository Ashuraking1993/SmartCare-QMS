import { useEffect, useState } from "react";
import {
  getDashboardStats,
  getBranches,
  getCounters,
  getUsers,
  createAgent,
  createBranch,
  deleteBranch,
  updateBranch,
  createCounter,
  updateCounter,
  deleteCounter,
} from "../../services/adminService";

import { useNavigate } from "react-router-dom";
import "./AdminDashboard.css";
import logo from "../../assets/logo.png";

interface DashboardStats {
  totalWaiting: number;
  totalServing: number;
  totalCompletedToday: number;
  activeCounters: number;
}

export default function AdminDashboard() {
  const navigate = useNavigate();

  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [branches, setBranches] = useState<any[]>([]);
  const [counters, setCounters] = useState<any[]>([]);
  const [users, setUsers] = useState<any[]>([]);

  const [showAddBranch, setShowAddBranch] = useState(false);
  const [editingBranch, setEditingBranch] = useState<any>(null);
  const [branchToDelete, setBranchToDelete] = useState<string | null>(null);
  const [branchForm, setBranchForm] = useState({ name: "", code: "" });

  const [showAddCounter, setShowAddCounter] = useState(false);
  const [showEditCounter, setShowEditCounter] = useState(false);
  const [selectedCounter, setSelectedCounter] = useState<any>(null);
  const [counterForm, setCounterForm] = useState({ name: "", branchId: "" });

  const [showCreateAgent, setShowCreateAgent] = useState(false);
  const [agentForm, setAgentForm] = useState<{
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    branchId: string | null;
    counterId: string | null;
  }>({
    firstName: "",
    lastName: "",
    email: "",
    password: "",
    branchId: null,
    counterId: null,
  });

  useEffect(() => {
    const token = localStorage.getItem("token");
    if (!token) navigate("/admin-login");
  }, []);

  useEffect(() => {
    loadDashboard();
    loadBranches();
    loadCounters();
    loadUsers();
  }, []);

  const loadDashboard = async () => {
    const data = await getDashboardStats();
    setStats(data);
  };
  const loadBranches = async () => setBranches(await getBranches());
  const loadCounters = async () => setCounters(await getCounters());
  const loadUsers = async () => setUsers(await getUsers());

  const handleCreateAgent = async () => {
    try {
      await createAgent(agentForm);
      await loadUsers();
      setShowCreateAgent(false);
      setAgentForm({ firstName: "", lastName: "", email: "", password: "", branchId: null, counterId: null });
    } catch {
      alert("Failed to create agent");
    }
  };

  const handleCreateBranch = async () => {
    try {
      await createBranch(branchForm);
      await loadBranches();
      setShowAddBranch(false);
      setBranchForm({ name: "", code: "" });
    } catch {
      alert("Failed to create branch");
    }
  };

  const handleUpdateBranch = async () => {
    try {
      await updateBranch(editingBranch.id, { ...branchForm, isActive: true });
      await loadBranches();
      setEditingBranch(null);
      setBranchForm({ name: "", code: "" });
    } catch {
      alert("Failed to update branch");
    }
  };

  const handleDeleteBranch = async (id: string) => {
    await deleteBranch(id);
    await loadBranches();
    setBranchToDelete(null);
  };

  const handleCreateCounter = async () => {
    await createCounter({ name: counterForm.name, branchId: counterForm.branchId });
    await loadCounters();
    setShowAddCounter(false);
    setCounterForm({ name: "", branchId: "" });
  };

  const handleUpdateCounter = async () => {
    await updateCounter(selectedCounter.id, { ...counterForm, isActive: true });
    await loadCounters();
    setShowEditCounter(false);
  };

  const openEditCounter = (counter: any) => {
    setSelectedCounter(counter);
    setCounterForm({ name: counter.name, branchId: counter.branchId });
    setShowEditCounter(true);
  };

  const handleDeleteCounter = async (id: string) => {
    if (!window.confirm("Disable this counter?")) return;
    await deleteCounter(id);
    await loadCounters();
  };

  return (
    <div className="adm-root">

      {/* ── Sidebar ── */}
      <aside className="adm-sidebar">
              <div className="adm-sidebar-brand">
          <img
            src={logo}
            alt="SmartCare Hospital"
            className="adm-sidebar-logo"
          />

          <div className="adm-brand-info">
            <strong>ADMIN PORTAL</strong>
            <span>Smart Queue Management</span>
          </div>
        </div>
        <nav className="adm-nav">
          <span className="adm-nav-group">Main</span>
          <div className="adm-nav-item adm-nav-active" onClick={() => navigate("/admin/dashboard")}>
            <span className="adm-nav-icon">📊</span> Dashboard
          </div>
          <div className="adm-nav-item" onClick={() => navigate("/admin/branches")}>
            <span className="adm-nav-icon">🏢</span> Branches
          </div>
          <div className="adm-nav-item" onClick={() => navigate("/admin/counters")}>
            <span className="adm-nav-icon">🖥️</span> Counters
          </div>
          <div className="adm-nav-item" onClick={() => navigate("/admin/users")}>
            <span className="adm-nav-icon">👥</span> Users
          </div>

          <span className="adm-nav-group">Operations</span>
          <div className="adm-nav-item" onClick={() => navigate("/admin/queue-monitoring")}>
            <span className="adm-nav-icon">🎫</span> Queue Monitoring
          </div>
        </nav>

        <div className="adm-sidebar-footer">
          Forged by Digital Ronin
        </div>
      </aside>

      {/* ── Main ── */}
      <main className="adm-main">

        {/* Watermark */}
        <img src={logo} alt="" className="adm-watermark" />

        {/* Header */}
        <div className="adm-hero">
          <h1>Super Admin Dashboard</h1>
          <p>Real-time overview of all branches, counters and queue activity.</p>
        </div>

        {/* Stats */}
        <div className="adm-stats-grid">
          <div className="adm-stat-card adm-stat-wait">
            <div className="adm-stat-top">
              <span className="adm-stat-label">WAITING</span>
              <div className="adm-stat-icon adm-icon-wait">⏳</div>
            </div>
            <div className="adm-stat-num">{stats?.totalWaiting ?? 0}</div>
            <div className="adm-stat-sub">Customers in queue</div>
          </div>
          <div className="adm-stat-card adm-stat-serve">
            <div className="adm-stat-top">
              <span className="adm-stat-label">SERVING</span>
              <div className="adm-stat-icon adm-icon-serve">✅</div>
            </div>
            <div className="adm-stat-num">{stats?.totalServing ?? 0}</div>
            <div className="adm-stat-sub">Currently being served</div>
          </div>
          <div className="adm-stat-card adm-stat-comp">
            <div className="adm-stat-top">
              <span className="adm-stat-label">COMPLETED</span>
              <div className="adm-stat-icon adm-icon-comp">🏁</div>
            </div>
            <div className="adm-stat-num">{stats?.totalCompletedToday ?? 0}</div>
            <div className="adm-stat-sub">Transactions today</div>
          </div>
          <div className="adm-stat-card adm-stat-count">
            <div className="adm-stat-top">
              <span className="adm-stat-label">COUNTERS</span>
              <div className="adm-stat-icon adm-icon-count">🖥️</div>
            </div>
            <div className="adm-stat-num">{stats?.activeCounters ?? 0}</div>
            <div className="adm-stat-sub">Active counters</div>
          </div>
        </div>

        {/* Content Grid */}
        <div className="adm-content-grid">

          {/* Branch Management */}
          <div className="adm-panel">
            <div className="adm-panel-header">
              <h2 className="adm-panel-title">Branch Management</h2>
              <button className="adm-btn-add" onClick={() => setShowAddBranch(true)}>
                + Add Branch
              </button>
            </div>
            {branches.map((branch) => (
              <div key={branch.id} className="adm-row">
                <div className="adm-row-dot" />
                <span className="adm-row-name">{branch.name}</span>
                <div className="adm-row-actions">
                  <button className="adm-btn-edit" onClick={() => {
                    setEditingBranch(branch);
                    setBranchForm({ name: branch.name, code: branch.code });
                  }}>Edit</button>
                  <button className="adm-btn-del" onClick={() => setBranchToDelete(branch.id)}>Delete</button>
                </div>
              </div>
            ))}
          </div>

          {/* Counter Management */}
          <div className="adm-panel">
            <div className="adm-panel-header">
              <h2 className="adm-panel-title">Counter Management</h2>
              <button className="adm-btn-add" onClick={() => setShowAddCounter(true)}>
                + Add Counter
              </button>
            </div>
            {counters.map((counter) => (
              <div key={counter.id} className="adm-row">
                <div className="adm-row-dot adm-dot-purple" />
                <span className="adm-row-name">{counter.name}</span>
                <div className="adm-row-actions">
                  <button className="adm-btn-edit" onClick={() => openEditCounter(counter)}>Edit</button>
                  <button className="adm-btn-del" onClick={() => handleDeleteCounter(counter.id)}>Disable</button>
                </div>
              </div>
            ))}
          </div>

        </div>
      </main>

      {/* ── Modals ── */}

      {/* Add Branch */}
      {showAddBranch && (
        <div className="adm-overlay">
          <div className="adm-modal">
            <h2 className="adm-modal-title">Add Branch</h2>
            <input className="adm-modal-input" placeholder="Branch Name"
              value={branchForm.name}
              onChange={(e) => setBranchForm({ ...branchForm, name: e.target.value })} />
            <input className="adm-modal-input" placeholder="Branch Code"
              value={branchForm.code}
              onChange={(e) => setBranchForm({ ...branchForm, code: e.target.value })} />
            <div className="adm-modal-actions">
              <button className="adm-btn-cancel" onClick={() => setShowAddBranch(false)}>Cancel</button>
              <button className="adm-btn-save" onClick={handleCreateBranch}>Save</button>
            </div>
          </div>
        </div>
      )}

      {/* Edit Branch */}
      {editingBranch && (
        <div className="adm-overlay">
          <div className="adm-modal">
            <h2 className="adm-modal-title">Edit Branch</h2>
            <input className="adm-modal-input" placeholder="Branch Name"
              value={branchForm.name}
              onChange={(e) => setBranchForm({ ...branchForm, name: e.target.value })} />
            <input className="adm-modal-input" placeholder="Branch Code"
              value={branchForm.code}
              onChange={(e) => setBranchForm({ ...branchForm, code: e.target.value })} />
            <div className="adm-modal-actions">
              <button className="adm-btn-cancel" onClick={() => setEditingBranch(null)}>Cancel</button>
              <button className="adm-btn-save" onClick={handleUpdateBranch}>Save Changes</button>
            </div>
          </div>
        </div>
      )}

      {/* Delete Branch */}
      {branchToDelete && (
        <div className="adm-overlay">
          <div className="adm-modal adm-modal-sm">
            <h2 className="adm-modal-title">Delete Branch</h2>
            <p className="adm-modal-body">Are you sure you want to delete this branch? This action cannot be undone.</p>
            <div className="adm-modal-actions">
              <button className="adm-btn-cancel" onClick={() => setBranchToDelete(null)}>Cancel</button>
              <button className="adm-btn-danger" onClick={() => handleDeleteBranch(branchToDelete)}>Delete</button>
            </div>
          </div>
        </div>
      )}

      {/* Add Counter */}
      {showAddCounter && (
        <div className="adm-overlay">
          <div className="adm-modal">
            <h2 className="adm-modal-title">Add Counter</h2>
            <input className="adm-modal-input" placeholder="Counter Name"
              value={counterForm.name}
              onChange={(e) => setCounterForm({ ...counterForm, name: e.target.value })} />
            <select className="adm-modal-select"
              value={counterForm.branchId}
              onChange={(e) => setCounterForm({ ...counterForm, branchId: e.target.value })}>
              <option value="">Select Branch</option>
              {branches.map((b) => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
            <div className="adm-modal-actions">
              <button className="adm-btn-cancel" onClick={() => setShowAddCounter(false)}>Cancel</button>
              <button className="adm-btn-save" onClick={handleCreateCounter}>Save</button>
            </div>
          </div>
        </div>
      )}

      {/* Edit Counter */}
      {showEditCounter && (
        <div className="adm-overlay">
          <div className="adm-modal">
            <h2 className="adm-modal-title">Edit Counter</h2>
            <input className="adm-modal-input" placeholder="Counter Name"
              value={counterForm.name}
              onChange={(e) => setCounterForm({ ...counterForm, name: e.target.value })} />
            <select className="adm-modal-select"
              value={counterForm.branchId}
              onChange={(e) => setCounterForm({ ...counterForm, branchId: e.target.value })}>
              {branches.map((b) => (
                <option key={b.id} value={b.id}>{b.name}</option>
              ))}
            </select>
            <div className="adm-modal-actions">
              <button className="adm-btn-cancel" onClick={() => setShowEditCounter(false)}>Cancel</button>
              <button className="adm-btn-save" onClick={handleUpdateCounter}>Save Changes</button>
            </div>
          </div>
        </div>
      )}

    </div>
  );
}
