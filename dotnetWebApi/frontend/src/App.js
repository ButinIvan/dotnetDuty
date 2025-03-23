import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import { AuthProvider } from './AuthContext'; 
import Navbar from './сomponents/Navbar';
import LoginForm from './сomponents/LoginForm';
import RegisterForm from './сomponents/RegisterForm';
import DocumentList from './сomponents/DocumentList';
import DocumentDetails from './сomponents/DocumentDetails';

const App = () => {
    return (
        <AuthProvider>
            <Router>
                <Navbar />
                <Routes>
                    <Route path="/login" element={<LoginForm />} />
                    <Route path="/register" element={<RegisterForm />} />
                    <Route path="/profile" element={<DocumentList />} />
                    <Route path="/documents/:documentId" element={<DocumentDetails />} />
                </Routes>
            </Router>
        </AuthProvider>
    );
};

export default App;