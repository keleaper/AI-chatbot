import React, { useState, createContext } from "react";
import { BrowserRouter as Router, Routes, Route, useLocation } from "react-router-dom";
import { AnimatePresence } from "framer-motion"; // adds page fade in and out

import ChatBox from "./components/ChatBox.jsx";
import AuthForm from "./components/AuthForm.jsx";
import ReactSwitch from "react-switch";
import './App.css';
import "./index.css";


// Context API 
export const ThemeContext = createContext(null);

function App() {
  const [theme, setTheme] = useState("light"); // Default theme is light
  const [user, setUser] = useState(null); // track logged in user

  const location = useLocation();

  function toggleTheme() {
    // If current theme is light, switch to dark and if its dark, switch to light
    setTheme((currentTheme) => (currentTheme === "light" ? "dark" : "light"));
  }

  function handleAuthSuccess(username) {
    setUser(username);
    console.log("Logged in as: ", username)
  }

  return (
    // fade in and out
    <AnimatePresence mode="wait"> 
      {/* Allows us to have a global state for theme instead of having to add a style tag to every element in ChatBox.jsx */}
      <ThemeContext.Provider value={{ theme, toggleTheme}}>
        <div className="App" id={theme}>
          <Routes location={location} key={location.pathname}>
            <Route path="/" element={<ChatBox user={user} setUser={setUser}/>} /> {/* giving ChatBox a prop called user that stores the user info from App.jsx(This file) so i can use it over in ChatBox.jsx */}
            <Route path="/auth" element={<AuthForm onAuthSuccess={handleAuthSuccess} />} />
          </Routes>

          {/* <ChatBox /> */}
          <div className="switch">
            <label>
              <span className="switch-label">Toggle Theme</span>
            </label>
            <ReactSwitch 
              onChange={toggleTheme} 
              checked={theme === "dark"} 
              checkedIcon={
                <img
                  src="dark-logo.png"
                  alt="Dark mode icon"
                  style={{ height: "25px", width: "25px", marginLeft: "3px", marginTop: "1px" }}
                />
                }
              uncheckedIcon={
                <img
                  src="light-logo.png"
                  alt="Light mode icon"
                  style={{ height: "25px", width: "25px", marginLeft: "3px", marginTop: "1px" }}
                />
              }
            />
          </div>
        </div>
      </ThemeContext.Provider>
    </AnimatePresence>
  );
}

export default App;
