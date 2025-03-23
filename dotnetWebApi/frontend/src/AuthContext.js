import React, { createContext, useState, useContext } from 'react';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
    const [isAuthenticated, setIsAuthenticated] = useState(!!localStorage.getItem('token'));

    const login = (token) => {
        console.log('Пользователь вошёл в систему'); // Логируем вход
        localStorage.setItem('token', token);
        setIsAuthenticated(true);
    };

    const logout = async () => {
        try {
            await fetch('/api/Auth/logout', {
                method: 'POST',
                headers: {
                    'Authorization': `Bearer ${localStorage.getItem('token')}`,
                },
            });
        } catch (err) {
            console.error('Ошибка при выходе:', err);
        } finally {
            console.log('Пользователь вышел из системы'); // Логируем выход
            localStorage.removeItem('token');
            setIsAuthenticated(false);
        }
    };

    return (
        <AuthContext.Provider value={{ isAuthenticated, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

export const useAuth = () => useContext(AuthContext);