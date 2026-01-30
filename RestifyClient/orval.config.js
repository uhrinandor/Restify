module.exports = {
    adminApi: {
        input: '../RestifyServer/openapi/RestifyServer.json',
        output: {
            target: './src/api/generated/index.ts',
            schemas: './src/api/generated/models',
            mode: 'tags',
            client: 'angular-query'
        },
    },
};
