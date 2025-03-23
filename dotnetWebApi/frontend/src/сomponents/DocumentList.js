import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom'; // Импортируем useNavigate
import { getUserDocuments } from '../api';

const DocumentList = () => {
    const [documents, setDocuments] = useState([]);
    const navigate = useNavigate(); // Хук для навигации

    useEffect(() => {
        const fetchDocuments = async () => {
            try {
                const response = await getUserDocuments();
                console.log('Documents response:', response); // Логируем ответ
                setDocuments(response);
            } catch (err) {
                console.error('Ошибка при загрузке документов:', err);
            }
        };
        fetchDocuments();
    }, []);

    // Обработчик клика на название документа
    const handleDocumentClick = (documentId) => {
        navigate(`/documents/${documentId}`); // Переход на страницу документа
    };

    return (
        <div>
            <h2>Мои документы</h2>
            {documents.length > 0 ? (
                <ul>
                    {documents.map((doc) => (
                        <li key={doc.id} style={{ marginBottom: '10px' }}>
                            {/* Добавляем обработчик клика на название документа */}
                            <h3 onClick={() => handleDocumentClick(doc.id)} style={{ cursor: 'pointer', color: 'blue' }}>
                                <span>📄</span>
                                {doc.title}
                            </h3>
                        </li>
                    ))}
                </ul>
            ) : (
                <p>У вас пока нет документов.</p>
            )}
        </div>
    );
};

export default DocumentList;