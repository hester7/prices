import "../styles/globals.css";
import "../styles/main.scss";
import type { AppProps } from "next/app";
import { ApolloProvider } from "@apollo/client";
import { Layout } from "../components/core/layout.component";
import { apolloClient } from "../gql/apolloClient";
import { CssBaseline, ThemeProvider } from "@mui/material";
import { usePreferredTheme } from "../theme/use-preferred-theme";
import { ModeProvider } from "../contexts/mode.context";
import { EnumMetadataProvider } from "../contexts/enum-metadata.context";

export default function App({ Component, pageProps }: AppProps) {
    const { theme, modeContext } = usePreferredTheme();

    return (
        <ApolloProvider client={apolloClient}>
            <ThemeProvider theme={theme}>
                <ModeProvider value={modeContext}>
                    <EnumMetadataProvider>
                        <CssBaseline enableColorScheme />
                        <Layout theme={theme}>
                            <Component {...pageProps} />
                        </Layout>
                    </EnumMetadataProvider>
                </ModeProvider>
            </ThemeProvider>
        </ApolloProvider>
    );
}
