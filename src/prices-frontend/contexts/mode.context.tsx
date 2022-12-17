import _ from "lodash";
import { createContext } from "react";

export type ModeContextType = {
    mode: "light" | "dark" /*| "auto"*/;
    setModeLight: () => void;
    setModeDark: () => void;
    //setModeAuto: () => void;
    toggleMode: () => void;
};

const initialValue: ModeContextType = {
    mode: "light",
    setModeLight: _.noop,
    setModeDark: _.noop,
    //setModeAuto: _.noop,
    toggleMode: _.noop,
};

export const ModeContext = createContext<ModeContextType>(initialValue);

// Set the display name for the context object for easier debugging
ModeContext.displayName = "ModeContext";

// Define the type for the provider component
type ModeProviderProps = React.ProviderProps<ModeContextType>;

/**
 * @example
 * const ctx = {mode, setModeLight, setModeDark, setModeAuto, toggleMode};
 *
 * <ModeProvider value={ctx}>
 *   ...
 * </ModeProvider>
 */
export const ModeProvider = (props: ModeProviderProps) => {
    return <ModeContext.Provider {...props} />;
};
