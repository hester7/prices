import { Typography } from "@mui/material";
import { memo } from "react";
import { toPascalCase } from "../../utils/string";
import { formatCurrency } from "../../utils/number";

type PriceProps = {
    value: number;
    options?: Intl.NumberFormatOptions;
    locales?: string;
    size?: "tiny" | "small" | "medium" | "large";
};

export const Price = memo<PriceProps>(function Price({ value, options, locales, size = "medium" }) {
    return (
        <Typography
            variant="caption"
            sx={(theme) => ({
                color: theme.palette.text.primary,
                fontSize: theme.typography[`fontSize${toPascalCase(size)}`],
                fontWeight: theme.typography.fontWeightMedium,
                lineHeight: 1,
            })}
        >
            {formatCurrency(value, options, locales)}
        </Typography>
    );
});
