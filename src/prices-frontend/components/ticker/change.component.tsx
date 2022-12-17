import { Typography } from "@mui/material";
import { memo } from "react";
import { toPascalCase } from "../../utils/string";
import { direction, formatPercent } from "../../utils/number";

type ChangeProps = {
    value: number;
    options?: Intl.NumberFormatOptions;
    locales?: string;
    size?: "tiny" | "small" | "medium" | "large";
};

export const Change = memo<ChangeProps>(function Change({ value, options, locales, size = "medium" }) {
    return (
        <Typography
            variant="caption"
            sx={(theme) => ({
                color: theme.palette["trend"][direction(value)],
                fontSize: theme.typography[`fontSize${toPascalCase(size)}`],
                fontWeight: theme.typography.fontWeightMedium,
                lineHeight: 1,
            })}
        >
            {formatPercent(value, options, locales)}
        </Typography>
    );
});
