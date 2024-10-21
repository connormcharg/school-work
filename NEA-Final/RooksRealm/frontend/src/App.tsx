import React from 'react';
import { BrowserRouter as Router, Route, Routes, Navigate } from 'react-router-dom';
import Sidebar from './components/Sidebar';
import Play from './components/Play';
import StartGame from './components/StartGame';
import PlayGame from './components/PlayGame';
import Account from './components/account/account';
import Login from './components/account/login';
import Register from './components/account/register';
import Authorize from './components/account/authorize';
import './App.css';

const App: React.FC = () => {
    return (
        <Router>
            <div className="flex h-screen">
                <Sidebar />
                <div className="flex-1 p-6 bg-gray-300">
                    <Routes>
                        <Route path="/" element={
                            <Authorize
                                authorized={<Play />}
                                unauthorized={<Navigate to="/login" replace />}
                            />
                        } />
                        <Route path="/start" element={
                            <Authorize
                                authorized={<StartGame />}
                                unauthorized={<Navigate to="/login" replace />}
                            />
                        } />
                        <Route path="/play/:id" element={
                            <Authorize
                                authorized={<PlayGame boardSize={36} />}
                                unauthorized={<Navigate to="/login" replace />}
                            />
                        } />
                        <Route path="/account" element={<Account />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/register" element={<Register />} />
                        <Route path="*" element={<Navigate to="/" replace />} />
                    </Routes>
                </div>
            </div>
        </Router>
    );
};

export default App;