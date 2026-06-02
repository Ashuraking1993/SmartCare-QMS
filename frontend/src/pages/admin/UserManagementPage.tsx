import { useEffect, useState } from "react";
import "./UserManagementPage.css";
import logo from "../../assets/logo.png";
import {
  getUsers,
  getBranches,
  getCounters
} from "../../services/adminService";

const UserManagementPage = () => {
const [users, setUsers] = useState<any[]>([]);

const loadUsers = async () => {
    const data = await getUsers();
    setUsers(data);
  };

const [showAddAgentModal, setShowAddAgentModal] =
  useState(false);

const [branches, setBranches] = useState<any[]>([]);

const [counters, setCounters] = useState<any[]>([]);       

useEffect(() => {
  loadUsers();
  loadBranches();
  loadCounters();
}, []);

 const loadBranches = async () => {
  const data = await getBranches();
  setBranches(data);
};

const loadCounters = async () => {
  const data = await getCounters();
  setCounters(data);
}; 

const [form, setForm] = useState({
  firstName: "",
  lastName: "",
  email: "",
  password: "",
  branchId: "",
  counterId: ""
});

 return (
    <div className="user-page">
         <img
        src={logo}
        alt="Banco Logo"
        className="user-watermark"
        />

      <div className="user-card">
      <h1 className="user-title">
        USER MANAGEMENT
        </h1>

       <button
            className="add-agent-btn"
            onClick={() => setShowAddAgentModal(true)}>
    
            + Add Agent
        </button>
       

      <table className="admin-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Username</th>
            <th>Full Name</th>
            <th>Role</th>
            <th>Branch</th>
            <th>Counter</th>
          </tr>
        </thead>

            <tbody>
                {users.map((user) => (
                <tr key={user.id}>
            <td>{user.firstName}</td>
            <td>{user.lastName}</td>
            <td>{user.email}</td>
            <td>{user.role?.name}</td>
            <td>{user.branch?.name}</td>
            <td>{user.counter?.name}</td>
            </tr>
        ))}
        </tbody>
      </table>
      </div>

        {showAddAgentModal && (
  <div className="modal-overlay">
    <div className="modal-card">

      <h2 className="modal-title">
        Add Agent
      </h2>

      <div className="modal-form">

        <input
          type="text"
          placeholder="First Name"
          value={form.firstName}
          onChange={(e) =>
            setForm({
              ...form,
              firstName: e.target.value
            })
          }
        />

        <input
          type="text"
          placeholder="Last Name"
          value={form.lastName}
          onChange={(e) =>
            setForm({
              ...form,
              lastName: e.target.value
            })
          }
        />

        <input
          type="email"
          placeholder="Email"
          value={form.email}
          onChange={(e) =>
            setForm({
              ...form,
              email: e.target.value
            })
          }
        />

        <input
          type="password"
          placeholder="Password"
          value={form.password}
          onChange={(e) =>
            setForm({
              ...form,
              password: e.target.value
            })
          }
        />

        <select
          value={form.branchId}
          onChange={(e) =>
            setForm({
              ...form,
              branchId: e.target.value
            })
          }
        >
          <option value="">
            Select Branch
          </option>

          {branches.map((branch) => (
            <option
              key={branch.id}
              value={branch.id}
            >
              {branch.name}
            </option>
          ))}
        </select>

        <select
          value={form.counterId}
          onChange={(e) =>
            setForm({
              ...form,
              counterId: e.target.value
            })
          }
        >
          <option value="">
            Select Counter
          </option>

          {counters.map((counter) => (
            <option
              key={counter.id}
              value={counter.id}
            >
              {counter.name}
            </option>
          ))}
        </select>

      </div>

      <div className="modal-actions">

        <button
          className="cancel-btn"
          onClick={() =>
            setShowAddAgentModal(false)
          }
        >
          Cancel
        </button>

        <button className="save-btn">
          Create Agent
        </button>

      </div>

    </div>
  </div>
)}

        

     </div>

      
  );
};

export default UserManagementPage;