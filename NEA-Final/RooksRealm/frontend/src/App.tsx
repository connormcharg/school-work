import React from "react";
import {
  BrowserRouter as Router,
  Route,
  Routes,
  Navigate,
} from "react-router-dom";
import { AuthProvider, useAuth } from "./contexts/AuthProvider";
import Sidebar from "./components/Sidebar";
import Play from "./components/Play";
import Login from "./components/account/Login";
import Register from "./components/account/Register";
import Account from "./components/account/Account";
import PlayGame from "./components/gameplay/PlayGame";
import StartGame from "./components/gameplay/StartGame";
import "./App.css";
import AnnouncementBoard from "./components/AnnouncementBoard";
import GameStatsBoard from "./components/GameStats";

const AppRoutes: React.FC = () => {
  const { isLoggedIn } = useAuth();
  return (
    <Routes>
      <Route path="/" element={<Play />} />
      {isLoggedIn ? (
        <>
          <Route path="/login" element={<Navigate to="/" replace />} />
          <Route path="/register" element={<Navigate to="/" replace />} />
          <Route path="/account" element={<Account />} />
          <Route path="/play/:id" element={<PlayGame boardSize={40} />} />
          <Route path="/start" element={<StartGame />} />
          <Route path="/messages" element={<AnnouncementBoard />} />
          <Route path="/stats" element={<GameStatsBoard />} />
        </>
      ) : (
        <>
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/account" element={<Navigate to="/" replace />} />
          <Route path="/play/:id" element={<Navigate to="/" replace />} />
          <Route path="/start" element={<Navigate to="/" replace />} />
          <Route path="/messages" element={<Navigate to="/" replace />} />
          <Route path="/stats" element={<Navigate to="/" replace />} />
        </>
      )}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
};

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="flex h-screen">
          <Sidebar />
          <div className="flex-1 bg-gray-300 p-6">
            <AppRoutes />
          </div>
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;
