import { SvgIcon } from "@mui/material";
import { DefaultComponentProps, OverridableTypeMap } from "@mui/material/OverridableComponent";

interface AppIconProps extends OverridableTypeMap {}

export const AppIcon = (props: DefaultComponentProps<AppIconProps>) => {
    return (
        <SvgIcon viewBox="0 0 100 100" {...props}>
            <polygon
                fill="#09A69F"
                points="39.5,47.6 25.3,55.8 25.3,60.6 29.5,63 43.7,54.8 58,63 62.2,60.6 62.2,55.8 48,47.6 48,31.3 
	43.7,28.9 39.5,31.3 39.5,47.6 "
            />
            <path
                fill="#4263f5"
                d="M43.8,0L0,25v50l43.8,25l43.7-25l0,0V25L43.8,0z M43.8,9.6l31.1,17.8l-8.4,4.8l-22.7-13l-22.7,13l-8.4-4.8
	L43.8,9.6z M39.5,88L8.4,70.2V34.6l8.4,4.8v26l22.7,13V88z M25.3,60.6V39.4l18.5-10.6l18.5,10.6v21.1L43.8,71.1L25.3,60.6z M48,88
	v-9.6l22.7-13v-26l8.4-4.8v35.6h0L48,88z"
            />
        </SvgIcon>
    );
};
