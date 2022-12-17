import styled from "@emotion/styled";
import { Divider, IconButton, Stack, Theme, Tooltip, Typography } from "@mui/material";
import { useRouter } from "next/router";
import { ReactNode, useContext } from "react";
import { ModeContext } from "../../contexts/mode.context";
import { AppIcon } from "./app-icon.component";
import DarkModeIcon from "@mui/icons-material/DarkModeTwoTone";
import LightModeIcon from "@mui/icons-material/LightMode";

const Root = styled.div`
    height: 100vh;
    width: 100%;
    position: relative;
    display: flex;
    flex-direction: column;
    box-shadow: 1px 0 0 0 #4a4c50, 0 0 0 100vw #292a2d;
`;

const Menubar = styled(Stack)(
    ({ theme }: { theme: Theme }) => `
  height: ${theme.spacing(8.5)};
  padding: ${theme.spacing(1)};
  flex-direction: row;
  align-items: center;
`
);

const Statusbar = styled(Stack)(
    ({ theme }: { theme: Theme }) => `
  height: ${theme.spacing(6.5)};
  padding: ${theme.spacing(1)};
  flex-direction: row;
  align-items: center;
`
);

const Controlbar = styled(Stack)`
    flex-direction: row;
    align-items: center;
    margin-left: auto;
`;

const Main = styled.div(
    ({ theme }: { theme: Theme }) => `
  padding: ${theme.spacing(3)};
  flex: 1;
  display: flex;
  flex-direction: column;
  position: relative;
  overflow: auto;
`
);

type LayoutProps = {
    theme: Theme;
    children: ReactNode;
};

export const Layout = (props: LayoutProps) => {
    const { theme, children } = props;
    const router = useRouter();
    const { mode, toggleMode } = useContext(ModeContext);

    return (
        <Root>
            <Menubar theme={theme}>
                <IconButton
                    size="medium"
                    disableRipple
                    onClick={() => {
                        router.push("/");
                    }}
                >
                    <AppIcon fontSize="medium" />
                    <Typography
                        variant="h2"
                        fontWeight={600}
                        sx={(theme) => ({
                            fontFamily: theme.typography["fontFamilyAlt"],
                            paddingLeft: 1,
                        })}
                    >
                        LMP Prices
                    </Typography>
                </IconButton>
                <Controlbar />
                <Tooltip title={mode === "light" ? "Dark Mode" : "Light Mode"}>
                    <IconButton aria-label="mode" size="medium" onClick={toggleMode}>
                        {mode === "light" ? <DarkModeIcon fontSize="inherit" /> : <LightModeIcon fontSize="inherit" />}
                    </IconButton>
                </Tooltip>
            </Menubar>
            <Divider />
            <Main theme={theme}>{children}</Main>
            <Divider />
            <Statusbar theme={theme}></Statusbar>
        </Root>
    );
};
