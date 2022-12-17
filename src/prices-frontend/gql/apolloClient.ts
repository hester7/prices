import { ApolloClient, HttpLink, InMemoryCache, split } from "@apollo/client";
import { GraphQLWsLink } from "@apollo/client/link/subscriptions";
import { createClient } from "graphql-ws";
import { getMainDefinition } from "@apollo/client/utilities";

const wsLink =
    typeof window !== "undefined"
        ? new GraphQLWsLink(
              createClient({
                  url: `${process.env.NEXT_PUBLIC_GRAPHQL_WS_ENDPOINT}`,
              })
          )
        : null;

const httpLink = new HttpLink({
    uri: process.env.NEXT_PUBLIC_GRAPHQL_HTTP_ENDPOINT,
});

const link =
    typeof window !== "undefined" && wsLink != null
        ? split(
              ({ query }) => {
                  const def = getMainDefinition(query);
                  return def.kind === "OperationDefinition" && def.operation === "subscription";
              },
              wsLink,
              httpLink
          )
        : httpLink;

export const apolloClient = new ApolloClient({
    link,
    cache: new InMemoryCache(),
});
