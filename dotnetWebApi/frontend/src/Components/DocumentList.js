import React, { useEffect, useState } from 'react';
import { getUserDocuments } from '../api';

const DocumentList = () => {
    const [documents, setDocuments] = useState([]);

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

    return (
        <div>
            <h2>Мои документы</h2>
            {documents.length > 0 ? (
                <ul>
                    {documents.map((doc) => (
                        <li key={doc.id}>
                            <h3>{doc.title}</h3>
                            <p>{doc.content}</p>
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