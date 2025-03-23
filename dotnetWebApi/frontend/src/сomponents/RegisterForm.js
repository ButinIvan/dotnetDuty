import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { register } from '../api';

const RegisterForm = () => {
    const [username, setUsername] = useState('');
    const [firstName, setFirstName] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            const response = await register(username, firstName, password);
            if (response.message === "User successfully registered") {
                navigate('/login'); // Переходим на страницу входа
            }
        } catch (err) {
            setError(err.message || 'Ошибка при регистрации');
        }
    };

    return (
        <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '10px', maxWidth: '300px', margin: 'auto' }}>
            <input
                type="text"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                placeholder="Имя пользователя"
                required
            />
            <input
                type="text"
                value={firstName}
                onChange={(e) => setFirstName(e.target.value)}
                placeholder="Имя"
                required
            />
            <input
                type="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="Пароль"
                required
            />
            <button type="submit">Зарегистрироваться</button>
            {error && <p style={{ color: 'red' }}>{error}</p>}
        </form>
    );
};

export default RegisterForm;