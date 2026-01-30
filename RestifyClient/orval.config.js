module.exports = {
    restify: {
        output: {
            mode: 'tags-split',
            target: 'src/api/generated',
            schemas: 'src/api/model',
            client: 'angular-query'
        },
        input: {
            target: './openapi/RestifyServer.json'
        },
    },
};
