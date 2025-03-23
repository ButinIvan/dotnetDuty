import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom'; // Импортируем useParams для получения documentId
import { getDocument } from '../api'; // Импортируем функцию для получения документа

const DocumentDetails = () => {
    const { documentId } = useParams(); // Получаем documentId из URL
    const [document, setDocument] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        const fetchDocument = async () => {
            try {
                const response = await getDocument(documentId); // Загружаем документ по ID
                setDocument(response);
            } catch (err) {
                setError('Ошибка при загрузке документа');
            } finally {
                setLoading(false);
            }
        };
        fetchDocument();
    }, [documentId]);

    if (loading) {
        return <p>Загрузка...</p>;
    }

    if (error) {
        return <p style={{ color: 'red' }}>{error}</p>;
    }

    return (
        <div>
            <h2>Детали документа</h2>
            {document ? (
                <div>
                    <h3>{document.title}</h3>
                    <p>{document.content}</p>
                    <p>ID: {document.id}</p>
                    <p>Владелец: {document.ownerId}</p>
                    <p>Завершён: {document.isFinished ? 'Да' : 'Нет'}</p>
                    <p>Комментарии закрыты: {document.isClosedToComment ? 'Да' : 'Нет'}</p>
                    <p>Последнее изменение: {new Date(document.lastModified).toLocaleString()}</p>
                </div>
            ) : (
                <p>Документ не найден.</p>
            )}
        </div>
    );
};

export default DocumentDetails;