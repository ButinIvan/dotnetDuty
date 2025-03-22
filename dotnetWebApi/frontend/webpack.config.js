const path = require('path');

module.exports = {
    devServer: {
        setupMiddlewares: (middlewares, devServer) => {
            if (!devServer) {
                throw new Error('webpack-dev-server is not defined');
            }

            // Добавьте свои middleware здесь, если нужно
            return middlewares;
        },
    },
};