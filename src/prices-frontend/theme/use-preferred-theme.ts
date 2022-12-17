import { Theme } from "@mui/material";
import { useEffect, useMemo, useState } from "react";

import { Config, syncWithStorage } from "../config";
import { ModeContextType } from "../contexts/mode.context";
import { createTheme } from "../theme";

export const usePreferredTheme = (initial?: string | undefined): { theme: Theme; modeContext: ModeContextType } => {
    //const prefersDarkMode = useMediaQuery("(prefers-color-scheme: dark)");
    const [mode, setMode] = useState<string | undefined>(initial);

    useEffect(() => {
        if (initial === undefined) {
            //const stored = Config.MODE ?? (prefersDarkMode ? "dark" : "light");
            const stored = Config.MODE ?? "dark";

            if (stored && mode !== stored) {
                setMode(stored);
            }
        }
    }, []);

    useEffect(() => {
        syncWithStorage({ MODE: mode });
    }, [mode]);

    return useMemo(() => {
        return {
            theme: createTheme(mode),
            modeContext: {
                mode,
                setModeLight() {
                    setMode("light");
                },
                setModeDark() {
                    setMode("dark");
                },
                // setModeAuto() {
                //     setMode(prefersDarkMode ? "dark" : "light");
                // },
                toggleMode() {
                    setMode(mode === "light" ? "dark" : "light");
                },
            } as ModeContextType,
        };
    }, [/*prefersDarkMode,*/ mode]);
};
