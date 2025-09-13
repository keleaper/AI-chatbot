// src/components/Sidebar.jsx
import React from "react";
import { Link, useNavigate } from "react-router-dom";
import "../ChatBox.css";

export default function Sidebar({ chatSessions, selectedSessionId, loadSession, setMenuOpen, menuOpen, user, setUser, token }) {

    const navigate = useNavigate();

    function handleLogout() {
        localStorage.removeItem("token"); // when a user logs in the backend issue them a JWT token and is stored in localStorage. My removing it we remove proof they were logged in
        setMenuOpen(false);
        setUser(null);
        navigate("/");
    }

  return (
    <div className="chat-sessions-sidebar">
      <h3>Previous Chats</h3>
      {chatSessions.length === 0 && <p>No previous chats found.</p>}

      <ul style={{ listStyle: "none", padding: 0 }}>
        {chatSessions.map((session) => (
          <li
            key={session.id}
            onClick={() => loadSession(session.id)}
            style={{
              cursor: "pointer",
              padding: "8px",
              backgroundColor: session.id === selectedSessionId ? "#e4e4e7" : "transparent",
              borderRadius: "4px",
              marginBottom: "5px",
              transition: "background-color 0.2s",
            }}
            title={session.lastMessage}
          >
            <strong>{session.title || `Chat ${session.id}`}</strong>
            <br />
            <small>{new Date(session.lastUpdated).toLocaleString()}</small>
          </li>
        ))}
      </ul>

      {/* sidebar dropdown menu */}
      <div className="sidebar-menu">
        <button className="menu-button" onClick={() => setMenuOpen(!menuOpen)}>
         ☰ {user ? user : "Menu"}
        </button>
        <div className={`menu-dropup ${menuOpen ? "open" : ""}`}>
          {/* <Link to="/" onClick={() => setMenuOpen(false)}>Chat</Link> */}
          {!token ? (
            <Link to="/auth" onClick={() => setMenuOpen(false)}>Login</Link>
          ) : (
            <button onClick={handleLogout} className="logout-button">Logout</button>
          )}
        </div>
      </div>
    </div>
  );
}
