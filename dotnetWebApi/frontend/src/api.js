import axios from 'axios';

const api = axios.create({
    baseURL: 'http://localhost:5100/api', // Убедитесь, что это правильный URL вашего .NET API
});

// Добавляем токен в заголовок каждого запроса
api.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});

// Функция для входа
export const login = async (username, password) => {
    try {
        const response = await api.post('/Auth/login', { username, password });
        return response.data; // Возвращаем данные ответа
    } catch (err) {
        // Обрабатываем ошибку
        if (err.response) {
            // Сервер вернул ответ с ошибкой
            throw err.response.data;
        } else if (err.request) {
            // Запрос был сделан, но ответ не получен (например, сервер недоступен)
            throw { message: 'Сервер недоступен. Попробуйте позже.' };
        } else {
            // Ошибка на стороне клиента (например, неправильные параметры запроса)
            throw { message: 'Ошибка при отправке запроса.' };
        }
    }
};

// Функция для регистрации
export const register = async (username, firstName, password) => {
    try {
        const response = await api.post('/Auth/register', { username, firstName, password });
        return response.data; // Возвращаем данные ответа
    } catch (err) {
        // Обрабатываем ошибку
        if (err.response) {
            // Сервер вернул ответ с ошибкой
            throw err.response.data;
        } else if (err.request) {
            // Запрос был сделан, но ответ не получен (например, сервер недоступен)
            throw { message: 'Сервер недоступен. Попробуйте позже.' };
        } else {
            // Ошибка на стороне клиента (например, неправильные параметры запроса)
            throw { message: 'Ошибка при отправке запроса.' };
        }
    }
};

// Функция для получения документов пользователя
export const getUserDocuments = async () => {
    try {
        const response = await api.get('/Documents');
        return response.data; // Возвращаем данные ответа
    } catch (err) {
        // Обрабатываем ошибку
        if (err.response) {
            // Сервер вернул ответ с ошибкой
            throw err.response.data;
        } else if (err.request) {
            // Запрос был сделан, но ответ не получен (например, сервер недоступен)
            throw { message: 'Сервер недоступен. Попробуйте позже.' };
        } else {
            // Ошибка на стороне клиента (например, неправильные параметры запроса)
            throw { message: 'Ошибка при отправке запроса.' };
        }
    }
};