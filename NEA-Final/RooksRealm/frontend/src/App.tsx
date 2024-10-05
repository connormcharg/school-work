import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Play from './components/Play';
import StartGame from './components/StartGame';
import PlayGame from './components/PlayGame';
import './App.css';

const App: React.FC = () => {
    return (
        <Router>
            <div className="flex h-screen">
                <Sidebar />
                <div className="flex-1 p-6 bg-gray-300">
                    <Routes>
                        <Route path="/" element={<Play />} />
                        <Route path="/start" element={<StartGame />} />
                        <Route path="/play/:id" element={<PlayGame />} />
                        <Route path="*" element={<Navigate to="/" replace />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
};

export default App;