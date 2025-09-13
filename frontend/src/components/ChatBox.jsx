// src/components/ChatBox.jsx
import React, { useState, useEffect } from "react";
import axios from "axios";
import ReactMarkdown from "react-markdown";
import "../ChatBox.css";
import Sidebar from "./Sidebar";


function ChatBox({ user, setUser }) { // user here is a way of destructering (more modern rn) other way was using (props) and calling props.user either way works
  const [input, setInput] = useState("");
  const [messages, setMessages] = useState([
    { role: "assistant", content: "Your helpful assistant is ready." }, // almost like a welcome message
  ]);
  const [chatSessions, setChatSessions] = useState([]); // List of previous chats
  const [selectedSessionId, setSelectedSessionId] = useState(null);
  const [menuOpen, setMenuOpen] = useState(false);

  const token = localStorage.getItem("token"); // Get token from local storage

  // Fetch chat sessions list on mount
  useEffect(() => {
    if (!token) {
      setChatSessions([]); // Clear sessions if logged out
      setSelectedSessionId(null);
      return;
    }

    axios
      .get("http://localhost:5267/api/ChatHistory/List", {
        headers: { Authorization: `Bearer ${token}` },
        withCredentials: true
      })
      .then((res) => setChatSessions(res.data))
      .catch((err) => console.error("Failed to fetch chat sessions", err));
  }, [token]);

  // Load a selected chat session messages
  const loadSession = (sessionId) => {
    if (!token) return;
    axios
      .get(`http://localhost:5267/api/ChatHistory/${sessionId}`, {
        headers: { Authorization: `Bearer ${token}` },
      })
      .then((res) => {
        setMessages(res.data);
        setSelectedSessionId(sessionId);
      })
      .catch((err) => console.error("Failed to load chat session", err));
  };

  const sendMessage = async () => {
    if (!input.trim()) {
      return;
    }

    const updatedMessages = [...messages, { role: "user", content: input }];
    setMessages(updatedMessages);
    setInput("");

    // Strip out Id property from each message before sending
    // This is because the backend expects a specific format without Ids
    const cleanedMessages = updatedMessages.map(({ role, content }) => ({ 
      role, content 
    }));
    
    const headers = token ? { Authorization: `Bearer ${token}` } : {};

    try {
      const response = await axios.post("http://localhost:5267/api/chat", 
        { Messages: cleanedMessages, SessionId: selectedSessionId }, // Messages and SessionId are whats expected in the backend (ChatRequest.cs 13-14) this is because were talking with the backend not just spreading in our messages state
        { headers }
      );

      const botMessage = { role: "assistant", content: response.data.response };
      setMessages((prev) => [...prev, botMessage]);

      // store new sessionId if it was created
      if (response.data.sessionId) {
        setSelectedSessionId(response.data.sessionId);
      }
    } catch (err) {
      console.error("Error talking to backend", err);
    }
  };

  return (
    <div className="chat-page">
      {/* Sidebar extracted into its own component (Bring props over to Sidebar.jsx)*/}
      <Sidebar 
        chatSessions={chatSessions}
        selectedSessionId={selectedSessionId}
        loadSession={loadSession}
        setMenuOpen={setMenuOpen}
        menuOpen={menuOpen}
        user={user}
        setUser={setUser}
        token={token}
      />
      
      {/* Sidebar dropdown menu */}
      
      
      {/* Main Chat Area */}
      <div className="chat-container">
        <h2 className="chat-title">AI Chatbot</h2>
        <div className="chat-box">
          {messages.map((msg, idx) => (
            <div key={idx} className={`chat-message ${msg.role}`}>
              <strong>{msg.role === "user" ? "You" : "AI"}:</strong>
              <div className="chat-text">
                <ReactMarkdown>{msg.content}</ReactMarkdown>
              </div>
            </div>
          ))}
        </div>
        <input
          className="input-box"
          type="text"
          value={input}
          onChange={(event) => setInput(event.target.value)}
          onKeyDown={(event) => event.key === "Enter" && sendMessage()}
          placeholder="Type your message..."
        />
      </div>
    </div>
  );
}

export default ChatBox;