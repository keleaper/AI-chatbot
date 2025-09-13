// src/components/AuthForm.jsx
import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import axiosClient from "../axiosClient";
import { motion } from "framer-motion";
import "../Form.css";

export default function AuthForm({ onAuthSuccess }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [isLogin, setIsLogin] = useState(true);

  const navigate = useNavigate();

  const handleSubmit = async (event) => {
    event.preventDefault();

    try {
      const res = await axiosClient.post(`/Auth/${isLogin ? "login" : "register"}`,
         { username, password },
         { withCredentials: true }
      );

      if (isLogin) {
        localStorage.setItem("token", res.data.token);
        onAuthSuccess(res.data.username);
        navigate("/"); // Redirect to chat page on successful login
      } else {
        alert("Registration successful! You can now log in.");
        setIsLogin(true);
      }
    } catch (error) {
      console.log("Login error:", error);
      alert(error.response?.data.message || error.response?.data || error.message || "Error occurred");
    }
  };

  // style={{ display: "inline-grid", boxShadow: "0px 5px 10px", padding: "1rem", border: "1px solid #ccc", width: "500px" }}

  return (
    // creates the motion for AuthForm page
    <motion.div 
      className="page-container"
      initial={{ opacity: 0, y:-50 }}
      animate={{ opacity: 1, y: 0 }}
      exit={{ opacity: 0, y: 50 }}
      transition={{ duration: 0.5, ease: "easeOut" }}
    >
      <div className="form-box">
        <h2>{isLogin ? "Login" : "Register"}</h2>
        <form onSubmit={handleSubmit}>
          <input
            placeholder="Username"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
          <input
            placeholder="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
          <button type="submit">{isLogin ? "Login" : "Register"}</button>
        </form>
        <p onClick={() => setIsLogin(!isLogin)} style={{ cursor: "pointer", marginTop: "1rem", color: "blue" }}>
          {isLogin ? "No account? Register" : "Have an account? Login"}
        </p>
      </div>
    </motion.div>
  );
}
