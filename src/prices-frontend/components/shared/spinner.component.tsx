import { CircularProgress, Fade } from "@mui/material";

type SpinnerProps = {
    in?: boolean;
    timeout?: number;
    sx?: object;
};

export const Spinner = ({ in: inProp = true, timeout = 300, sx, ...rest }: SpinnerProps) => {
    return (
        <Fade in={inProp} mountOnEnter unmountOnExit style={{ transitionDelay: timeout.toString() }}>
            <CircularProgress sx={{ margin: "auto", ...sx }} {...rest} />
        </Fade>
    );
};
