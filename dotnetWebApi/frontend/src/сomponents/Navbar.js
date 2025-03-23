import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext'; // Импортируем useAuth

const Navbar = () => {
    const { isAuthenticated, logout } = useAuth(); // Получаем состояние аутентификации и функцию logout

    return (
        <nav style={{ display: 'flex', justifyContent: 'flex-end', padding: '10px', gap: '10px' }}>
            {isAuthenticated ? (
                <Link to="/" style={{ textDecoration: 'none', color: 'white', backgroundColor: 'red', padding: '10px', borderRadius: '5px', border: 'none', cursor: 'pointer' }}>
                    Выйти
                </Link>
            ) : (
                <>
                    <Link to="/register" style={{ textDecoration: 'none', color: 'white', backgroundColor: 'green', padding: '10px', borderRadius: '5px' }}>
                        Регистрация
                    </Link>
                    <Link to="/login" style={{ textDecoration: 'none', color: 'white', backgroundColor: 'blue', padding: '10px', borderRadius: '5px' }}>
                        Войти
                    </Link>
                </>
            )}
        </nav>
    );
};

export default Navbar;