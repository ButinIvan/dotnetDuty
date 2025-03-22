import React from 'react';
import { Link } from 'react-router-dom';

const Navbar = () => {
    return (
        <nav style={{ display: 'flex', justifyContent: 'flex-end', padding: '10px', gap: '10px' }}>
            <Link to="/register" style={{ textDecoration: 'none', color: 'white', backgroundColor: 'green', padding: '10px', borderRadius: '5px' }}>
                Регистрация
            </Link>
            <Link to="/login" style={{ textDecoration: 'none', color: 'white', backgroundColor: 'blue', padding: '10px', borderRadius: '5px' }}>
                Войти
            </Link>
        </nav>
    );
};

export default Navbar;