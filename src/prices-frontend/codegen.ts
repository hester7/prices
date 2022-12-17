import { CodegenConfig } from "@graphql-codegen/cli";

const config: CodegenConfig = {
    schema: process.env.NEXT_PUBLIC_GRAPHQL_HTTP_ENDPOINT,
    documents: "./gql/documents/**/*.ts",
    ignoreNoDocuments: true, // for better experience with the watcher
    generates: {
        "./gql/codegen/": {
            preset: "client",
            plugins: [],
        },
    },
};

export default config;
