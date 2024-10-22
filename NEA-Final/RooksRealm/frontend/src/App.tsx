import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthProvider';
import Sidebar from './components/Sidebar';
import Play from './components/Play';

import './App.css'

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="flex h-screen">
          <Sidebar />
          <div className="flex-1 p-6 bg-gray-300">
            <Routes>
              <Route path="/" element={<Play />} />
            </Routes>
          </div>
        </div>
      </Router>
    </AuthProvider>
  )
}

export default App;