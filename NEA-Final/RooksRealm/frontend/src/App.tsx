import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthProvider';
import Sidebar from './components/Sidebar';
import Play from './components/Play';
import Login from './components/account/Login';
import Register from './components/account/Register';
import './App.css'

const AppRoutes: React.FC = () => {
  const { isLoggedIn } = useAuth();
  return (
    <Routes>
      <Route path="/" element={<Play />} />
      {
        isLoggedIn ?
          <>
            <Route path="/login" element={<Navigate to="/" replace />} />
            <Route path="/register" element={<Navigate to="/" replace />} />
            <Route path="/account" element={<Navigate to="/" replace />} />
          </> :
          <>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/account" element={<Navigate to="/" replace />} />
          </>
      }
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="flex h-screen">
          <Sidebar />
          <div className="flex-1 p-6 bg-gray-300">
            <AppRoutes />
          </div>
        </div>
      </Router>
    </AuthProvider>
  )
}

export default App;